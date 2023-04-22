using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Model.Collections
{
    /// <summary>
    /// Interface ICheckedList
    /// Implements the <see cref="ICollection" />
    /// </summary>
    /// <seealso cref="ICollection" />
    public interface ICheckedList : ICollection
    {
        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        object[] Items { get; }

        /// <summary>
        /// Gets the source items.
        /// </summary>
        /// <value>The source items.</value>
        object[] SourceItems { get; }

        /// <summary>
        /// Determines whether the specified item is checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
        bool IsChecked(object item);

        /// <summary>
        /// Sets the checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="checked">if set to <c>true</c> [checked].</param>
        void SetChecked(object item, bool @checked);

        /// <summary>
        /// Sets the range checked.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="checked">if set to <c>true</c> [checked].</param>
        void SetRangeChecked(IEnumerable<object> items, bool @checked);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Selects the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Select(object item);

        /// <summary>
        /// Selects the range.
        /// </summary>
        /// <param name="items">The items.</param>
        void SelectRange(IEnumerable<object> items);
    }

    /// <summary>
    /// Class CheckedList.
    /// Implements the <see cref="ICollection{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ICollection{T}" />
    public class CheckedList<T> : ICollection<T>, ICheckedList
    {
        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// The source items core
        /// </summary>
        List<T> SourceItemsCore = new List<T>();

        /// <summary>
        /// The selected items
        /// </summary>
        List<T> ItemsCore = new List<T>();

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public T[] Items => ItemsCore.ToArray();

        /// <summary>
        /// Gets the source items.
        /// </summary>
        /// <value>The source items.</value>
        public T[] SourceItems => SourceItemsCore.ToArray();

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count => ItemsCore.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether this instance is synchronized.
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        public bool IsSynchronized => ((ICollection)ItemsCore).IsSynchronized;

        /// <summary>
        /// Gets the synchronize root.
        /// </summary>
        /// <value>The synchronize root.</value>
        public object SyncRoot => ((ICollection)ItemsCore).SyncRoot;

        private bool IsUpdating = false;

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        object[] ICheckedList.Items
        {
            get
            {
                List<object> list = new List<object>();
                foreach (T item in ItemsCore)
                {
                    list.Add(item);
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets the source items.
        /// </summary>
        /// <value>The source items.</value>
        object[] ICheckedList.SourceItems
        {
            get
            {
                List<object> list = new List<object>();
                foreach (T item in SourceItemsCore)
                {
                    list.Add(item);
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public CheckedList(IEnumerable<T> items)
        {
            SourceItemsCore.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="checkedItems">The checked items.</param>
        public CheckedList(IEnumerable<T> items, IEnumerable<T> checkedItems)
        {
            SourceItemsCore.AddRange(items);
            SetRangeChecked(checkedItems, true);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CheckedList<T> other)
            {
                return ItemsCore.Equals(other.ItemsCore) && SourceItemsCore.Equals(other.SourceItemsCore);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private void BeginUpdate()
        {
            IsUpdating = true;
        }

        private void EndUpdate()
        {
            IsUpdating = false;

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(T item)
        {
            if (SourceItemsCore.Contains(item) && !ItemsCore.Contains(item))
            {
                ItemsCore.Add(item);

                if(!IsUpdating)
                {
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }                
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            if (ItemsCore.Count > 0)
            {
                ItemsCore.Clear();

                if(!IsUpdating)
                {
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }                
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(T item)
        {
            return ItemsCore.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ItemsCore.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)ItemsCore).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ItemsCore.GetEnumerator();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(T item)
        {
            if (ItemsCore.Remove(item))
            {
                if(!IsUpdating)
                {
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ItemsCore.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the specified item is checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
        bool ICheckedList.IsChecked(object item)
        {
            return ItemsCore.Contains((T)item);
        }

        void ICheckedList.SetChecked(object item, bool @checked)
        {
            if (item is T t)
            {
                SetChecked(t, @checked);
            }
        }

        /// <summary>
        /// Sets the range checked.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="checked">if set to <c>true</c> [checked].</param>
        void ICheckedList.SetRangeChecked(IEnumerable<object> items, bool @checked)
        {
            SetRangeChecked(items.Cast<T>(), @checked);
        }

        /// <summary>
        /// Determines whether the specified item is checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
        public bool IsChecked(T item)
        {
            return ItemsCore.Contains(item);
        }

        /// <summary>
        /// Sets the checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="checked">if set to <c>true</c> [checked].</param>
        public void SetChecked(T item, bool @checked)
        {
            if (@checked)
            {
                Add(item);
            }
            else
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Sets the range checked.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="checked">if set to <c>true</c> [checked].</param>
        public void SetRangeChecked(IEnumerable<T> items, bool @checked)
        {
            BeginUpdate();

            try
            {
                foreach (var item in items)
                {
                    SetChecked(item, @checked);
                }
            }
            finally
            {
                EndUpdate();
            }            
        }

        /// <summary>
        /// Selects the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Select(T item)
        {
            BeginUpdate();

            try
            {
                Clear();
                SetChecked(item, true);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Selects the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void SelectRange(IEnumerable<T> items)
        {
            BeginUpdate();

            try
            {
                Clear();

                foreach (var item in items)
                {
                    SetChecked(item, true);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Selects the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void ICheckedList.Select(object item)
        {
            if(item is T t)
            {
                Select(t);
            }            
        }

        /// <summary>
        /// Selects the range.
        /// </summary>
        /// <param name="items">The items.</param>
        void ICheckedList.SelectRange(IEnumerable<object> items)
        {
            SelectRange(items.Cast<T>());
        }
    }

}
