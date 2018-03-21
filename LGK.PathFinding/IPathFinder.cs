// See LICENSE file in the root directory
//
namespace LGK.PathFinding
{
    public interface IPathFinder<T> where T : INode
    {
        void Find(NodePosition from, NodePosition to, Path<T> path);
    }
}