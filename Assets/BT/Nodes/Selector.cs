namespace BT
{
    public class Selector : Composite
    {
        protected override Status Update()
        {
            foreach (var child in GetChildren())
            {
                Status status = child.Tick();

                if (status != Status.Failure)
                {
                    return status;
                }
            }

            return Status.Failure;
        }
    }
}
