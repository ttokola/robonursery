namespace BT
{
    public class Parallel : Composite
    {
        protected override Status Update()
        {
            int running = 0;

            foreach (var child in GetChildren())
            {
                Status status = child.Tick();

                if (status == Status.Running)
                {
                    running++;
                }
                else if (status != Status.Success)
                {
                    return Status.Failure;
                }
            }

            if (running > 0)
            {
                return Status.Running;
            }
            else
            {
                return Status.Success;
            }
        }
    }
}
