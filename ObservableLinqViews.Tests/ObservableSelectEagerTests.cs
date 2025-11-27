using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using static System.Collections.Specialized.NotifyCollectionChangedAction;

namespace ObservableLinqViews.Tests;

[TestClass]
public sealed class ObservableSelectEagerTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    public void Count_ReturnsTheCorrectCount(int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));
        var view = source.SelectEager(x => x.ToString())
            .SelectEager(x => x.Length);

        var result = view.Count;

        Assert.AreEqual(count, result);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(0, 2)]
    [DataRow(1, 2)]
    [DataRow(0, 5)]
    [DataRow(1, 5)]
    [DataRow(2, 5)]
    [DataRow(3, 5)]
    [DataRow(4, 5)]
    public void Indexer_ReturnsTheCorrectElement(int i, int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));
        var view = source.SelectEager(x => x.ToString());

        var result = view[i];

        Assert.AreEqual(i.ToString(), result);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    public void GetEnumerator_ReturnsEnumerator(int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));

        var result = source.SelectEager(x => x.ToString());

        var expected = source.Select(x => x.ToString()).ToList();
        CollectionAssert.AreEqual(expected, result.ToList());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    public void Reset_IsRaised(int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Clear();

        Assert.HasCount(1, log);
        Assert.AreEqual(Reset, log[0].Action);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    public void Reset_UpdatesMappedItems(int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));
        var view = source.SelectEager(x => x.ToString());

        source.Clear();

        CollectionAssert.AreEqual(new List<string>(), view.ToList());
    }

    [TestMethod]
    public void Add_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Add(5);

        Assert.HasCount(1, log);
        Assert.AreEqual(Add, log[0].Action);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("5", log[0].NewItems![0]);
        Assert.AreEqual(5, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Add_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Add(5);

        var expected = new List<string>() { "0", "1", "2", "3", "4", "5" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Add_ToTheMiddle_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Insert(1, 5);

        Assert.HasCount(1, log);
        Assert.AreEqual(Add, log[0].Action);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("5", log[0].NewItems![0]);
        Assert.AreEqual(1, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Add_ToTheMiddle_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Insert(1, 5);

        var expected = new List<string>() { "0", "5", "1", "2", "3", "4" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Remove_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.RemoveAt(2);

        Assert.HasCount(1, log);
        Assert.AreEqual(Remove, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("2", log[0].OldItems![0]);
        Assert.AreEqual(2, log[0].OldStartingIndex);
    }

    [TestMethod]
    public void Remove_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.RemoveAt(2);

        var expected = new List<string>() { "0", "1", "3", "4" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Replace_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source[2] = 8;

        Assert.HasCount(1, log);
        Assert.AreEqual(Replace, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("2", log[0].OldItems![0]);
        Assert.AreEqual(2, log[0].OldStartingIndex);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("8", log[0].NewItems![0]);
        Assert.AreEqual(2, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Replace_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source[2] = 8;

        var expected = new List<string>() { "0", "1", "8", "3", "4" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Move_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Move(1, 3);

        Assert.HasCount(1, log);
        Assert.AreEqual(Move, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("1", log[0].OldItems![0]);
        Assert.AreEqual(1, log[0].OldStartingIndex);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("1", log[0].NewItems![0]);
        Assert.AreEqual(3, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Move_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Move(1, 3);

        var expected = new List<string>() { "0", "2", "3", "1", "4" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Move_Forward_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Move(3, 1);

        Assert.HasCount(1, log);
        Assert.AreEqual(Move, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("3", log[0].OldItems![0]);
        Assert.AreEqual(3, log[0].OldStartingIndex);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("3", log[0].NewItems![0]);
        Assert.AreEqual(1, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Move_Forward_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Move(3, 1);

        var expected = new List<string>() { "0", "3", "1", "2", "4" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Move_ToTheEnd_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Move(1, 4);

        Assert.HasCount(1, log);
        Assert.AreEqual(Move, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("1", log[0].OldItems![0]);
        Assert.AreEqual(1, log[0].OldStartingIndex);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("1", log[0].NewItems![0]);
        Assert.AreEqual(4, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Move_ToTheEnd_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Move(1, 4);

        var expected = new List<string>() { "0", "2", "3", "4", "1" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void Move_FromTheEnd_IsRaised()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var log = new List<NotifyCollectionChangedEventArgs>();
        var view = source.SelectEager(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Move(4, 1);

        Assert.HasCount(1, log);
        Assert.AreEqual(Move, log[0].Action);
        Assert.HasCount(1, log[0].OldItems!);
        Assert.AreEqual("4", log[0].OldItems![0]);
        Assert.AreEqual(4, log[0].OldStartingIndex);
        Assert.HasCount(1, log[0].NewItems!);
        Assert.AreEqual("4", log[0].NewItems![0]);
        Assert.AreEqual(1, log[0].NewStartingIndex);
    }

    [TestMethod]
    public void Move_FromTheEnd_UpdatesMappedItems()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        source.Move(4, 1);

        var expected = new List<string>() { "0", "4", "1", "2", "3" };
        CollectionAssert.AreEqual(expected, view.ToList());
    }

    [TestMethod]
    public void SelectFn_IsCalledOnce()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var callCount = 0;

        var view = source.SelectEager(x =>
        {
            callCount++;
            return x.ToString();
        });

        _ = view[2];
        _ = view[2];

        Assert.AreEqual(5, callCount);
    }

    [TestMethod]
    public void CollectionChange_WithoutEventHandler_Works()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectEager(x => x.ToString());

        try
        {
            source.Clear();
        }
        catch
        {
            Assert.Fail("Exception was thrown when no event handler was attached.");
        }
    }
}
