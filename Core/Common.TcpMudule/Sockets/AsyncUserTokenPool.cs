using System;
using System.Collections.Generic;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// AsyncUserToken对象池（固定缓存设计）
    /// </summary>
    public class AsyncUserTokenPool
    {
        private readonly Queue<AsyncUserToken> _pool;

        private static readonly object Lock = new object();

        /// <summary>
        /// Initializes the object pool to the specified size
        /// The "capacity" parameter is the maximum number of
        /// AsyncUserToken objects the pool can hold
        /// </summary>
        public AsyncUserTokenPool(int capacity)
        {
            _pool = new Queue<AsyncUserToken>(capacity);
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool
        /// The "item" parameter is the AsyncUserToken instance
        /// to add to the pool
        /// </summary>
        public void Push(AsyncUserToken item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (Lock)
            {
                _pool.Enqueue(item);
            }
        }

        /// <summary>
        /// Removes a AsyncUserToken instance from the pool
        /// and returns the object removed from the pool
        /// </summary>
        /// <returns></returns>
        public AsyncUserToken Pop()
        {
            lock (Lock)
            {
                return _pool.Dequeue();
            }
        }

        /// <summary>
        /// The number of AsyncUserToken instances in the pool
        /// </summary>
        public int Count => _pool.Count;
    }
}
