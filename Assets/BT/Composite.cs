using System;
using System.Collections.Generic;

namespace BT
{
    public class Composite : INode
    {
        private readonly List<INode> children = new List<INode>();

        public void AddChild(INode child)
        {
            children.Add(child);
        }

        public void RemoveChild(INode child)
        {
            children.Remove(child);
        }

        public IEnumerable<INode> GetChildren()
        {
            return children.AsReadOnly();
        }

        public virtual Status Tick()
        {
            throw new NotImplementedException();
        }
    }
}
