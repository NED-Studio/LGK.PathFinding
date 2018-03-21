// See LICENSE file in the root directory
//

namespace LGK.PathFinding
{
    public class Path<T> where T : INode
    {
        static readonly T DEFAULT = default(T);

        public readonly byte Capacity;

        readonly T[] m_Nodes;

        byte m_CurrentIndex;
        byte m_Count;
        bool m_Locked;

        public Path(byte capacity)
        {
            Capacity = capacity;
            m_Nodes = new T[capacity];
            m_CurrentIndex = 0;
        }

        public byte Count
        {
            get { return m_Count; }
        }

        public T this[byte index]
        {
            get
            {
                if (index >= m_Count)
                    throw new System.IndexOutOfRangeException();

                return m_Nodes[index];
            }
        }

        public bool IsReady
        {
            get { return !m_Locked; }
        }

        public bool IsValid
        {
            get { return m_CurrentIndex < m_Count; }
        }

        public void Clear()
        {
            m_Count = 0;
            m_CurrentIndex = 0;

            Reset();
        }

        public bool Add(T point)
        {
            if (m_Count == m_Nodes.Length)
                return false;

            m_Nodes[m_Count] = point;
            m_Count++;
            return true;
        }

        public bool MoveNext()
        {
            if (m_CurrentIndex < m_Count)
            {
                m_CurrentIndex++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            m_CurrentIndex = 0;
        }

        public T Current
        {
            get
            {
                if (m_CurrentIndex >= m_Count)
                    return DEFAULT;

                return this[m_CurrentIndex];
            }
        }

        internal void Lock()
        {
            m_Locked = true;
        }

        internal void UnLock()
        {
            System.Array.Reverse(m_Nodes, 0, m_Count);

            m_Locked = false;

            Reset();
        }
    }
}