using BT;

namespace Env
{
    public abstract class Objective : Node
    {
        public Progress progress;

        public bool IsCompleted()
        {
            return progress == Progress.COMPLETE;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            progress = Progress.INCOMPLETE;
        }

        protected override Status Update()
        {
            progress = Check();

            if (IsCompleted())
            {
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }

        protected abstract Progress Check();
    }
}