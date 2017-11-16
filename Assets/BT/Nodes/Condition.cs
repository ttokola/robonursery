namespace BT
{
    public class Condition : INode
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

        public Status Tick()
        {
            if (Evaluate())
            {
                return Status.SUCCESS;
            }
            else
            {
                return Status.FAILURE;
            }
        }
    }
}
