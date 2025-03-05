using System;
using System.Collections.Generic;
using System.Linq;

namespace WindNight.RabbitMq.@internal
{
    internal class ConcurrentQueue<T>
    {
        private readonly Queue<T> m_Queue;
        private readonly object m_SyncRoot;

        public ConcurrentQueue()
        {
            m_SyncRoot = new object();
            m_Queue = new Queue<T>();
        }

        public ConcurrentQueue(IEnumerable<T> collection)
        {
            m_SyncRoot = new object();
            m_Queue = new Queue<T>(collection);
        }

        public ConcurrentQueue(int capacity)
        {
            m_SyncRoot = new object();
            m_Queue = new Queue<T>(capacity);
        }

        public int Count
        {
            get
            {
                lock (m_SyncRoot)
                {
                    return m_Queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (m_SyncRoot)
            {
                m_Queue.Enqueue(item);
            }
        }

        public bool Any(Func<T, bool> predicate)
        {
            return m_Queue.Any(predicate);
        }

        public bool TryDequeue(out T item)
        {
            lock (m_SyncRoot)
            {
                if (m_Queue.Count <= 0)
                {
                    item = default;
                    return false;
                }

                item = m_Queue.Dequeue();
                return true;
            }
        }

        public void Clear()
        {
            lock (m_SyncRoot)
            {
                m_Queue.Clear();
            }
        }
    }
}
