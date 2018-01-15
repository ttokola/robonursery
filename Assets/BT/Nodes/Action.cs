namespace RobotNursery.BT
{
    public class Action : Node
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
                return Status.Failure;
            }
        }

        protected override Status Update()
        {
            return Execute();
        }
    }
}
