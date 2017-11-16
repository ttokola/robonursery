namespace BT
{
    public class Action : INode
    {
        public delegate Status ActionDelegate();

        private readonly ActionDelegate actionDelegate;

        public Action()
            : this(null)
        {
            
        }

        public Action(ActionDelegate actionDelegate)
        {
            this.actionDelegate = actionDelegate;
        }

        public virtual Status Execute()
        {
            if (actionDelegate != null)
            {
                return actionDelegate();
            }
            else
            {
                return Status.FAILURE;
            }
        }

        public Status Tick()
        {
            return Execute();
        }
    }
}
