namespace RobotNursery.BT
{
    public class AlwaysSucceed : Decorator
    {
        public AlwaysSucceed(Node child) : base(child)
        {
        }

        protected override Status Update()
        {
            Child.Tick();

            return Status.Success;
        }
    }
}
