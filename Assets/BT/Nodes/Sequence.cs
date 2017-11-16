namespace BT
{
    public class Sequence : Composite
    {
        public override Status Tick()
        {
            foreach (var node in GetChildren())
            {
                Status status = node.Tick();

                if (status != Status.SUCCESS)
                {
                    return status;
                }
            }

            return Status.SUCCESS;
        }
    }
}
