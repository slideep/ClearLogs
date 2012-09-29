namespace ClearLogs.Common
{
    internal sealed class Pair<TLeft, TRight> where TLeft : class where TRight : class
    {
        public Pair(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }

        public TLeft Left { get; private set; }

        public TRight Right { get; private set; }

        public override int GetHashCode()
        {
            var leftHash = (Left == null ? 0 : Left.GetHashCode());
            var rightHash = (Right == null ? 0 : Right.GetHashCode());

            return leftHash ^ rightHash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Pair<TLeft, TRight>;

            if (other == null)
                return false;

            return Equals(Left, other.Left) && Equals(Right, other.Right);
        }
    }
}