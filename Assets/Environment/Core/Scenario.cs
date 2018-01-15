#define LOG_INFO

using System;
using UnityEngine;

namespace RobotNursery.Environment
{
    public abstract partial class Scenario : MonoBehaviour
    {
        protected virtual void OnBegin() { }

        protected virtual void OnEnd() { }
    }

    public abstract partial class Scenario : MonoBehaviour
    {
        private int sessions;

        public virtual Session CreateSession(Supervisee supervisee)
        {
            return new Session(this, supervisee);
        }

        public partial class Session
        {
            public event EventHandler BeginEvent;

            public event EventHandler StepEvent;

            public event EventHandler EndEvent;

            public Supervisee Supervisee { get; private set; }

            public Scenario Parent { get; private set; }

            protected virtual void OnBegin() { }

            protected virtual void OnEnd() { }

            protected virtual void OnStep() { }
        }

        public partial class Session
        {
            public bool Running { get; private set; }

            public Session(Scenario parent, Supervisee supervisee)
            {
                Parent = parent;
                Supervisee = supervisee;
            }

            private void Notify(EventHandler handler)
            {
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            public void Begin()
            {
                if (!Running)
                {
                    if (Parent.sessions == 0)
                    {
                        Log.Info("Scenario begin: {0}", Parent);

                        Parent.OnBegin();
                    }

                    Log.Info("Scenario session begin: {0}", this);

                    Parent.sessions++;

                    OnBegin();
                    Notify(BeginEvent);

                    Running = true;
                }
            }

            public void Step()
            {
                if (Running)
                {
                    OnStep();
                    Notify(StepEvent);
                }
            }

            public void End()
            {
                if (Running)
                {
                    OnEnd();
                    Notify(EndEvent);

                    Parent.sessions--;

                    Log.Info("Scenario session end: {0}", this);

                    if (Parent.sessions == 0)
                    {
                        Log.Info("Scenario end: {0}", Parent);

                        Parent.OnEnd();
                    }
                }
            }
        }
    }
}
