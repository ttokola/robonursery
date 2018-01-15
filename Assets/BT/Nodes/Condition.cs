namespace RobotNursery.BT
{
    public class Condition : Node
    {
        public delegate bool ConditionDelegate();

        private readonly ConditionDelegate conditionDelegate;

        public Condition()
            : this(null)
        {

        }

        public Condition(ConditionDelegate conditionDelegate)
        {
            this.conditionDelegate = conditionDelegate;
        }

        public virtual bool Evaluate()
        {
            if (conditionDelegate != null)
            {
                return conditionDelegate();
            }
            else
            {
                return false;
            }
        }

        protected override Status Update()
        {
            if (Evaluate())
            {
                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }
}
