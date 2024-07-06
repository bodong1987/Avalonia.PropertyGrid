﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PropertyModels.Collections;

/// <summary>
/// Interface ISelectableList
/// Implements the <see cref="ICollection" />
/// </summary>
/// <seealso cref="ICollection" />
public interface ISelectableList : ICollection
{
    /// <summary>
    /// Occurs when [selection changed].
    /// </summary>
    event EventHandler SelectionChanged;

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>The values.</value>
    object[] Values { get; }

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    /// <value>The selected value.</value>
    object SelectedValue { get; set; }
}

/// <summary>
/// Interface ISelectableList
/// Implements the <see cref="PropertyModels.Collections.ISelectableList" />
/// Implements the <see cref="ICollection{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="PropertyModels.Collections.ISelectableList" />
/// <seealso cref="ICollection{T}" />
// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface ISelectableList<T> : ISelectableList, ICollection<T>
{
    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>The values.</value>
    new T[] Values { get; }

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    /// <value>The selected value.</value>
    new T SelectedValue { get; set; }
}

/// <summary>
/// Class SelectableList.
/// Implements the <see cref="PropertyModels.Collections.ISelectableList{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="PropertyModels.Collections.ISelectableList{T}" />
public class SelectableList<T> : ISelectableList<T>
{
    /// <summary>
    /// The values core
    /// </summary>
    private readonly List<T> _valuesCore = [];

    /// <summary>
    /// The selected value core
    /// </summary>
    private T _selectedValueCore;

    /// <summary>
    /// Occurs when [selection changed].
    /// </summary>
    public event EventHandler SelectionChanged;

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>The values.</value>
    public T[] Values => _valuesCore.ToArray();

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    /// <value>The selected value.</value>
    public T SelectedValue
    {
        get => _selectedValueCore;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(value, _selectedValueCore))
            {
                _selectedValueCore = value;

                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
    /// </summary>
    /// <value>The count.</value>
    public int Count => _valuesCore.Count;

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
    /// </summary>
    /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
    public bool IsSynchronized => false;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
    /// </summary>
    /// <value>The synchronize root.</value>
    public object SyncRoot => false;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    /// </summary>
    /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <value>The values.</value>
    object[] ISelectableList.Values => _valuesCore.Cast<object>().ToArray();

    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    /// <value>The selected value.</value>
    object ISelectableList.SelectedValue
    {
        get => _selectedValueCore;
        set
        {
            if (value != null && !EqualityComparer<T>.Default.Equals((T)value, _selectedValueCore))
            {
                _selectedValueCore = (T)value;

                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableList{T}"/> class.
    /// </summary>
    public SelectableList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableList{T}"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public SelectableList(IEnumerable<T> values)
    {
        _valuesCore.AddRange(values);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableList{T}"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="selectedValue">The selected value.</param>
    public SelectableList(IEnumerable<T> values, T selectedValue) :
        this(values)
    {
        SelectedValue = selectedValue;
    }

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public void Add(T item)
    {
        _valuesCore.Add(item);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    public void Clear()
    {
        _valuesCore.Clear();
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
    public bool Contains(T item)
    {
        return _valuesCore.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    public void CopyTo(Array array, int index)
    {
        ((ICollection)_valuesCore).CopyTo(array, index);
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _valuesCore.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _valuesCore.GetEnumerator();
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public bool Remove(T item)
    {
        return _valuesCore.Remove(item);
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _valuesCore.GetEnumerator();
    }
}