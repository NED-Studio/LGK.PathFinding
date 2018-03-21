// See LICENSE file in the root directory
//

namespace LGK.PathFinding
{
    [System.Serializable]
    public struct NodePosition
    {
        public byte Row;
        public byte Column;

        public NodePosition(byte row, byte column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"({Row},{Column})";
        }
    }
}