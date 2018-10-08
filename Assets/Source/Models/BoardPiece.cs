using System;

public class BoardPiece
{
    public event EventHandler Removed;
    public event EventHandler Fell;
    public event EventHandler Swapped;

    public readonly int Type;

    public int X { get; private set; }
    public int Y { get; private set; }

    public EState CurrentState { get; private set; }
    public enum EState
    {
        ReadyForMatch,
        Falling,
        Removed,
        UnderSwap
    }

    public BoardPiece(int type)
    {
        Type = type;
    }

    public BoardPiece(BoardPiece boardPiece) : this(boardPiece.Type) { }

    public void SetBoardPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void EnterReadyForMatchState()
    {
        CurrentState = EState.ReadyForMatch;
    }

    public void EnterSwapState()
    {
        CurrentState = EState.UnderSwap;
        if (Swapped != null)
        {
            Swapped(this, EventArgs.Empty);
        }
    }

    public void EnterRemovedState()
    {
        CurrentState = EState.Removed;
        if (Removed != null)
        {
            Removed(this, EventArgs.Empty);
        }
    }

    public void EnterFallingState()
    {
        CurrentState = EState.Falling;
        if (Fell != null)
        {
            Fell(this, EventArgs.Empty);
        }
    }

    public bool Matches(BoardPiece targetPieceType)
    {
        return targetPieceType.Type != int.MinValue && Type != int.MinValue && targetPieceType.Type == Type 
            && targetPieceType.CurrentState == EState.ReadyForMatch && CurrentState == EState.ReadyForMatch;
    }

    public override string ToString()
    {
        return string.Format("[X={0}, Y={1}, _CurrentState={2}, Type={3}]",
            X, Y, CurrentState, Type);
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
            && other.CurrentState == CurrentState
            && other.Type == Type;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 ^ X.GetHashCode();
            hash = hash * 23 ^ Y.GetHashCode();
            hash = hash * 23 ^ CurrentState.GetHashCode();
            hash = hash * 23 ^ Type.GetHashCode();
            return hash;
        }
    }
}
