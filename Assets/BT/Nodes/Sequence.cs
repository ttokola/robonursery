namespace BT
{
    public class Sequence : Composite
    {
        protected override Status Update()
        {
            foreach (var child in GetChildren())
            {
                Status status = child.Tick();

                if (status != Status.Success)
                {
                    return status;
                }
            }

            return Status.Success;
        }
    }
}
