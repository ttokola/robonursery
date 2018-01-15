using System;
using System.Collections.Generic;

namespace RobotNursery.BT
{
    public class Model
    {
        private readonly Node root;
        private List<Layer> layers = new List<Layer>();

        public Model(Node root)
        {
            if (root == null)
            {
                throw new NullReferenceException("root == null");
            }

            this.root = root;
        }

        public void Reset()
        {
            root.Terminate();
            layers.ForEach(layer => layer.Reset());
        }

        public void Step()
        {
            root.Tick();
            layers.ForEach(layer => layer.Step());
        }

        public Layer CreateLayer()
        {
            Layer layer = new Layer();
            layers.Add(layer);
            return layer;
        }
    }
}
