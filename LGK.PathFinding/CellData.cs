// See LICENSE file in the root directory
//

namespace LGK.PathFinding
{
    internal class CellData
    {
        public readonly NodePosition Position;
        public ushort HeapIndex;

        public byte Version;
        public bool IsClosed;
        public byte Level;
        public CellData Parent;

        public int GCost;
        public int HCost;
        public int FCost;

        public CellData(byte row, byte column)
        {
            Position.Row = row;
            Position.Column = column;
        }

        public sbyte CompareTo(ref CellData other)
        {
            sbyte compare;
            if (FCost == other.FCost)
            {
                if (HCost == other.HCost)
                    compare = 0;
                else if (HCost > other.HCost)
                    compare = 1;
                else
                    compare = -1;
            }
            else if (FCost > other.FCost)
            {
                compare = 1;
            }
            else
            {
                compare = -1;
            }

            return compare;
        }
    }
}
