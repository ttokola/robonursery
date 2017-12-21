using BT;
using System;
using System.Collections.Generic;

namespace RobotNursery
{
    public class Layer
    {
        private List<Binding> bindings = new List<Binding>();

        public void Reset()
        {
            bindings.ForEach(binding => binding.Reset());
        }

        public void Step()
        {
            bindings.ForEach(binding => binding.Step());
        }

        public void Bind(Node trigger, Node branch)
        {
            if (trigger == null)
            {
                throw new NullReferenceException("trigger == null");
            }
            if (branch == null)
            {
                throw new NullReferenceException("branch == null");
            }

            Binding binding = new Binding(trigger, branch);

            bindings.Add(binding);
        }

        public void Unbind(Node trigger)
        {
            bindings.RemoveAll(binding
                => ReferenceEquals(binding.Trigger, trigger));
        }

        private class Binding
        {
            public Node Trigger { get { return trigger; } }

            private readonly Node trigger;
            private readonly Node branch;
            private Status previousStatus;

            public Binding(Node trigger, Node branch)
            {
                this.trigger = trigger;
                this.branch = branch;
                previousStatus = Status.Unknown;
            }

            public void Reset()
            {
                branch.Terminate();
                previousStatus = Status.Unknown;
            }

            public void Step()
            {
                Status currentStatus = trigger.Status;

                if (previousStatus != currentStatus
                    && currentStatus == Status.Success)
                {
                    Log.Verbose("Status changed to {0}", currentStatus);

                    branch.Tick();
                    previousStatus = currentStatus;
                }
            }
        }
    }
}
