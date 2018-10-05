﻿using System;
using System.Collections.Generic;
using System.Text;

public class Board
{
    public delegate void SwappedEventHandler(object sender, SwappedEventArgs e);

    public event SwappedEventHandler Swapped;

    private readonly BoardPiece[,] _board;

    private List<BoardPiece> _movedList = new List<BoardPiece>();
    private List<BoardPiece> _removedList = new List<BoardPiece>();

    private Matches _matches = new Matches();
    private PossibleMatches _possibleMatches = new PossibleMatches();

    private BoardPiece _selectedPiece;
    private BoardPiece _swapCandidate;

    public readonly int Width;
    public readonly int Height;
    public readonly int MinMatchSize;

    public bool IsReadyToSwap { get { return (_selectedPiece != null && _swapCandidate != null); } }
    public bool IsSwapping { get; private set; }
    public bool ThereAreMatches { get { return _matches.Count > 0; } }
    public bool ThereAreRemovedPieces { get { return _removedList.Count > 0; } }
    public bool ThereAreMovedPieces { get { return _movedList.Count > 0; } }
    public bool IsReadyForInput { get { return !IsSwapping && !ThereAreRemovedPieces && !ThereAreMovedPieces && !ThereAreMatches; } }

    public Board(int width, int height, int minMatchSize)
    {
        Width = width;
        Height = height;
        _board = new BoardPiece[width, height];
        MinMatchSize = minMatchSize;
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
        _board[x, y].RemoveFromBoard();
        _removedList.Add(_board[x, y]);
        _board[x, y] = null;
    }

    public void MovePiecesDown()
    {
        foreach(var removed in _removedList)
        {
            int length = _board.GetLength(1);
            for (int j = removed.Y; j < length - 1; j++)
            {
                MovePieceTo(_board[removed.X, j + 1], removed.X, j);
            }
            _board[removed.X, length - 1] = GetNewBoardPiece();
        }

        _removedList.Clear();
    }

    private void MovePieceTo(BoardPiece boardPiece, int x, int y)
    {
        SetPieceAt(boardPiece, x, y);
        SetMovedPieceAt(x, y);
    }

    public void SetMovedPieceAt(int x, int y)
    {
        _movedList.Add(_board[x, y]);
    }

    public void SetPieceAt(BoardPiece piece, int x, int y)
    {
        piece.SetBoardPosition(x, y);
        _board[x, y] = piece;
    }

    private BoardPiece GetNewBoardPiece()
    {
        return new BoardPiece(int.MinValue);
    }

    public Matches GetMatchesFromMovedPieces()
    {
        _matches.Clear();
        foreach (var movedPiece in _movedList)
        {
            var horizontalMatch = new Match(movedPiece.Type) { movedPiece };
            var verticalMatch = new Match(movedPiece.Type) { movedPiece };

            // check left
            for (int i = movedPiece.X - 1; i >= 0; i--)
            {
                if (!_board[i, movedPiece.Y].Matches(movedPiece))
                {
                    break;
                }
                horizontalMatch.Add(_board[i, movedPiece.Y]);
            }

            // check right
            for (int i = movedPiece.X + 1; i < _board.GetLength(0); i++)
            {
                if (!_board[i, movedPiece.Y].Matches(movedPiece))
                {
                    break;
                }
                horizontalMatch.Add(_board[i, movedPiece.Y]);
            }

            // check up
            for (int j = movedPiece.Y + 1; j < _board.GetLength(1); j++)
            {
                if (!_board[movedPiece.X, j].Matches(movedPiece))
                {
                    break;
                }
                verticalMatch.Add(_board[movedPiece.X, j]);
            }

            // check down
            for (int j = movedPiece.Y - 1; j >= 0; j--)
            {
                if (!_board[movedPiece.X, j].Matches(movedPiece))
                {
                    break;
                }
                verticalMatch.Add(_board[movedPiece.X, j]);
            }
            
            if (horizontalMatch.Count >= MinMatchSize && verticalMatch.Count >= MinMatchSize)
            {
                horizontalMatch.UnionWith(verticalMatch);
                _matches.Add(horizontalMatch);
            }
            else if (horizontalMatch.Count >= MinMatchSize)
            {
                _matches.Add(horizontalMatch);
            }
            else if (verticalMatch.Count >= MinMatchSize)
            {
                _matches.Add(verticalMatch);
            }
        }

        return _matches;
    }

    public void SelectPieceAt(int x, int y)
    {
        SelectPiece(_board[x, y]);
    }

    public void SelectPiece(BoardPiece boardPiece)
    {
        // TODO: add test for 2nd comparision
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
        return Math.Abs(piece1.X - piece2.X) == 1
            || Math.Abs(piece1.Y - piece2.Y) == 1;
    }

    public void SwapCandidates()
    {
        Swap(_selectedPiece, _swapCandidate);
        IsSwapping = !IsSwapping;
        if (Swapped != null)
        {
            Swapped(this, new SwappedEventArgs(_selectedPiece, _swapCandidate));
        }
    }

    private void Swap(BoardPiece piece1, BoardPiece piece2)
    {
        int tempX = piece1.X;
        int tempY = piece1.Y;

        SetPieceAt(piece1, piece2.X, piece2.Y);
        SetPieceAt(piece2, tempX, tempY);

        SetMovedPiece(piece1);
        SetMovedPiece(piece2);
    }

    private void SetMovedPiece(BoardPiece piece)
    {
        SetMovedPieceAt(piece.X, piece.Y);
    }

    public void RemovePiecesFromMatch(Match match)
    {
        foreach(var piece in match)
        {
            RemovePieceAt(piece.X, piece.Y);
        }
    }

    /// <summary>
    /// Fills the board with random pieces with types from types.
    /// </summary>
    /// <param name="types">Types of pieces. Requires at least 3 types to avoid the L case.</param>
    public void RandomFillUp(int seed, params int[] types)
    {
        if (types.Length < 3)
        {
            throw new ArgumentOutOfRangeException("types", "RandomFillUp requires at least 3 types of pieces to avoid the L case.");
        }

        // check unique
        var uniqueCheckSet = new HashSet<int>();
        foreach(var type in types)
        {
            if (!uniqueCheckSet.Add(type))
            {
                throw new ArgumentException("All types should be unique.", "types");
            }
        }

        var random = new Random(seed);
        var typesList = new List<int>(types);
        var tempList = new List<int>(types);

        int preMatchSize = MinMatchSize - 1;

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                tempList.Clear();
                tempList.AddRange(typesList);

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

                int randomValue = random.Next(tempList.Count);
                var randomPiece = tempList[randomValue];
                SetPieceAt(new BoardPiece(randomPiece), x, y);
                SetMovedPieceAt(x, y);
            }
        }
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

    public void ShufflePieces(int seed)
    {
        var random = new Random(seed);
        int maxY = _board.GetLength(1) - 1;
        int maxX = _board.GetLength(0) - 1;

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                int rx = random.Next(0, maxX);
                int ry = random.Next(0, maxY);
                Swap(_board[x, y], _board[rx, ry]);
            }
        }
    }

    public bool AreThereAnyPossibleMatchesLeft()
    {        
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                // swap in left and down (if possible) and check each one
                // swapping up and right would generate same match tests, so we avoid it.

                // left
                if (x > 0)
                {
                    Swap(_board[x, y], _board[x - 1, y]);
                    if (GetMatchesFromMovedPieces().Count > 0)
                    {
                        return true;
                    }
                    Swap(_board[x, y], _board[x - 1, y]);
                    _movedList.Clear();
                }

                // down
                if (y > 0)
                {
                    Swap(_board[x, y], _board[x, y - 1]);
                    if (GetMatchesFromMovedPieces().Count > 0)
                    {
                        return true;
                    }
                    Swap(_board[x, y], _board[x, y - 1]);
                    _movedList.Clear();
                }
            }
        }

        return false;
    }

    public PossibleMatches GetAllPossibleMatchesLeft()
    {
        _possibleMatches.Clear();

        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                // swap in left and down (if possible) and check each one
                // swapping up and right would generate same match tests, so we avoid it.

                // left
                if (x > 0)
                {
                    Swap(_board[x, y], _board[x - 1, y]);
                    var matches = GetMatchesFromMovedPieces();
                    foreach (var match in matches)
                    {
                        _possibleMatches.Add(new PossibleMatch(match, _board[x, y], _board[x - 1, y]));
                    }
                    Swap(_board[x, y], _board[x - 1, y]);
                    _movedList.Clear();
                }

                // down
                if (y > 0)
                {
                    Swap(_board[x, y], _board[x, y - 1]);
                    var matches = GetMatchesFromMovedPieces();
                    foreach(var match in matches)
                    {
                        _possibleMatches.Add(new PossibleMatch(match, _board[x, y], _board[x, y - 1]));
                    }
                    Swap(_board[x, y], _board[x, y - 1]);
                    _movedList.Clear();
                }
            }
        }

        return _possibleMatches;
    }

    public void RemovePiecesFromMatches(Matches currentMatches)
    {
        foreach (var match in currentMatches)
        {
            RemovePiecesFromMatch(match);
        }
    }

    public void ConfirmMovedPieces()
    {
        _movedList.Clear();
    }

    public void ConfirmSwappedPieces()
    {
        IsSwapping = false;
        _selectedPiece = _swapCandidate = null;
    }
}

public class SwappedEventArgs
{
    public readonly BoardPiece SelectedPiece;
    public readonly BoardPiece SwapCandidate;

    public SwappedEventArgs(BoardPiece selectedPiece, BoardPiece swapCandidate)
    {
        SelectedPiece = selectedPiece;
        SwapCandidate = swapCandidate;
    }
}