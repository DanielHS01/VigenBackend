using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace VigenBackend.Tests.Mocks
{
    public class AsyncEnumerableMock<T> : IAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly IQueryable<T> _queryable;

        public AsyncEnumerableMock(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
            _queryable = enumerable.AsQueryable();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumeratorMock<T>(_enumerable.GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator() => _queryable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();

        public Type ElementType => _queryable.ElementType;
        public System.Linq.Expressions.Expression Expression => _queryable.Expression;
        public IQueryProvider Provider => _queryable.Provider;
    }

    public class AsyncEnumeratorMock<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumeratorMock(IEnumerator<T> inner) { _inner = inner; }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }
}
