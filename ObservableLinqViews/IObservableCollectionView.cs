using System.Collections.Generic;
using System.Collections.Specialized;

namespace ObservableLinqViews;

public interface IObservableCollectionView<TElement> : IReadOnlyList<TElement>, INotifyCollectionChanged
{
}

public interface IObservableCollectionViewLazy<TElement> : IObservableCollectionView<TElement>
{
}

public interface IObservableCollectionViewEager<TElement> : IObservableCollectionView<TElement>
{
}
