using System.Collections.ObjectModel;
using System.ComponentModel;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.UnitTests.Model;

[TestClass]
public class UnitTestGenericInterfaceInvoker
{
    [TestMethod]
    public void Test_BindingList()
    {
        var list = new BindingList<int> { 1, 2, 3 };

        // Test Add
        GenericInterfaceInvoker.Add(list, 4);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(4, list[3]);

        // Test RemoveAt
        GenericInterfaceInvoker.RemoveAt(list, 1);
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(3, list[1]);

        // Test Contains
        var contains = GenericInterfaceInvoker.Contains(list, 3);
        Assert.IsTrue(contains);

        // Test IndexOf
        var index = GenericInterfaceInvoker.IndexOf(list, 3);
        Assert.AreEqual(1, index);

        // Test Insert
        GenericInterfaceInvoker.Insert(list, 1, 5);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(5, list[1]);

        // Test Remove
        var removed = GenericInterfaceInvoker.Remove(list, 5);
        Assert.IsTrue(removed);
        Assert.AreEqual(3, list.Count);

        // Test Count
        var count = GenericInterfaceInvoker.Count(list);
        Assert.AreEqual(3, count);

        // Test GetItem
        var item = GenericInterfaceInvoker.GetItem(list, 0);
        Assert.AreEqual(1, item);

        // Test SetItem
        GenericInterfaceInvoker.SetItem(list, 0, 10);
        Assert.AreEqual(10, list[0]);

        // Test Clear
        GenericInterfaceInvoker.Clear(list);
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void Test_List()
    {
        var list = new List<int> { 1, 2, 3 };

        // Test Add
        GenericInterfaceInvoker.Add(list, 4);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(4, list[3]);

        // Test RemoveAt
        GenericInterfaceInvoker.RemoveAt(list, 1);
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(3, list[1]);

        // Test Contains
        var contains = GenericInterfaceInvoker.Contains(list, 3);
        Assert.IsTrue(contains);

        // Test IndexOf
        var index = GenericInterfaceInvoker.IndexOf(list, 3);
        Assert.AreEqual(1, index);

        // Test Insert
        GenericInterfaceInvoker.Insert(list, 1, 5);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(5, list[1]);

        // Test Remove
        var removed = GenericInterfaceInvoker.Remove(list, 5);
        Assert.IsTrue(removed);
        Assert.AreEqual(3, list.Count);

        // Test Count
        var count = GenericInterfaceInvoker.Count(list);
        Assert.AreEqual(3, count);

        // Test GetItem
        var item = GenericInterfaceInvoker.GetItem(list, 0);
        Assert.AreEqual(1, item);

        // Test SetItem
        GenericInterfaceInvoker.SetItem(list, 0, 10);
        Assert.AreEqual(10, list[0]);

        // Test Clear
        GenericInterfaceInvoker.Clear(list);
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void Test_ObservableCollection()
    {
        var list = new ObservableCollection<int> { 1, 2, 3 };

        // Test Add
        GenericInterfaceInvoker.Add(list, 4);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(4, list[3]);

        // Test RemoveAt
        GenericInterfaceInvoker.RemoveAt(list, 1);
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(3, list[1]);

        // Test Contains
        var contains = GenericInterfaceInvoker.Contains(list, 3);
        Assert.IsTrue(contains);

        // Test IndexOf
        var index = GenericInterfaceInvoker.IndexOf(list, 3);
        Assert.AreEqual(1, index);

        // Test Insert
        GenericInterfaceInvoker.Insert(list, 1, 5);
        Assert.AreEqual(4, list.Count);
        Assert.AreEqual(5, list[1]);

        // Test Remove
        var removed = GenericInterfaceInvoker.Remove(list, 5);
        Assert.IsTrue(removed);
        Assert.AreEqual(3, list.Count);

        // Test Count
        var count = GenericInterfaceInvoker.Count(list);
        Assert.AreEqual(3, count);

        // Test GetItem
        var item = GenericInterfaceInvoker.GetItem(list, 0);
        Assert.AreEqual(1, item);

        // Test SetItem
        GenericInterfaceInvoker.SetItem(list, 0, 10);
        Assert.AreEqual(10, list[0]);

        // Test Clear
        GenericInterfaceInvoker.Clear(list);
        Assert.AreEqual(0, list.Count);
    }
}