namespace BT
{
    public class Selector : Composite
    {
        public override Status Tick()
        {
            foreach (var node in GetChildren())
            {
                Status status = node.Tick();

                if (status != Status.FAILURE)
                {
                    return status;
                }
            }

            return Status.FAILURE;
        }
    }
}
