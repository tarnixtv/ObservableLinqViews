using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace ObservableLinqViews;

[ExcludeFromCodeCoverage]
public static class NotifyCollectionChangedExtensions
{
    public static IObservableCollectionViewLazy<TResult> SelectLazy<TSource, TResult>(this ObservableCollection<TSource> source, Func<TSource, TResult> f)
        => new ObservableSelectLazy<ObservableCollection<TSource>, TSource, TResult>(source, f);

    public static IObservableCollectionViewLazy<TResult> SelectLazy<TSource, TResult>(this IObservableCollectionViewLazy<TSource> source, Func<TSource, TResult> f)
        => new ObservableSelectLazy<IObservableCollectionViewLazy<TSource>, TSource, TResult>(source, f);

    public static IObservableCollectionViewLazy<TResult> SelectLazy<TIntermediate, TResult>(this IObservableCollectionViewEager<TIntermediate> source, Func<TIntermediate, TResult> f)
        => new ObservableSelectLazy<IObservableCollectionViewEager<TIntermediate>, TIntermediate, TResult>(source, f);

    public static IObservableCollectionViewEager<TResult> SelectEager<TSource, TResult>(this ObservableCollection<TSource> source, Func<TSource, TResult> f)
        => new ObservableSelectEager<ObservableCollection<TSource>, TSource, TResult>(source, f);

    public static IObservableCollectionViewEager<TResult> SelectEager<TSource, TResult>(this IObservableCollectionViewEager<TSource> source, Func<TSource, TResult> f)
        => new ObservableSelectEager<IObservableCollectionViewEager<TSource>, TSource, TResult>(source, f);

    public static IObservableCollectionViewEager<TResult> SelectEager<TSource, TResult>(this IObservableCollectionViewLazy<TSource> source, Func<TSource, TResult> f)
        => new ObservableSelectEager<IObservableCollectionViewLazy<TSource>, TSource, TResult>(source, f);
}
