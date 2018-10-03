public class BoardPiece
{
    public readonly int Type;

    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsRemoved { get; private set; }

    public BoardPiece(int type)
    {
        Type = type;
    }

    public BoardPiece(BoardPiece boardPiece)
    {
        Type = boardPiece.Type;
    }

    public void SetBoardPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void RemoveFromBoard()
    {
        IsRemoved = true;
    }

    public bool Matches(BoardPiece targetPieceType)
    {
        return targetPieceType.Type != int.MinValue && Type != int.MinValue && targetPieceType.Type == Type;
    }

    public override string ToString()
    {
        return string.Format("[X={0}, Y={1}, IsRemoved={2}, Type={3}]",
            X, Y, IsRemoved, Type);
    }

    public override bool Equals(object obj)
    {
        var other = obj as BoardPiece;
        if (other == null)
        {
            return false;
        }

        return other.X == X
            && other.Y == Y
            && other.IsRemoved == other.IsRemoved
            && other.Type == Type;
    }

    public override int GetHashCode()
    {
        // TODO: evaluate this
        unchecked
        {
            int hash = 17;
            hash = hash * 23 ^ X.GetHashCode();
            hash = hash * 23 ^ Y.GetHashCode();
            hash = hash * 23 ^ IsRemoved.GetHashCode();
            hash = hash * 23 ^ Type.GetHashCode();
            return hash;
        }
    }
}
