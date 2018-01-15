namespace RobotNursery.BT
{
    public class Parallel : Composite
    {
        private readonly ICondition successCondition;
        private readonly ICondition failureCondition;

        public Parallel() : this(Condition.All, Condition.One)
        {
        }

        public Parallel(
            ICondition successCondition,
            ICondition failureCondition)
        {
            this.successCondition = successCondition;
            this.failureCondition = failureCondition;
        }

        protected override Status Update()
        {
            int successCount = 0;
            int failureCount = 0;

            foreach (var child in GetChildren())
            {
                Status status = child.Tick();

                if (status == Status.Success)
                {
                    successCount++;
                }
                else if (status == Status.Failure)
                {
                    failureCount++;
                }

                // Question/improvement:
                // Fail/success here instead at the end? Either way would be
                // somewhat "correct" as tasks are executed in a parallel manner
                // (well, not actually...).
            }

            Status result = Check(successCount, failureCount);

            Log.Verbose("successCount: {0}, failureCount: {1}, result: {2}",
                successCount, failureCount, result);

            return result;
        }

        private Status Check(int successCount, int failureCount)
        {
            if (IsFailure(failureCount))
            {
                return Status.Failure;
            }
            else if (IsSuccess(successCount))
            {
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }

        private bool IsSuccess(int successCount)
        {
            return successCondition.Check(successCount, this);
        }

        private bool IsFailure(int failureCount)
        {
            return failureCondition.Check(failureCount, this);
        }

        public interface ICondition
        {
            bool Check(int count, Parallel node);
        }

        public class Condition : ICondition
        {
            public static readonly Condition One
                = new Condition { Count = 1 };
            public static readonly Condition All
                = new Condition { Count = -1 };

            public int Count { get; private set; }

            public bool Check(int count, Parallel node)
            {
                int threshold = IsAllRequired()
                    ? node.ChildCount /* All */
                    : Count           /* Custom */;

                return count >= threshold;
            }

            private bool IsAllRequired()
            {
                return Count == -1;
            }
        }
    }
}
