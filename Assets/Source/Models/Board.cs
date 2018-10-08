using System;
using System.Collections.Generic;
using System.Text;

public class Board
{
    public delegate void MatchResolvedEventHandler(object sender, MatchResolvedEventArgs e);
    public event MatchResolvedEventHandler MatchResolved;

    public delegate void PieceSpawnedEventHandler(object sender, PieceSpawnedEventArgs e);
    public event PieceSpawnedEventHandler PieceSpawned;

    private readonly BoardPiece[,] _board;

    private BoardPiece _selectedPiece;
    private BoardPiece _swapCandidate;

    public readonly int Width;
    public readonly int Height;
    public readonly int MinMatchSize;

    private readonly List<int> _pieceTypes;
    private readonly Random _random;

    public Board(int width, int height, int minMatchSize, HashSet<int> pieceTypes, int randomSeed)
    {
        if (pieceTypes.Count < 3)
        {
            throw new ArgumentOutOfRangeException("pieceTypes", "Board requires at least 3 types of pieces to avoid the L case.");
        }

        Width = width;
        Height = height;
        _board = new BoardPiece[width, height];
        MinMatchSize = minMatchSize;
        _pieceTypes = new List<int>(pieceTypes);
        _random = new Random(randomSeed);
    }

    public void Fill(params BoardPiece[] pieces)
    {
        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                SetPieceAt(new BoardPiece(pieces.Length > 0 ? pieces[0].Type : int.MinValue), x, y);
            }
        }
    }

    public BoardPiece GetPieceAt(int x, int y)
    {
        return _board[x, y];
    }

    public void RemovePieceAt(int x, int y)
    {
        if (_board[x, y] == null)
        {
            return;
        }
        _board[x, y].EnterRemovedState();
        _board[x, y] = null;
    }

    public void MovePiecesDown()
    {
        int emptySpaces = 0;
        for (int x = 0; x < _board.GetLength(0); x++)
        {
            emptySpaces = 0;
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                if (_board[x, y] == null)
                {
                    emptySpaces++;
                    continue;
                }
                if (emptySpaces > 0)
                {
                    MovePieceTo(_board[x, y], x, y - emptySpaces);
                }
            }

            int startingSpaceHeight = emptySpaces;
            while (emptySpaces > 0)
            {
                var newPiece = GetNewBoardPiece(_random.Next(0, _pieceTypes.Count));
                SetPieceAt(newPiece, x, Height - emptySpaces--);

                if (PieceSpawned != null)
                {
                    PieceSpawned(this, new PieceSpawnedEventArgs(newPiece, startingSpaceHeight - emptySpaces));
                }
            }
        }
    }

    public void ResolveMatch(Match match)
    {
        foreach (var piece in match)
        {
            RemovePieceAt(piece.X, piece.Y);
        }

        if (MatchResolved != null)
        {
            MatchResolved(this, new MatchResolvedEventArgs(match));
        }
    }

    public void ResolveMatches(Matches matches)
    {
        foreach (var match in matches)
        {
            ResolveMatch(match);
        }
    }

    public Matches GetMatchesForCandidates()
    {
        var matches = new Matches();
        var match1 = GetMatchFor(_selectedPiece);
        if (match1 != null)
        {
            matches.Add(match1);
        }
        var match2 = GetMatchFor(_swapCandidate);
        if (match2 != null)
        {
            matches.Add(match2);
        }
        return matches.Count > 0 ? matches : null;
    }

    private void MovePieceTo(BoardPiece boardPiece, int x, int y)
    {
        SetPieceAt(boardPiece, x, y);
        boardPiece.EnterFallingState();
    }

    public void SetPieceAt(BoardPiece piece, int x, int y)
    {
        piece.SetBoardPosition(x, y);
        _board[x, y] = piece;
    }

    private BoardPiece GetNewBoardPiece(int type)
    {
        // TODO: add pool
        var newPiece = new BoardPiece(type);
        return newPiece;
    }

    public Match GetMatchFor(BoardPiece movedPiece)
    {
        var horizontalMatch = new Match(movedPiece.Type) { movedPiece };
        var verticalMatch = new Match(movedPiece.Type) { movedPiece };

        // check left
        for (int i = movedPiece.X - 1; i >= 0; i--)
        {
            if (_board[i, movedPiece.Y] == null || !_board[i, movedPiece.Y].Matches(movedPiece))
            {
                break;
            }
            horizontalMatch.Add(_board[i, movedPiece.Y]);
        }

        // check right
        for (int i = movedPiece.X + 1; i < _board.GetLength(0); i++)
        {
            if (_board[i, movedPiece.Y] == null || !_board[i, movedPiece.Y].Matches(movedPiece))
            {
                break;
            }
            horizontalMatch.Add(_board[i, movedPiece.Y]);
        }

        // check up
        for (int j = movedPiece.Y + 1; j < _board.GetLength(1); j++)
        {
            if (_board[movedPiece.X, j] == null || !_board[movedPiece.X, j].Matches(movedPiece))
            {
                break;
            }
            verticalMatch.Add(_board[movedPiece.X, j]);
        }

        // check down
        for (int j = movedPiece.Y - 1; j >= 0; j--)
        {
            if (_board[movedPiece.X, j] == null || !_board[movedPiece.X, j].Matches(movedPiece))
            {
                break;
            }
            verticalMatch.Add(_board[movedPiece.X, j]);
        }

        if (horizontalMatch.Count >= MinMatchSize && verticalMatch.Count >= MinMatchSize)
        {
            horizontalMatch.UnionWith(verticalMatch);
            return horizontalMatch;
        }
        else if (horizontalMatch.Count >= MinMatchSize)
        {
            return horizontalMatch;
        }
        else if (verticalMatch.Count >= MinMatchSize)
        {
            return verticalMatch;
        }

        return null;
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x > Width || y < 0 || y > Height;
    }

    public void SelectPiece(BoardPiece boardPiece)
    {
        if (_selectedPiece != null && _selectedPiece != boardPiece && AreNeighbors(_selectedPiece, boardPiece))
        {
            _swapCandidate = boardPiece;
        }
        else
        {
            _selectedPiece = boardPiece;
        }
    }

    private bool AreNeighbors(BoardPiece piece1, BoardPiece piece2)
    {
        var dx = Math.Abs(piece1.X - piece2.X);
        var dy = Math.Abs(piece1.Y - piece2.Y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public bool IsReadyToSwap()
    {
        return _selectedPiece != null && _swapCandidate != null
            && _selectedPiece.CurrentState == BoardPiece.EState.ReadyForMatch
            && _swapCandidate.CurrentState == BoardPiece.EState.ReadyForMatch;
    }

    public void SwapCandidates()
    {
        Swap(_selectedPiece, _swapCandidate);
    }

    private void Swap(BoardPiece piece1, BoardPiece piece2)
    {
        int tempX = piece1.X;
        int tempY = piece1.Y;

        SetPieceAt(piece1, piece2.X, piece2.Y);
        SetPieceAt(piece2, tempX, tempY);

        piece1.EnterSwapState();
        piece2.EnterSwapState();
    }

    public void RandomFillUp()
    {
        var tempList = new List<int>(_pieceTypes);

        int preMatchSize = MinMatchSize - 1;

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                tempList.Clear();
                tempList.AddRange(_pieceTypes);

                // check left
                if (x >= preMatchSize)
                {
                    int deltaX = x - 1;
                    for (; deltaX > 0; deltaX--)
                    {
                        if (!_board[deltaX, y].Matches(_board[deltaX - 1, y]))
                        {
                            break;
                        }
                    }

                    if (x - deltaX >= preMatchSize)
                    {
                        tempList.Remove(_board[x - 1, y].Type);
                    }
                }

                // check downwards
                if (y >= preMatchSize)
                {
                    int deltaY = y - 1;
                    for (; deltaY > 0; deltaY--)
                    {
                        if (!_board[x, deltaY].Matches(_board[x, deltaY - 1]))
                        {
                            break;
                        }
                    }

                    if (y - deltaY >= preMatchSize)
                    {
                        tempList.Remove(_board[x, y - 1].Type);
                    }
                }

                int randomValue = _random.Next(0, tempList.Count);
                var randomPiece = tempList[randomValue];
                SetPieceAt(new BoardPiece(randomPiece), x, y);
            }
        }
    }

    public void ConfirmSwappedPieces()
    {
        _selectedPiece = _swapCandidate = null;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        for (int y = _board.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                sb.Append((_board[x, y].Type != int.MinValue) ? _board[x, y].Type.ToString() : "N");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public class MatchResolvedEventArgs
{
    public readonly Match Match;

    public MatchResolvedEventArgs(Match match)
    {
        Match = match;
    }
}

public class PieceSpawnedEventArgs
{
    public readonly BoardPiece NewPiece;
    public readonly int StartingHeight;

    public PieceSpawnedEventArgs(BoardPiece newPiece, int startingHeight)
    {
        NewPiece = newPiece;
        StartingHeight = startingHeight;
    }
}
