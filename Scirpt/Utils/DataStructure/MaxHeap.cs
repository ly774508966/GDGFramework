using System;
using System.Linq;
using System.Text;

namespace GDG.Utils
{
    public class MaxHeap<T> where T : IComparable<T>
    {
        private T[] heap;
        private int Capacity;
        public int Count { get; private set; }
        public MaxHeap(int capacity = 100)
        {
            this.Capacity = capacity + 1;
            heap = new T[capacity];
            Count = 0;
        }
        public MaxHeap(T[] array,int capacity = 100)
        {
            this.Capacity = capacity + 1;
            Count = array.Length;
            
            T[] zero = new T[1] { default(T) };
            array = zero.Concat(array).ToArray();
            Array.Resize(ref array, capacity + 1);

            for (int i = array.Length / 2 ; i >= 1; i--)
            {
                ShiftDown(ref array, i);
            }
            heap = array;
        }
        public void Heapify()
        {
            for (int i = Count / 2 ; i >= 1; i--)
            {
                ShiftDown(ref heap, i);
            }            
        }
        private void SwitchNode(ref T node1, ref T node2)
        {
            T temp = node1;
            node1 = node2;
            node2 = temp;
        }
        private void ShiftUp(ref T[] array, int index)
        {
            while (index > 0)
            {
                //如果比父节点小，则与父节点交换
                if (array[index].CompareTo(array[index / 2]) > 0)
                {
                    SwitchNode(ref array[index], ref array[index / 2]);
                    index = index / 2;
                }
                else
                    break;
            }
        }
        private void ShiftDown(ref T[] array, int index)
        {
            //要比较的孩子
            int tempIndex = 0;
            while (true)
            {
                //如果既有左孩子，又有右孩子，则比较两孩子大小
                if (index * 2 < Count && index * 2 + 1 < Count)
                {
                    if (array[index * 2].CompareTo(array[index * 2 + 1]) > 0)
                    {
                        tempIndex = index * 2;
                    }
                    else
                    {
                        tempIndex = index * 2 + 1;
                    }
                }
                //如果只有左孩子
                else if (index * 2 < Count && index * 2 + 1 > Count)
                {
                    tempIndex = index * 2;
                }
                //如果没有孩子
                else
                    break;

                //与最小的孩子比
                if (array[index].CompareTo(array[tempIndex]) < 0)
                {
                    SwitchNode(ref array[index], ref array[tempIndex]);
                    index = tempIndex;
                }
                else
                    break;
            }
        }
        /// <summary>
        /// 插入结点并向上调整
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryInsert(T item)
        {
            if (Count >= Capacity)
                return false;
            Count++;
            heap[Count] = item;
            ShiftUp(ref heap, Count);
            return true;
        }
        /// <summary>
        /// 移除最小结点并向下调整
        /// </summary>
        /// <returns></returns>
        public bool TryRemoveMin()
        {
            if (Count == 0)
                return false;
            heap[1] = heap[Count];
            ShiftDown(ref heap, 1);
            Count--;
            return true;
        }
        public T GetMax()
        {
            if (Count > 0)
                return heap[1];
            else
                return default(T);
        }
        public override string ToString()
        {
            if (Count == 0)
                return "null";
            if (Count == 1)
                return $"[{heap[1]}]";

            return "[" + string.Join(", ", heap.Skip(1).Take(Count).ToArray()) + "]";
        }
        public T[] ToArray()
        {
            return heap.Skip(1).Take(Count).ToArray();
        }
    }
}