using RobotNursery.BT;
using System;
using System.Collections.Generic;

namespace RobotNursery.Environment
{
    public class Supervisor
    {
        private readonly Node model;
        private List<Layer> layers = new List<Layer>();

        public Supervisor(Node model)
        {
            if (model == null)
            {
                throw new NullReferenceException("model == null");
            }

            this.model = model;
        }

        public void Reset()
        {
            model.Terminate();

            layers.ForEach(layer => layer.Reset());
        }

        public void Step()
        {
            model.Tick();

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
