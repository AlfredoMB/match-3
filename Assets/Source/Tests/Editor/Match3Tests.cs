using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Match3Tests
{
    public class BoardTests
    {
        [Test]
        public void FillBoard()
        {
            int width = 8;
            int height = 8;
            Board board = new Board(width, height);
            board.Fill();

            Assert.NotNull(board.GetPieceAt(0, 0));
        }
    }

    public class BoardPieceRemovalTests
    {
        [Test]
        public void BoardPiecesFallWhenPieceIsRemoved()
        {
            int width = 8;
            int height = 8;
            Board board = new Board(width, height);
            board.Fill();

            int targetPieceY = 4;

            BoardPiece upperPiece = board.GetPieceAt(0, targetPieceY + 1);
            board.RemovePieceAt(0, targetPieceY);
            board.MovePiecesDown();

            Assert.AreSame(upperPiece, board.GetPieceAt(0, targetPieceY));
        }

        [Test]
        public void BoardPiecesFallWhen3VerticalPiecesAreRemoved()
        {
            int width = 8;
            int height = 8;
            Board board = new Board(width, height);
            board.Fill();

            int targetPieceY = 0;

            BoardPiece upperPiece = board.GetPieceAt(0, targetPieceY + 3);
            board.RemovePieceAt(0, targetPieceY + 2);
            board.RemovePieceAt(0, targetPieceY + 1);
            board.RemovePieceAt(0, targetPieceY);
            board.MovePiecesDown();

            Assert.AreSame(upperPiece, board.GetPieceAt(0, targetPieceY));
        }

        [Test]
        public void NewBoardPieceIsCreatedAtTheTopWhenPieceIsRemoved()
        {
            int width = 8;
            int height = 8;
            Board board = new Board(width, height);
            board.Fill();

            int topPieceY = height - 1;
            int targetPieceY = 0;

            BoardPiece topPiece = board.GetPieceAt(0, topPieceY);
            board.RemovePieceAt(0, targetPieceY);
            board.MovePiecesDown();

            Assert.AreNotSame(topPiece, board.GetPieceAt(0, topPieceY));
            Assert.AreSame(topPiece, board.GetPieceAt(0, topPieceY - 1));
        }

        [Test]
        public void NewBoardPiecesAreCreatedAtTheTopWhen3VerticalPiecesAreRemoved()
        {
            int width = 8;
            int height = 8;
            Board board = new Board(width, height);
            board.Fill();

            int topPieceY = height - 1;
            int targetPieceY = 0;

            BoardPiece topPiece = board.GetPieceAt(0, topPieceY);
            board.RemovePieceAt(0, targetPieceY + 2);
            board.RemovePieceAt(0, targetPieceY + 1);
            board.RemovePieceAt(0, targetPieceY);
            board.MovePiecesDown();

            Assert.AreNotSame(topPiece, board.GetPieceAt(0, topPieceY));
            Assert.AreNotSame(topPiece, board.GetPieceAt(0, topPieceY - 1));
            Assert.AreNotSame(topPiece, board.GetPieceAt(0, topPieceY - 2));
            Assert.AreSame(topPiece, board.GetPieceAt(0, topPieceY - 3));
        }
    }

    public class MatchRecognitionTests
    {
        public class HorizontalRecognitionTests
        {
            [Test]
            public void RecognizeHorizontal3PieceStartingOnLeftAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, targetX, 0);
                board.SetPieceAt(matchPiece, targetX + 1, 0);
                board.SetPieceAt(matchPiece, targetX + 2, 0);

                board.SetMovedPieceAt(targetX, 0);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void RecognizeHorizontal3PieceStartingOnMiddleAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, targetX - 1, 0);
                board.SetPieceAt(matchPiece, targetX, 0);
                board.SetPieceAt(matchPiece, targetX + 1, 0);

                board.SetMovedPieceAt(targetX, 0);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void RecognizeHorizontal3PieceStartingOnRightAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetX = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, targetX - 2, 0);
                board.SetPieceAt(matchPiece, targetX - 1, 0);
                board.SetPieceAt(matchPiece, targetX, 0);

                board.SetMovedPieceAt(targetX, 0);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void DeclineHorizontal2PieceStartingOnLeftAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, targetX, 0);
                board.SetPieceAt(matchPiece, targetX + 1, 0);

                board.SetMovedPieceAt(targetX, 0);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void DeclineHorizontal2PieceStartingOnRightAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetX = 1;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, targetX - 1, 0);
                board.SetPieceAt(matchPiece, targetX, 0);

                board.SetMovedPieceAt(targetX, 0);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }
        }
        
        public class VerticalRecognitionTests
        {
            [Test]
            public void RecognizeVertical3PieceStartingOnLeftAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, 0, targetY);
                board.SetPieceAt(matchPiece, 0, targetY + 1);
                board.SetPieceAt(matchPiece, 0, targetY + 2);

                board.SetMovedPieceAt(0, targetY);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void RecognizeVertical3PieceStartingOnMiddleAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();

                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, 0, targetY - 1);
                board.SetPieceAt(matchPiece, 0, targetY);
                board.SetPieceAt(matchPiece, 0, targetY + 1);

                board.SetMovedPieceAt(0, targetY);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void RecognizeVertical3PieceStartingOnRightAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();
                
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, 0, targetY - 2);
                board.SetPieceAt(matchPiece, 0, targetY - 1);
                board.SetPieceAt(matchPiece, 0, targetY);

                board.SetMovedPieceAt(0, targetY);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsNotEmpty(matches);
            }

            [Test]
            public void DeclineVertical2PieceStartingOnLeftAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();                

                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, 0, targetY);
                board.SetPieceAt(matchPiece, 0, targetY + 1);

                board.SetMovedPieceAt(0, targetY);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }

            [Test]
            public void DeclineVertical2PieceStartingOnRightAsMatch()
            {
                int width = 8;
                int height = 8;
                Board board = new Board(width, height);
                board.Fill();
                
                int targetY = 2;

                BoardPiece matchPiece = new BoardPiece();
                board.SetPieceAt(matchPiece, 0, targetY - 1);
                board.SetPieceAt(matchPiece, 0, targetY);

                board.SetMovedPieceAt(0, targetY);

                var matches = board.GetMatchesFromMovedPieces();
                CollectionAssert.IsEmpty(matches);
            }
        }
    }
}

public class Match : List<BoardPiece> { }

public class BoardPiece
{
    public BoardPiece()
    {
    }

    public BoardPiece(BoardPiece boardPiece)
    {
    }
}

public class Board
{
    private BoardPiece[,] _board;
    private List<BoardPosition> _movedList = new List<BoardPosition>();
    private List<BoardPosition> _removedList = new List<BoardPosition>();
    private List<Match> _matchList = new List<Match>();

    public Board(int width, int height)
    {
        _board = new BoardPiece[width, height];
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
        _board[x, y] = null;
        _removedList.Add(new BoardPosition() { X = x, Y = y });
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
        _movedList.Add(new BoardPosition() { X = x, Y = y });
    }

    public void SetPieceAt(BoardPiece piece, int x, int y)
    {
        _board[x, y] = piece;
    }

    private BoardPiece GetNewBoardPiece()
    {
        return new BoardPiece();
    }

    public List<Match> GetMatchesFromMovedPieces()
    {
        _matchList.Clear();
        foreach (var moved in _movedList)
        {
            var targetPieceType = _board[moved.X, moved.Y];
            var horizontalMatch = new Match();
            var verticalMatch = new Match();

            // check left
            for (int i = moved.X - 1; i >= 0; i--)
            {
                if (_board[i, moved.Y] != targetPieceType)
                {
                    break;
                }
                horizontalMatch.Add(_board[i, moved.Y]);
            }

            // check right
            for (int i = moved.X + 1; i < _board.GetLength(0); i++)
            {
                if (_board[i, moved.Y] != targetPieceType)
                {
                    break;
                }
                horizontalMatch.Add(_board[i, moved.Y]);
            }

            // check up
            for (int j = moved.Y + 1; j < _board.GetLength(1); j++)
            {
                if (_board[moved.X, j] != targetPieceType)
                {
                    break;
                }
                verticalMatch.Add(_board[moved.X, j]);
            }

            // check down
            for (int j = moved.Y - 1; j >= 0; j--)
            {
                if (_board[moved.X, j] != targetPieceType)
                {
                    break;
                }
                verticalMatch.Add(_board[moved.X, j]);
            }

            if (horizontalMatch.Count >= 2 && verticalMatch.Count >= 2)
            {
                horizontalMatch.AddRange(verticalMatch);
                _matchList.Add(horizontalMatch);
            }
            else if (horizontalMatch.Count >= 2)
            {
                _matchList.Add(horizontalMatch);
            }
            else if (verticalMatch.Count >= 2)
            {
                _matchList.Add(verticalMatch);
            }
        }

        return _matchList;
    }
}

public struct BoardPosition
{
    public int X, Y;
}