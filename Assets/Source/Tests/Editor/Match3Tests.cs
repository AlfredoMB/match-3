﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

public class Match3Tests
{
    public class BoardTests
    {
        protected const int _width = 8;
        protected const int _height = 8;
        protected const int _minMatchSize = 3;
        protected Board _board;
        
        [SetUp]
        public void SetUp()
        {
            _board = new Board(_width, _height, _minMatchSize);
            _board.Fill(new BoardPiece(int.MinValue));
        }
    }

    public class BoardInitializationTests : BoardTests
    {
        [Test]
        public void FillBoard()
        {
            Assert.NotNull(_board.GetPieceAt(0, 0));
        }
    }

    public class BoardPieceRemovalTests : BoardTests
    {
        [Test]
        public void BoardPiecesFallWhenPieceIsRemoved()
        {
            int targetPieceY = 4;

            BoardPiece upperPiece = _board.GetPieceAt(0, targetPieceY + 1);
            _board.RemovePieceAt(0, targetPieceY);
            _board.MovePiecesDown();

            Assert.AreSame(upperPiece, _board.GetPieceAt(0, targetPieceY));
        }

        [Test]
        public void BoardPiecesFallWhen3VerticalPiecesAreRemoved()
        {
           int targetPieceY = 0;

            BoardPiece upperPiece = _board.GetPieceAt(0, targetPieceY + 3);
            _board.RemovePieceAt(0, targetPieceY + 2);
            _board.RemovePieceAt(0, targetPieceY + 1);
            _board.RemovePieceAt(0, targetPieceY);
            _board.MovePiecesDown();

            Assert.AreSame(upperPiece, _board.GetPieceAt(0, targetPieceY));
        }

        [Test]
        public void NewBoardPieceIsCreatedAtTheTopWhenPieceIsRemoved()
        {
            int topPieceY = _height - 1;
            int targetPieceY = 0;

            BoardPiece topPiece = _board.GetPieceAt(0, topPieceY);
            _board.RemovePieceAt(0, targetPieceY);
            _board.MovePiecesDown();

            Assert.AreNotSame(topPiece, _board.GetPieceAt(0, topPieceY));
            Assert.AreSame(topPiece, _board.GetPieceAt(0, topPieceY - 1));
        }

        [Test]
        public void NewBoardPiecesAreCreatedAtTheTopWhen3VerticalPiecesAreRemoved()
        {
            int topPieceY = _height - 1;
            int targetPieceY = 0;

            BoardPiece topPiece = _board.GetPieceAt(0, topPieceY);
            _board.RemovePieceAt(0, targetPieceY + 2);
            _board.RemovePieceAt(0, targetPieceY + 1);
            _board.RemovePieceAt(0, targetPieceY);
            _board.MovePiecesDown();

            Assert.AreNotSame(topPiece, _board.GetPieceAt(0, topPieceY));
            Assert.AreNotSame(topPiece, _board.GetPieceAt(0, topPieceY - 1));
            Assert.AreNotSame(topPiece, _board.GetPieceAt(0, topPieceY - 2));
            Assert.AreSame(topPiece, _board.GetPieceAt(0, topPieceY - 3));
        }

        [Test]
        public void RemoveMatchPieces()
        {
            int targetX = 0;

            BoardPiece matchPiece = new BoardPiece(0);
            _board.SetPieceAt(matchPiece, targetX, 0);
            _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 0);
            _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);

            _board.SetMovedPieceAt(targetX, 0);

            var matches = _board.GetMatchesFromMovedPieces();
            _board.RemovePiecesFromMatch(matches[0]);
            Assert.IsTrue(!matches[0].Exists(piece => !piece.IsRemoved));
        }
    }

    public class MatchRecognitionTests
    {
        public class HorizontalRecognitionTests : BoardTests
        {
            [Test]
            public void RecognizeHorizontal3PieceStartingOnLeftAsMatch()
            {
                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);

                _board.SetMovedPieceAt(targetX, 0);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void RecognizeHorizontal3PieceStartingOnMiddleAsMatch()
            {
                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX - 1, 0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 0);

                _board.SetMovedPieceAt(targetX, 0);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void RecognizeHorizontal3PieceStartingOnRightAsMatch()
            {
                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX - 2, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX - 1, 0);
                _board.SetPieceAt(matchPiece, targetX, 0);

                _board.SetMovedPieceAt(targetX, 0);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void DeclineHorizontal2PieceStartingOnLeftAsMatch()
            {
                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 0);

                _board.SetMovedPieceAt(targetX, 0);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void DeclineHorizontal2PieceStartingOnRightAsMatch()
            {
                int targetX = 1;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX - 1, 0);
                _board.SetPieceAt(matchPiece, targetX, 0);

                _board.SetMovedPieceAt(targetX, 0);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void TwoHorizontalMatchesForDifferentPiecesCanHappen()
            {
                int targetX = 2;
                int target1Y = 0;
                int target2Y = target1Y + 1;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX - 1, target1Y);
                _board.SetPieceAt(matchPiece, targetX, target1Y);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, target1Y);

                BoardPiece matchPiece2 = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece2), targetX - 1, target2Y);
                _board.SetPieceAt(matchPiece2, targetX, target2Y);
                _board.SetPieceAt(new BoardPiece(matchPiece2), targetX + 1, target2Y);

                _board.SetMovedPieceAt(targetX, target1Y);
                _board.SetMovedPieceAt(targetX, target2Y);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(2, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                Assert.AreEqual(3, matches[1].Count);
                Assert.That((matches[0].Contains(matchPiece) && matches[1].Contains(matchPiece2))
                    || (matches[0].Contains(matchPiece2) && matches[1].Contains(matchPiece)));
            }

            [Test]
            public void SwapHorizontalNeighborPieces()
            {
                BoardPiece swappedPiece1 = new BoardPiece(0);
                BoardPiece swappedPiece2 = new BoardPiece(1);

                int x = 0;
                int y = 0;
                int targetX = x + 1;

                _board.SetPieceAt(swappedPiece1, x, y);
                _board.SetPieceAt(swappedPiece2, targetX, y);

                _board.SelectPieceAt(x, y);
                _board.SelectPieceAt(targetX, y);

                Assert.IsTrue(_board.TryToSwap());
                Assert.AreEqual(swappedPiece2, _board.GetPieceAt(x, y));
                Assert.AreNotEqual(swappedPiece1, _board.GetPieceAt(x, y));
                Assert.AreEqual(swappedPiece1, _board.GetPieceAt(targetX, y));
                Assert.AreNotEqual(swappedPiece2, _board.GetPieceAt(targetX, y));
            }

            [Test]
            public void FailToSwapHorizontalNotNeighborPieces()
            {
                BoardPiece swappedPiece1 = new BoardPiece(0);
                BoardPiece swappedPiece2 = new BoardPiece(1);

                int x = 0;
                int y = 0;
                int targetX = x + 2;

                _board.SetPieceAt(swappedPiece1, x, y);
                _board.SetPieceAt(swappedPiece2, targetX, y);

                _board.SelectPieceAt(x, y);
                _board.SelectPieceAt(targetX, y);

                Assert.IsFalse(_board.TryToSwap());
                Assert.AreEqual(swappedPiece1, _board.GetPieceAt(x, y));
                Assert.AreNotEqual(swappedPiece2, _board.GetPieceAt(x, y));
                Assert.AreEqual(swappedPiece2, _board.GetPieceAt(targetX, y));
                Assert.AreNotEqual(swappedPiece1, _board.GetPieceAt(targetX, y));
            }
        }
        
        public class VerticalRecognitionTests : BoardTests
        {
            [Test]
            public void RecognizeVertical3PieceStartingOnLeftAsMatch()
            {
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, 0, targetY);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY + 1);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY + 2);

                _board.SetMovedPieceAt(0, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void RecognizeVertical3PieceStartingOnMiddleAsMatch()
            {
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY - 1);
                _board.SetPieceAt(matchPiece, 0, targetY);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY + 1);

                _board.SetMovedPieceAt(0, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void RecognizeVertical3PieceStartingOnRightAsMatch()
            {
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY - 2);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY - 1);
                _board.SetPieceAt(matchPiece, 0, targetY);

                _board.SetMovedPieceAt(0, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(1, matches.Count);
                Assert.AreEqual(3, matches[0].Count);
                CollectionAssert.Contains(matches[0], matchPiece);
            }

            [Test]
            public void DeclineVertical2PieceStartingOnLeftAsMatch()
            {
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, 0, targetY);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY + 1);

                _board.SetMovedPieceAt(0, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void DeclineVertical2PieceStartingOnRightAsMatch()
            {
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), 0, targetY - 1);
                _board.SetPieceAt(matchPiece, 0, targetY);

                _board.SetMovedPieceAt(0, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void TwoVerticalMatchesForDifferentPiecesCanHappen()
            {
                int target1X = 0;
                int target2X = target1X + 1;
                int targetY = 2;
                
                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(new BoardPiece(matchPiece), target1X, targetY - 1);
                _board.SetPieceAt(matchPiece, target1X, targetY);
                _board.SetPieceAt(new BoardPiece(matchPiece), target1X, targetY + 1);

                BoardPiece matchPiece2 = new BoardPiece(1);
                _board.SetPieceAt(new BoardPiece(matchPiece2), target2X, targetY - 1);
                _board.SetPieceAt(matchPiece2, target2X, targetY);
                _board.SetPieceAt(new BoardPiece(matchPiece2), target2X, targetY + 1);

                _board.SetMovedPieceAt(target1X, targetY);
                _board.SetMovedPieceAt(target2X, targetY);

                var matches = _board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
                Assert.AreEqual(matches.Count, 2);
                Assert.AreEqual(3, matches[0].Count);
                Assert.AreEqual(3, matches[1].Count);
                Assert.That((matches[0].Contains(matchPiece) && matches[1].Contains(matchPiece2))
                    || (matches[0].Contains(matchPiece2) && matches[1].Contains(matchPiece)));
            }

            [Test]
            public void SwapVerticalNeighborPieces()
            {
                BoardPiece swappedPiece1 = new BoardPiece(0);
                BoardPiece swappedPiece2 = new BoardPiece(1);

                int x = 0;
                int y = 0;
                int targetY = y + 1;

                _board.SetPieceAt(swappedPiece1, x, y);
                _board.SetPieceAt(swappedPiece2, x, targetY);

                _board.SelectPieceAt(x, y);
                _board.SelectPieceAt(x, targetY);

                Assert.IsTrue(_board.TryToSwap());
                Assert.AreEqual(swappedPiece2, _board.GetPieceAt(x, y));
                Assert.AreNotEqual(swappedPiece1, _board.GetPieceAt(x, y));
                Assert.AreEqual(swappedPiece1, _board.GetPieceAt(x, targetY));
                Assert.AreNotEqual(swappedPiece2, _board.GetPieceAt(x, targetY));
            }

            [Test]
            public void FailToSwapHorizontalNotNeighborPieces()
            {
                BoardPiece swappedPiece1 = new BoardPiece(0);
                BoardPiece swappedPiece2 = new BoardPiece(1);

                int x = 0;
                int y = 0;
                int targetY = y + 2;

                _board.SetPieceAt(swappedPiece1, x, y);
                _board.SetPieceAt(swappedPiece2, x, targetY);

                _board.SelectPieceAt(x, y);
                _board.SelectPieceAt(x, targetY);

                Assert.IsFalse(_board.TryToSwap());
                Assert.AreEqual(swappedPiece1, _board.GetPieceAt(x, y));
                Assert.AreNotEqual(swappedPiece2, _board.GetPieceAt(x, y));
                Assert.AreEqual(swappedPiece2, _board.GetPieceAt(x, targetY));
                Assert.AreNotEqual(swappedPiece1, _board.GetPieceAt(x, targetY));
            }
        }
    }
}

public class Match : List<BoardPiece> { }

public class BoardPiece
{
    private readonly int _type;

    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsRemoved { get; private set; }

    public BoardPiece(int type)
    {
        _type = type;
    }

    public BoardPiece(BoardPiece boardPiece)
    {
        _type = boardPiece._type;
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
        return targetPieceType._type == _type;
    }
}

public class Board
{
    private readonly BoardPiece[,] _board;
    private readonly int _minMatchSize;

    private List<BoardPiece> _movedList = new List<BoardPiece>();
    private List<BoardPiece> _removedList = new List<BoardPiece>();

    private List<Match> _matchList = new List<Match>();

    private BoardPiece _selectedPiece;
    private BoardPiece _swapCandidate;

    public Board(int width, int height, int minMatchSize)
    {
        _board = new BoardPiece[width, height];
        _minMatchSize = minMatchSize;
    }

    public void Fill(params BoardPiece[] pieces)
    {
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            for (int j = 0; j < _board.GetLength(1); j++)
            {
                _board[i, j] = new BoardPiece(pieces.Length > 0 ? pieces[0] : null);
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
                _board[removed.X, j] = _board[removed.X, j + 1];
            }
            _board[removed.X, length - 1] = GetNewBoardPiece();
        }
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

    public List<Match> GetMatchesFromMovedPieces()
    {
        _matchList.Clear();
        foreach (var moved in _movedList)
        {
            var targetPieceType = _board[moved.X, moved.Y];
            var horizontalMatch = new Match() { targetPieceType };
            var verticalMatch = new Match() { targetPieceType };

            // check left
            for (int i = moved.X - 1; i >= 0; i--)
            {
                if (!_board[i, moved.Y].Matches(targetPieceType))
                {
                    break;
                }
                horizontalMatch.Add(_board[i, moved.Y]);
            }

            // check right
            for (int i = moved.X + 1; i < _board.GetLength(0); i++)
            {
                if (!_board[i, moved.Y].Matches(targetPieceType))
                {
                    break;
                }
                horizontalMatch.Add(_board[i, moved.Y]);
            }

            // check up
            for (int j = moved.Y + 1; j < _board.GetLength(1); j++)
            {
                if (!_board[moved.X, j].Matches(targetPieceType))
                {
                    break;
                }
                verticalMatch.Add(_board[moved.X, j]);
            }

            // check down
            for (int j = moved.Y - 1; j >= 0; j--)
            {
                if (!_board[moved.X, j].Matches(targetPieceType))
                {
                    break;
                }
                verticalMatch.Add(_board[moved.X, j]);
            }
            
            if (horizontalMatch.Count >= _minMatchSize && verticalMatch.Count >= _minMatchSize)
            {
                horizontalMatch.AddRange(verticalMatch);
                _matchList.Add(horizontalMatch);
            }
            else if (horizontalMatch.Count >= _minMatchSize)
            {
                _matchList.Add(horizontalMatch);
            }
            else if (verticalMatch.Count >= _minMatchSize)
            {
                _matchList.Add(verticalMatch);
            }
        }

        return _matchList;
    }

    public void SelectPieceAt(int x, int y)
    {
        if (_selectedPiece != null && AreNeighbors(_selectedPiece, _board[x, y]))
        {
            _swapCandidate = _board[x, y];
        }
        else
        {
            _selectedPiece = _board[x, y];
        }
    }

    private bool AreNeighbors(BoardPiece piece1, BoardPiece piece2)
    {
        return Math.Abs(piece1.X - piece2.X) == 1
            || Math.Abs(piece1.Y - piece2.Y) == 1;
    }

    public bool TryToSwap()
    {
        if (_selectedPiece == null || _swapCandidate == null)
        {
            return false;
        }

        Swap(_selectedPiece, _swapCandidate);
        return true;
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
}