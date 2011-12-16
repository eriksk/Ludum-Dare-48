using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gex.Util
{
    public class Pool<T> where T : class, new()
    {
        private T[] items;
        private int count;

        public Pool(int capacity)
        {
            items = new T[capacity];
            for (int i = 0; i < capacity; i++)
            {
                items[i] = new T();
            }
            count = 0;
        }

        public int Count
        {
            get { return count; }
        }

        public T Pop()
        {
            return items[count++];
        }

        public void Push(int index)
        {
            T temp = items[count - 1];
            items[count - 1] = items[index];
            items[index] = temp;
            count--;
        }
        public T this[int index]
        {
            get { return items[index]; }
        }

        public void Clear()
        {
            count = 0;
        }
    }
}
