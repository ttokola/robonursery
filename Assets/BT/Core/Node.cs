//#define LOG_VERBOSE
#define LOG_INFO

namespace BT
{
    public abstract class Node : INode
    {
        public Status Status { get; private set; }

        private bool initialized;
        private bool entered;
        private bool terminate;

        public Status Tick()
        {
            OnPreUpdate();
            OnUpdate();
            OnPostUpdate();

            return Status;
        }

        public void Terminate()
        {
            terminate = true;

            Tick();
        }

        private void OnPreUpdate()
        {
            Log.Verbose("{0}: Status: {1}", this, Status);

            if (!terminate)
            {
                if (!initialized)
                {
                    Log.Info("{0}: Initializing", this);
                    OnInitialize();
                    initialized = true;
                }

                if (!entered)
                {
                    OnEnter();
                    entered = true;
                }
            }
        }

        private void OnUpdate()
        {
            if (!terminate)
            {
                Status = Update();
            }
        }

        private void OnPostUpdate()
        {
            Log.Verbose("{0}: Status: {1}", this, Status);

            if (entered)
            {
                OnLeave();
                entered = false;
            }

            if (terminate)
            {
                if (initialized)
                {
                    Log.Info("{0}: Terminating", this);
                    OnTerminate();
                }

                Status = Status.Unknown;
                initialized = false;
                terminate = false;
            }
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnLeave()
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnTerminate()
        {
        }

        protected abstract Status Update();
    }
}
