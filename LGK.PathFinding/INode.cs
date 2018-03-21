// See LICENSE file in the root directory
//

namespace LGK.PathFinding
{
    public interface INode
    {
        bool Walkable { get; }

        NodePosition NodePosition { get; }
    }
}