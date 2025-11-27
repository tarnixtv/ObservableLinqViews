using System;
using System.Collections.ObjectModel;

namespace ObservableLinqViews;

public static class NotifyCollectionChangedExtensions
{
    public static ObservableSelectLazy<ObservableCollection<TElement>, TElement, TNewElement> SelectLazy<TElement, TNewElement>(this ObservableCollection<TElement> source, Func<TElement, TNewElement> f)
        => new(source, f);

    public static ObservableSelectLazy<ObservableCollection<TElement>, TElement, TNewElement> SelectLazy<TElement, TIntermediate, TNewElement>(this ObservableSelectLazy<ObservableCollection<TElement>, TElement, TIntermediate> source, Func<TIntermediate, TNewElement> f)
        => new(source.Source, _ => f(source.Selector(_)));

    public static ObservableSelectEager<ObservableCollection<TElement>, TElement, TNewElement> SelectEager<TElement, TNewElement>(this ObservableCollection<TElement> source, Func<TElement, TNewElement> f)
        => new(source, f);

    public static ObservableSelectEager<ObservableCollection<TElement>, TElement, TNewElement> SelectEager<TElement, TIntermediate, TNewElement>(this ObservableSelectEager<ObservableCollection<TElement>, TElement, TIntermediate> source, Func<TIntermediate, TNewElement> f)
        => new(source.Source, _ => f(source.Selector(_)));
}
