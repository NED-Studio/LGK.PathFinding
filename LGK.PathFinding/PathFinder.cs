// See LICENSE file in the root directory
//

using System.Collections.Generic;

namespace LGK.PathFinding
{
    public class PathFinder<T> : IPathFinder<T> where T : INode
    {
        const byte BYTE_ONE = 1;

        readonly byte m_RowSize;
        readonly byte m_ColumnSize;

        readonly T[] m_Nodes;
        readonly CellData[] m_Cells;

        readonly OpenSetHeap m_OpenSet;
        readonly List<CellData> m_ClosedSet;

        byte m_Version = 0;

        public PathFinder(byte rowSize, byte columnSize, T[] nodes)
        {
            UnityEngine.Assertions.Assert.IsTrue(nodes.Length == (rowSize * columnSize));

            m_Nodes = nodes;
            m_RowSize = rowSize;
            m_ColumnSize = columnSize;

            m_OpenSet = new OpenSetHeap(20 * 5);
            m_ClosedSet = new List<CellData>(20 * 2);

            m_Cells = new CellData[m_RowSize * m_ColumnSize];
            for (byte i = 0; i < m_RowSize; i++)
            {
                for (byte j = 0; j < m_ColumnSize; j++)
                {
                    m_Cells[i * m_ColumnSize + j] = new CellData(i, j);
                }
            }
        }

        public void Find(T from, T to, Path<T> path)
        {
            Find(from.NodePosition, to.NodePosition, path);
        }

        public void Find(NodePosition from, NodePosition to, Path<T> path)
        {
            UnityEngine.Profiling.Profiler.BeginSample("PathFinder.Find()");

            path.Clear();

            if (!m_Nodes[from.Row * m_ColumnSize + from.Column].Walkable)
                return;

            path.Lock();

            if (m_Version == byte.MaxValue)
                m_Version = 0;
            else
                m_Version++;

            m_OpenSet.FastClear();
            m_ClosedSet.Clear();

            var startCell = m_Cells[from.Row * m_ColumnSize + from.Column];
            var targetCell = m_Cells[to.Row * m_ColumnSize + to.Column];

            startCell.Version = m_Version;
            startCell.Parent = null;
            startCell.Level = 1;
            startCell.GCost = 0;
            startCell.HCost = GetDistance(ref startCell, ref targetCell);
            startCell.FCost = 0;

            m_OpenSet.Add(startCell);

            CellData closestToTarget = startCell;
            CellData currentCell = null;
            bool success = false;

            while (m_OpenSet.Count > 0)
            {
                currentCell = m_OpenSet.RemoveFirst();

                m_ClosedSet.Add(currentCell);

                currentCell.IsClosed = true;

                if ((currentCell == targetCell))
                {
                    success = true;
                    break;
                }
                else if (currentCell.Level > path.Capacity)
                {
                    break;
                }

                ProcessAllNeighbours(ref currentCell, ref targetCell, ref closestToTarget);
            }

            if (success)
                GeneratePath(path, targetCell);
            else
                GeneratePath(path, closestToTarget);

            path.UnLock();

            UnityEngine.Profiling.Profiler.EndSample();
        }

        void ProcessAllNeighbours(ref CellData currentCell, ref CellData targetCell, ref CellData closestToTarget)
        {
            int checkRow = 0;
            int checkColumn = 0;

            for (sbyte x = -1; x <= 1; x++)
            {
                for (sbyte y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    checkRow = currentCell.Position.Row + x;
                    checkColumn = currentCell.Position.Column + y;

                    if ((checkRow >= 0) && (checkRow < m_RowSize) && (checkColumn >= 0) && (checkColumn < m_ColumnSize))
                    {
                        ProcessNeighbour(ref currentCell, ref targetCell, ref closestToTarget, ref m_Cells[checkRow * m_ColumnSize + checkColumn]);
                    }
                }
            }
        }

        void ProcessNeighbour(ref CellData currentCell, ref CellData targetCell, ref CellData closestToTarget, ref CellData neighbourCell)
        {
            if (!m_Nodes[neighbourCell.Position.Row * m_ColumnSize + neighbourCell.Position.Column].Walkable)
                return;

            if (neighbourCell.Version == m_Version && neighbourCell.IsClosed)
                return;

            var newGCostToNeighbour = currentCell.GCost + GetDistance(ref currentCell, ref neighbourCell);
            if (newGCostToNeighbour < neighbourCell.GCost || neighbourCell.Version != m_Version)
            {
                neighbourCell.GCost = newGCostToNeighbour;
                neighbourCell.HCost = GetDistance(ref neighbourCell, ref targetCell);
                neighbourCell.FCost = neighbourCell.GCost + neighbourCell.HCost;
                neighbourCell.Parent = currentCell;

                var level = currentCell.Level;
                neighbourCell.Level = (++level);

                if (neighbourCell.HCost < closestToTarget.HCost)
                {
                    closestToTarget = neighbourCell;
                }

                if (neighbourCell.Version != m_Version)
                {
                    m_OpenSet.Add(neighbourCell);
                    neighbourCell.Version = m_Version;
                    neighbourCell.IsClosed = false;
                }
                else
                    m_OpenSet.UpdateItem(neighbourCell);
            }
        }

        void GeneratePath(Path<T> path, CellData endCell)
        {
            while (endCell.Level != 1)
            {
                path.Add(m_Nodes[endCell.Position.Row * m_ColumnSize + endCell.Position.Column]);

                endCell = endCell.Parent;
            }
        }

        int GetDistance(ref CellData first, ref CellData second)
        {
            int deltaRow = first.Position.Row - second.Position.Row;
            if (deltaRow < 0) deltaRow *= -1;

            int deltaColumn = first.Position.Column - second.Position.Column;
            if (deltaColumn < 0) deltaColumn *= -1;

            if (deltaRow > deltaColumn)
                return 14 * deltaColumn + 10 * (deltaRow - deltaColumn);

            return 14 * deltaRow + 10 * (deltaColumn - deltaRow);
        }
    }
}