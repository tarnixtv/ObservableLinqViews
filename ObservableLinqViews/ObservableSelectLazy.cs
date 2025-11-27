using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static System.Collections.Specialized.NotifyCollectionChangedAction;

namespace ObservableLinqViews;

public class ObservableSelectLazy<TCollection, TElement, TNewElement>
    : IReadOnlyList<TNewElement>, INotifyCollectionChanged
    where TCollection : IReadOnlyList<TElement>, INotifyCollectionChanged
{
    internal readonly TCollection Source;
    internal readonly Func<TElement, TNewElement> Selector;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public TNewElement this[int index] => this.Selector(this.Source[index]);

    public int Count => this.Source.Count;

    public ObservableSelectLazy(TCollection source, Func<TElement, TNewElement> selector)
    {
        this.Source = source;
        this.Selector = selector;

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
            handler(this, e);
        }
        else if (e is { Action: Add, NewItems: IList toBeAdded, NewStartingIndex: int newStartingIndex })
        {
            var newItems = new TNewElement[toBeAdded.Count];

            for (var i = 0; i < toBeAdded.Count; i++)
            {
                newItems[i] = this.Selector((TElement)toBeAdded[i]!);
            }

            var args = new NotifyCollectionChangedEventArgs(Add, newItems, newStartingIndex);

            handler(this, args);
        }
        else if (e is { Action: Remove, OldItems: IList toBeRemoved, OldStartingIndex: int oldStartingIndex })
        {
            var oldItems = new TNewElement[toBeRemoved.Count];

            for (var i = 0; i < toBeRemoved.Count; i++)
            {
                oldItems[i] = this.Selector((TElement)toBeRemoved[i]!);
            }

            var args = new NotifyCollectionChangedEventArgs(Remove, oldItems, oldStartingIndex);

            handler(this, args);
        }
        else if (e is { Action: Replace, NewItems: IList newList, OldItems: IList oldList, NewStartingIndex: int newIndex })
        {
            var newNew = new TNewElement[newList.Count];

            for (var i = 0; i < newList.Count; i++)
            {
                newNew[i] = this.Selector((TElement)newList[i]!);
            }

            var newOld = new TNewElement[oldList.Count];

            for (var i = 0; i < oldList.Count; i++)
            {
                newOld[i] = this.Selector((TElement)oldList[i]!);
            }

            var args = new NotifyCollectionChangedEventArgs(Replace, newNew, newOld, newIndex);

            handler(this, args);
        }
        else if (e is { Action: Move, NewItems: IList movedItems, NewStartingIndex: int target, OldStartingIndex: int source })
        {
            var movedNew = new TNewElement[movedItems.Count];

            for (var i = 0; i < movedItems.Count; i++)
            {
                movedNew[i] = this.Selector((TElement)movedItems[i]!);
            }

            var args = new NotifyCollectionChangedEventArgs(Move, movedNew, target, source);

            handler(this, args);
        }
    }
}