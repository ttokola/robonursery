namespace BT
{
    public abstract class Decorator : Node
    {
        protected readonly Node Child;

        protected Decorator(Node child)
        {
            Child = child;
        }
    }
}
