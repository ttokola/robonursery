namespace BT
{
    public class OneShot : Decorator
    {
        private bool success;

        public OneShot(Node child) : base(child)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            success = false;
        }

        protected override Status Update()
        {
            if (!success)
            {
                Status status = Child.Tick();

                if (status == Status.Success)
                {
                    success = true;
                }

                return status;
            }
            else
            {
                return Status.Success;
            }
        }
    }
}
