using System.Collections.Generic;

namespace BT
{
    public abstract class Composite : Node
    {
        private readonly List<Node> children = new List<Node>();

        public void Add(Node child)
        {
            children.Add(child);
        }

        public void Remove(Node child)
        {
            children.Remove(child);
        }

        public IEnumerable<Node> GetChildren()
        {
            return children.AsReadOnly();
        }
    }
}
