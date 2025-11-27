using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using static System.Collections.Specialized.NotifyCollectionChangedAction;

namespace ObservableLinqViews.Tests;

[TestClass]
public sealed class ObservableSelectViewTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    public void Count_ReturnsTheCorrectCount(int count)
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, count));
        var view = source.SelectView(x => x.ToString())
            .SelectView(x => x.Length);

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
        var view = source.SelectView(x => x.ToString());

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

        var result = source.SelectView(x => x.ToString());

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
        var view = source.SelectView(x => x.ToString());
        view.CollectionChanged += (s, e) => log.Add(e);

        source.Clear();

        Assert.HasCount(1, log);
        Assert.AreEqual(Reset, log[0].Action);
    }

    [TestMethod]
    public void CollectionChange_WithoutEventHandler_Works()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 5));
        var view = source.SelectView(x => x.ToString());

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
