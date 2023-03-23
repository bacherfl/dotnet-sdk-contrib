using System.Collections.Generic;

namespace OpenFeature.Contrib.Providers.Flagd
{
    interface ICache<TKey, TValue>
    {
        void Add(TKey key, TValue value);
        TValue TryGet(TKey key);
        void Delete(TKey key);
    }
    class LRUCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, Node> _map;
        private Node _head;
        private Node _tail;

        public LRUCache(int capacity)
        {
            _capacity = capacity;
            _map = new Dictionary<TKey, Node>();
        }

        public TValue TryGet(TKey key)
        {
            System.Console.WriteLine("looking for " + key.ToString());
            if (_map.TryGetValue(key, out Node node))
            {
                System.Console.WriteLine("found: " + key.ToString() + " with value: " + node.ToString());
                MoveToFront(node);
                return node.Value;
            }
            return default(TValue);
        }

        public void Add(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out Node node))
            {
                node.Value = value;
                MoveToFront(node);
            }
            else
            {
                if (_map.Count == _capacity)
                {
                    _map.Remove(_tail.Key);
                    RemoveTail();
                }
                node = new Node(key, value);
                _map.Add(key, node);
                AddToFront(node);
            }
        }

        public void Delete(TKey key)
        {
            if (_map.TryGetValue(key, out Node node))
            {
                if (node == _head)
                {
                    _head = node.Next;
                } 
                else 
                {
                    node.Prev.Next = node.Next;
                }
                if (node.Next != null)
                {
                    node.Next.Prev = node.Prev;
                }
                _map.Remove(key);
            }
        }

        private void MoveToFront(Node node)
        {
            if (node == _head)
                return;
            node.Prev.Next = node.Next;
            if (node == _tail)
                _tail = node.Prev;
            else
                node.Next.Prev = node.Prev;
            AddToFront(node);
        }

        private void AddToFront(Node node)
        {
            if (_head == null)
            {
                _head = node;
                _tail = node;
                return;
            }
            node.Next = _head;
            _head.Prev = node;
            _head = node;
        }

        private void RemoveTail()
        {
            _tail = _tail.Prev;
            if (_tail != null)
                _tail.Next = null;
            else
                _head = null;
        }

        private class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Next;
            public Node Prev;

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }

}
