using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static System.Collections.Specialized.NotifyCollectionChangedAction;

namespace ObservableLinqViews;

public class ObservableSelectEager<TCollection, TElement, TNewElement>
    : IReadOnlyList<TNewElement>, INotifyCollectionChanged
    where TCollection : IReadOnlyList<TElement>, INotifyCollectionChanged
{
    internal readonly TCollection Source;
    internal readonly Func<TElement, TNewElement> Selector;
    internal readonly List<TNewElement> MappedItems = [];

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public TNewElement this[int index] => this.MappedItems[index];

    public int Count => this.Source.Count;

    public ObservableSelectEager(TCollection source, Func<TElement, TNewElement> selector)
    {
        this.Source = source;
        this.Selector = selector;

        foreach (var item in this.Source)
        {
            this.MappedItems.Add(this.Selector(item));
        }

        this.Source.CollectionChanged += this.Source_CollectionChanged;
    }

    public IEnumerator<TNewElement> GetEnumerator() => this.Source.Select(this.Selector).GetEnumerator();

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (this.CollectionChanged is not NotifyCollectionChangedEventHandler handler)
        {
            return;
        }

        if (e is { Action: Reset })
        {
            this.MappedItems.Clear();

            handler(this, e);
        }
        else if (e is { Action: Add, NewItems: IList toBeAdded, NewStartingIndex: int newStartingIndex })
        {
            var newItems = new TNewElement[toBeAdded.Count];
            var addToEnd = newStartingIndex == this.MappedItems.Count;

            for (var i = 0; i < toBeAdded.Count; i++)
            {
                var newItem = this.Selector((TElement)toBeAdded[i]!);
                newItems[i] = newItem;

                if (addToEnd)
                {
                    this.MappedItems.Add(newItem);
                }
                else
                {
                    this.MappedItems.Insert(newStartingIndex + i, newItem);
                }
            }

            var args = new NotifyCollectionChangedEventArgs(Add, newItems, newStartingIndex);

            handler(this, args);
        }
        else if (e is { Action: Remove, OldItems: IList toBeRemoved, OldStartingIndex: int oldStartingIndex })
        {
            var oldItems = new TNewElement[toBeRemoved.Count];

            for (var i = 0; i < toBeRemoved.Count; i++)
            {
                oldItems[i] = this.MappedItems[oldStartingIndex];

                this.MappedItems.RemoveAt(oldStartingIndex);
            }

            var args = new NotifyCollectionChangedEventArgs(Remove, oldItems, oldStartingIndex);

            handler(this, args);
        }
        else if (e is { Action: Replace, NewItems: IList newList, OldItems: IList oldList, NewStartingIndex: int index })
        {
            var newOld = new TNewElement[oldList.Count];

            for (var i = 0; i < oldList.Count; i++)
            {
                newOld[i] = this.MappedItems[index + i];
            }

            var newNew = new TNewElement[newList.Count];

            for (var i = 0; i < newList.Count; i++)
            {
                var newItem = this.Selector((TElement)newList[i]!);
                newNew[i] = newItem;
                this.MappedItems[index + i] = newItem;
            }

            var args = new NotifyCollectionChangedEventArgs(Replace, newNew, newOld, index);

            handler(this, args);
        }
        else if (e is { Action: Move, NewItems: IList movedItems, NewStartingIndex: int target, OldStartingIndex: int source })
        {
            var movedNew = new TNewElement[movedItems.Count];

            for (var i = 0; i < movedItems.Count; i++)
            {
                if (source < target)
                {
                    var sourceIndex = source;
                    var destIndex = target;
                    var newElement = this.MappedItems[sourceIndex];
                    movedNew[i] = newElement;

                    if (destIndex == this.MappedItems.Count - 1)
                    {
                        this.MappedItems.Add(newElement);
                    }
                    else
                    {
                        this.MappedItems.Insert(destIndex, newElement);
                    }

                    this.MappedItems.RemoveAt(sourceIndex);
                }
                else
                {
                    var sourceIndex = source + i;
                    var destIndex = target + 1;
                    var newElement = this.MappedItems[sourceIndex];
                    movedNew[i] = newElement;

                    this.MappedItems.Insert(destIndex, newElement);
                    this.MappedItems.RemoveAt(sourceIndex);
                }
            }

            var args = new NotifyCollectionChangedEventArgs(Move, movedNew, target, source);

            handler(this, args);
        }
    }
}