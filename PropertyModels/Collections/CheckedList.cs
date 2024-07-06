using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PropertyModels.Collections;

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
    private readonly List<T> _sourceItemsCore = [];

    /// <summary>
    /// The selected items
    /// </summary>
    private readonly List<T> _itemsCore = [];

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public T[] Items => _itemsCore.ToArray();

    /// <summary>
    /// Gets the source items.
    /// </summary>
    /// <value>The source items.</value>
    public T[] SourceItems => _sourceItemsCore.ToArray();

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <value>The count.</value>
    public int Count => _itemsCore.Count;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets a value indicating whether this instance is synchronized.
    /// </summary>
    /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
    public bool IsSynchronized => ((ICollection)_itemsCore).IsSynchronized;

    /// <summary>
    /// Gets the synchronize root.
    /// </summary>
    /// <value>The synchronize root.</value>
    public object SyncRoot => ((ICollection)_itemsCore).SyncRoot;

    private bool _isUpdating;

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    object[] ICheckedList.Items
    {
        get
        {
            List<object> list = [];
            list.AddRange(_itemsCore.Cast<object>());

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
            List<object> list = [];
            list.AddRange(_sourceItemsCore.Cast<object>());

            return list.ToArray();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedList{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public CheckedList(IEnumerable<T> items)
    {
        _sourceItemsCore.AddRange(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedList{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="checkedItems">The checked items.</param>
    public CheckedList(IEnumerable<T> items, IEnumerable<T> checkedItems)
    {
        _sourceItemsCore.AddRange(items);
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
            return _itemsCore.Equals(other._itemsCore) && _sourceItemsCore.Equals(other._sourceItemsCore);
        }

        return false;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }

    private void BeginUpdate()
    {
        _isUpdating = true;
    }

    private void EndUpdate()
    {
        _isUpdating = false;

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(T item)
    {
        if (_sourceItemsCore.Contains(item) && !_itemsCore.Contains(item))
        {
            _itemsCore.Add(item);

            if(!_isUpdating)
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
        if (_itemsCore.Count > 0)
        {
            _itemsCore.Clear();

            if(!_isUpdating)
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
        return _itemsCore.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _itemsCore.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Copies to.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="index">The index.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void CopyTo(Array array, int index)
    {
        ((ICollection)_itemsCore).CopyTo(array, index);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _itemsCore.GetEnumerator();
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(T item)
    {
        if (_itemsCore.Remove(item))
        {
            if(!_isUpdating)
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
        return _itemsCore.GetEnumerator();
    }

    /// <summary>
    /// Determines whether the specified item is checked.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
    bool ICheckedList.IsChecked(object item)
    {
        return _itemsCore.Contains((T)item);
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
        return _itemsCore.Contains(item);
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