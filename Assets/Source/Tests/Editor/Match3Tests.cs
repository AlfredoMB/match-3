using System;
using System.Collections.Generic;
using System.Text;
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

        [Test]
        public void RandomFillUpBoardWithoutMatches()
        {
            // trying to force random test failures
            for (int i = 0; i < 100; i++)
            {
                _board.RandomFillUp(i, 0, 1, 2);
                CollectionAssert.IsEmpty(_board.GetMatchesFromMovedPieces());
            }
        }

        [Test]
        public void BoardReshuffle()
        {
            _board.RandomFillUp(0, 0, 1, 2);
            _board.RandomFillUpCurrentPieces(1);
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

        public class PossibleMatchesTests : BoardTests
        {
            [Test]
            public void DetectPossibleMatches()
            {
                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 1);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);

                Assert.IsTrue(_board.AreThereAnyPossibleMatchesLeft());
            }

            [Test]
            public void ListPossibleMatches()
            {
                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 1);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);

                UnityEngine.Debug.Log(_board);

                var possibleMatches = _board.GetAllPossibleMatchesLeft();    //make unique
                CollectionAssert.IsNotEmpty(possibleMatches);
                Assert.AreEqual(1, possibleMatches.Count);
                Assert.IsTrue(possibleMatches.TrueForAll(match => match.Match.TrueForAll(piece => piece.Type == matchPiece.Type)));
            }
        }
    }
}

public class Match : List<BoardPiece> { }

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
}

public class Board
{
    private readonly BoardPiece[,] _board;
    private readonly int _minMatchSize;

    private List<BoardPiece> _movedList = new List<BoardPiece>();
    private List<BoardPiece> _removedList = new List<BoardPiece>();

    private Matches _matches = new Matches();
    private PossibleMatches _possibleMatches = new PossibleMatches();

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

    public Matches GetMatchesFromMovedPieces()
    {
        _matches.Clear();
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
                _matches.Add(horizontalMatch);
            }
            else if (horizontalMatch.Count >= _minMatchSize)
            {
                _matches.Add(horizontalMatch);
            }
            else if (verticalMatch.Count >= _minMatchSize)
            {
                _matches.Add(verticalMatch);
            }
        }

        return _matches;
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

    /// <summary>
    /// Fills the board with random pieces with types from types.
    /// </summary>
    /// <param name="types">Types of pieces. Requires at least 3 types to avoid the L case.</param>
    public void RandomFillUp(int seed, params int[] types)
    {
        Assert.GreaterOrEqual(types.Length, 3, "The automatic board start up requires at least 3 types of pieces to avoid the L case.");
        CollectionAssert.AllItemsAreUnique(types, "All types should be unique.");

        var random = new Random(seed);
        var typesList = new List<int>(types);
        var tempList = new List<int>(types);

        int preMatchSize = _minMatchSize - 1;

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
                _board[x, y] = new BoardPiece(randomPiece);
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

    public void RandomFillUpCurrentPieces(int v)
    {
        throw new NotImplementedException();
    }

    public bool AreThereAnyPossibleMatchesLeft()
    {        
        for (int y = 0; y < _board.GetLength(1); y++)
        {
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                // swap in 4 directions (if possible) and check each one
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

                // right
                if (x < _board.GetLength(0) - 1)
                {
                    Swap(_board[x, y], _board[x + 1, y]);
                    if (GetMatchesFromMovedPieces().Count > 0)
                    {
                        return true;
                    }
                    Swap(_board[x, y], _board[x + 1, y]);
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

                // up
                if (y < _board.GetLength(1) - 1)
                {
                    Swap(_board[x, y], _board[x, y + 1]);
                    if (GetMatchesFromMovedPieces().Count > 0)
                    {
                        return true;
                    }
                    Swap(_board[x, y], _board[x, y + 1]);
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
                // TODO: remove right and up tests?

                // swap in 4 directions (if possible) and check each one
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

                // right
                if (x < _board.GetLength(0) - 1)
                {
                    Swap(_board[x, y], _board[x + 1, y]);
                    var matches = GetMatchesFromMovedPieces();
                    foreach (var match in matches)
                    {
                        _possibleMatches.Add(new PossibleMatch(match, _board[x, y], _board[x + 1, y]));
                    }
                    Swap(_board[x, y], _board[x + 1, y]);
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

                // up
                if (y < _board.GetLength(1) - 1)
                {
                    Swap(_board[x, y], _board[x, y + 1]);
                    var matches = GetMatchesFromMovedPieces();
                    foreach (var match in matches)
                    {
                        _possibleMatches.Add(new PossibleMatch(match, _board[x, y], _board[x, y + 1]));
                    }
                    Swap(_board[x, y], _board[x, y + 1]);
                    _movedList.Clear();
                }
            }
        }

        return _possibleMatches;
    }
}

public class Matches : List<Match> { }

public class PossibleMatches : List<PossibleMatch> { }

public class PossibleMatch
{
    public readonly Match Match;
    public readonly BoardPiece SwappedPiece1;
    public readonly BoardPiece SwappedPiece2;

    public PossibleMatch(Match match, BoardPiece swappedPiece1, BoardPiece swappedPiece2)
    {
        Match = match;
        SwappedPiece1 = swappedPiece1;
        SwappedPiece2 = swappedPiece2;
    }
}