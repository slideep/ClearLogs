namespace ClearLogs.Common
{
    internal sealed class Pair<TLeft, TRight> where TLeft : class where TRight : class
    {
        private readonly TLeft _left;
        private readonly TRight _right;

        public Pair(TLeft left, TRight right)
        {
            _left = left;
            _right = right;
        }

        public TLeft Left
        {
            get { return _left; }
        }

        public TRight Right
        {
            get { return _right; }
        }

        public override int GetHashCode()
        {
            var leftHash = (_left == null ? 0 : _left.GetHashCode());
            var rightHash = (_right == null ? 0 : _right.GetHashCode());

            return leftHash ^ rightHash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Pair<TLeft, TRight>;

            if (other == null)
                return false;

            return Equals(_left, other._left) && Equals(_right, other._right);
        }
    }
}