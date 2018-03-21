// See LICENSE file in the root directory
//

namespace LGK.PathFinding
{
    internal class OpenSetHeap
    {
        readonly CellData[] m_Items;

        ushort m_Count;

        public OpenSetHeap(int maxHeapSize)
        {
            m_Items = new CellData[maxHeapSize];
        }

        public void Add(CellData item)
        {
            item.HeapIndex = m_Count;
            m_Items[m_Count] = item;
            SortUp(item);
            m_Count++;
        }

        public CellData RemoveFirst()
        {
            CellData firstItem = m_Items[0];
            m_Count--;
            m_Items[0] = m_Items[m_Count];
            m_Items[0].HeapIndex = 0;

            SortDown(m_Items[0]);

            return firstItem;
        }

        public void UpdateItem(CellData item)
        {
            SortUp(item);
        }

        public void FastClear()
        {
            m_Count = 0;
        }

        public int Count
        {
            get
            {
                return m_Count;
            }
        }

        void SortDown(CellData item)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            while (childIndexLeft < m_Count)
            {
                if (childIndexRight < m_Count && m_Items[childIndexLeft].CompareTo(ref m_Items[childIndexRight]) > 0)
                {
                    swapIndex = childIndexRight;
                }
                else
                {
                    swapIndex = childIndexLeft;
                }

                if (item.CompareTo(ref m_Items[swapIndex]) > 0)
                {
                    Swap(item, m_Items[swapIndex]);
                }
                else
                {
                    return;
                }

                childIndexLeft = item.HeapIndex * 2 + 1;
                childIndexRight = item.HeapIndex * 2 + 2;
                swapIndex = 0;
            }
        }

        void SortUp(CellData item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            var parentItem = m_Items[parentIndex];

            while (item.CompareTo(ref parentItem) < 0)
            {
                Swap(item, parentItem);

                parentIndex = (item.HeapIndex - 1) / 2;
                parentItem = m_Items[parentIndex];
            }
        }

        void Swap(CellData itemA, CellData itemB)
        {
            m_Items[itemA.HeapIndex] = itemB;
            m_Items[itemB.HeapIndex] = itemA;

            var itemAIndex = itemA.HeapIndex;

            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }

    }
}