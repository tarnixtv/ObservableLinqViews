using System;
using System.Collections.ObjectModel;

namespace ObservableLinqViews;

public static class NotifyCollectionChangedExtensions
{
    public static ObservableSelectView<ObservableCollection<TElement>, TElement, TNewElement> SelectView<TElement, TNewElement>(this ObservableCollection<TElement> source, Func<TElement, TNewElement> f)
        => new(source, f);

    public static ObservableSelectView<ObservableCollection<TElement>, TElement, TNewElement> SelectView<TElement, TIntermediate, TNewElement>(this ObservableSelectView<ObservableCollection<TElement>, TElement, TIntermediate> source, Func<TIntermediate, TNewElement> f)
        => new(source.Source, _ => f(source.Selector(_)));
}
