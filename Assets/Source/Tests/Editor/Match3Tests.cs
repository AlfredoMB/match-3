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
            _board.Fill(new BoardPiece(int.MinValue)); // TODO: uncouple this invalid value from int
            // TODO: check sizes to make sure it builds what we set
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
            // TODO: change this to  2 random compares
            // trying to force random test failures
            for (int i = 0; i < 100; i++)
            {
                _board.RandomFillUp(i, 0, 1, 2);
                CollectionAssert.IsEmpty(_board.GetMatchesFromMovedPieces());
                _board.ConfirmMovedPieces();
            }
        }

        [Test]
        public void ShuffleBoard()
        {
            // TODO: change this to  2 random compares
            // trying to force random test failures
            for (int i = 0; i < 100; i++)
            {
                _board.RandomFillUp(i, 0, 1, 2);
                var serializedBoard = _board.ToString();

                _board.ShufflePieces(i + 1);
                StringAssert.AreNotEqualIgnoringCase(serializedBoard, _board.ToString());
            }
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
            _board.RemovePiecesFromMatches(matches);

            foreach (var match in matches)
            {
                foreach (var piece in match)
                {
                    Assert.IsTrue(piece.IsRemoved);
                }
            }
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

                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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
                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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
                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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

                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    Assert.That(match.Contains(matchPiece) || match.Contains(matchPiece2));
                }
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

                Assert.IsTrue(_board.IsReadyToSwap);
                _board.SwapCandidates();

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

                Assert.IsFalse(_board.IsReadyToSwap);
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
                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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
                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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
                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    CollectionAssert.Contains(match, matchPiece);
                }
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

                foreach (var match in matches)
                {
                    Assert.AreEqual(3, match.Count);
                    Assert.That(match.Contains(matchPiece) || match.Contains(matchPiece2));
                }
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

                Assert.IsTrue(_board.IsReadyToSwap);
                _board.SwapCandidates();

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

                Assert.IsFalse(_board.IsReadyToSwap);
            }
        }

        public class PossibleMatchesTests : BoardTests
        {
            [Test]
            public void DetectPossibleMatches()
            {
                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 1);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);

                Assert.IsTrue(_board.AreThereAnyPossibleMatchesLeft());
            }

            [Test]
            public void FailToDetectPossibleMatches()
            {
                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 1, 1);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 3, 0);

                Assert.IsFalse(_board.AreThereAnyPossibleMatchesLeft());
            }

            [Test]
            public void ListPossibleMatches()
            {
                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 3, 0);

                var possibleMatches = _board.GetAllPossibleMatchesLeft();
                CollectionAssert.IsNotEmpty(possibleMatches);
                Assert.AreEqual(1, possibleMatches.Count);
                foreach(var possibleMatch in possibleMatches)
                {
                    Assert.AreEqual(matchPiece.Type, possibleMatch.Type);
                }
            }

            [Test]
            public void FailToListPossibleMatches()
            {
                int targetX = 0;

                BoardPiece matchPiece = new BoardPiece(0);
                _board.SetPieceAt(matchPiece, targetX, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 2, 0);
                _board.SetPieceAt(new BoardPiece(matchPiece), targetX + 4, 0);
                
                CollectionAssert.IsEmpty(_board.GetAllPossibleMatchesLeft());
            }
        }
    }

    public class Match3GameTests : BoardTests
    {
        [Test]
        public void SuccessfulSwapFlowTest()
        {
            int targetX = 0;

            BoardPiece matchPiece = new BoardPiece(0);
            BoardPiece swapPiece = new BoardPiece(matchPiece);
            BoardPiece thirdPiece = new BoardPiece(matchPiece);
            _board.SetPieceAt(matchPiece, targetX, 0);
            _board.SetPieceAt(swapPiece, targetX + 1, 1);
            _board.SetPieceAt(thirdPiece, targetX + 2, 0);

            BoardPiece pieceAboveMatchPiece = new BoardPiece(2);
            BoardPiece pieceBelowSwapPiece = new BoardPiece(3);
            BoardPiece pieceAboveThirdPiece = new BoardPiece(4);
            _board.SetPieceAt(pieceAboveMatchPiece, targetX, 1);
            _board.SetPieceAt(pieceBelowSwapPiece, targetX + 1, 0);
            _board.SetPieceAt(pieceAboveThirdPiece, targetX + 2, 1);

            var game = new Match3Game(_board);

            // select a piece
            _board.SelectPieceAt(targetX + 1, 0);
            game.Process();

            // select a second piece
            _board.SelectPieceAt(targetX + 1, 1);
            game.Process();

            // assert that they swapped
            Assert.IsTrue(_board.IsSwapping);
            game.Process();

            // assert that there were matches
            Assert.IsTrue(_board.ThereAreMatches);
            game.Process();

            // assert that pieces were removed
            Assert.IsTrue(_board.ThereAreRemovedPieces);
            game.Process();

            // assert that pieces moved down
            Assert.IsTrue(_board.ThereAreMovedPieces);
            game.Process();

            // assert that there are no matches
            Assert.IsFalse(_board.ThereAreMatches);
            game.Process();

            Assert.IsFalse(_board.IsSwapping);
            Assert.IsFalse(_board.ThereAreMatches);
            Assert.IsFalse(_board.ThereAreRemovedPieces);
            Assert.IsFalse(_board.ThereAreMovedPieces);

            // assert that the swap effects are over
            Assert.IsTrue(_board.IsReadyForInput);
        }

        [Test]
        public void SuccessfulSwapWithDoubleMatchFlowTest()
        {
            int targetX = 0;

            BoardPiece matchPiece = new BoardPiece(0);
            BoardPiece swapPiece = new BoardPiece(matchPiece);
            BoardPiece thirdPiece = new BoardPiece(matchPiece);
            _board.SetPieceAt(matchPiece, targetX, 0);
            _board.SetPieceAt(swapPiece, targetX + 1, 1);
            _board.SetPieceAt(thirdPiece, targetX + 2, 0);

            BoardPiece pieceAboveMatchPiece = new BoardPiece(2);
            BoardPiece pieceBelowSwapPiece = new BoardPiece(pieceAboveMatchPiece);
            BoardPiece pieceAboveThirdPiece = new BoardPiece(pieceAboveMatchPiece);
            _board.SetPieceAt(pieceAboveMatchPiece, targetX, 1);
            _board.SetPieceAt(pieceBelowSwapPiece, targetX + 1, 0);
            _board.SetPieceAt(pieceAboveThirdPiece, targetX + 2, 1);

            var game = new Match3Game(_board);

            // select a piece
            _board.SelectPieceAt(targetX + 1, 0);
            game.Process();

            // select a second piece
            _board.SelectPieceAt(targetX + 1, 1);
            game.Process();

            // assert that they swapped
            Assert.IsTrue(_board.IsSwapping);
            game.Process();

            // assert that there were matches
            Assert.IsTrue(_board.ThereAreMatches);
            game.Process();

            // assert that pieces were removed
            Assert.IsTrue(_board.ThereAreRemovedPieces);
            game.Process();

            // assert that pieces moved down
            Assert.IsTrue(_board.ThereAreMovedPieces);
            game.Process();

            // assert that there are no matches
            Assert.IsFalse(_board.ThereAreMatches);
            game.Process();

            Assert.IsFalse(_board.IsSwapping);
            Assert.IsFalse(_board.ThereAreMatches);
            Assert.IsFalse(_board.ThereAreRemovedPieces);
            Assert.IsFalse(_board.ThereAreMovedPieces);

            // assert that the swap effects are over
            Assert.IsTrue(_board.IsReadyForInput);
        }

        [Test]
        public void SuccessfulSwapWithMatchesOnFallingPiecesFlowTest()
        {
            int targetX = 0;

            BoardPiece swapPiece = new BoardPiece(0);
            BoardPiece secondPiece = new BoardPiece(swapPiece);
            BoardPiece thirdPiece = new BoardPiece(swapPiece);
            _board.SetPieceAt(swapPiece, targetX, 1);
            _board.SetPieceAt(secondPiece, targetX + 1, 0);
            _board.SetPieceAt(thirdPiece, targetX + 2, 0);

            BoardPiece pieceBelowSwapPiece = new BoardPiece(1);
            BoardPiece pieceAboveSecondPiece = new BoardPiece(2);
            BoardPiece pieceAboveThirdPiece = new BoardPiece(pieceAboveSecondPiece);
            BoardPiece fourthPiece = new BoardPiece(pieceAboveSecondPiece);
            _board.SetPieceAt(pieceBelowSwapPiece, targetX, 0);
            _board.SetPieceAt(pieceAboveSecondPiece, targetX + 1, 1);
            _board.SetPieceAt(pieceAboveThirdPiece, targetX + 2, 1);
            _board.SetPieceAt(fourthPiece, targetX + 3, 0);

            var game = new Match3Game(_board);

            // select a piece
            _board.SelectPieceAt(targetX, 0);
            game.Process();

            // select a second piece
            _board.SelectPieceAt(targetX, 1);
            game.Process();

            // assert that they swapped
            Assert.IsTrue(_board.IsSwapping);
            game.Process();

            // assert that there were matches
            Assert.IsTrue(_board.ThereAreMatches);
            Assert.AreEqual(1, _board.GetMatchesFromMovedPieces().Count);
            game.Process();

            // assert that pieces were removed
            Assert.IsTrue(_board.ThereAreRemovedPieces);
            game.Process();

            // assert that pieces moved down
            Assert.IsTrue(_board.ThereAreMovedPieces);
            game.Process();

            // assert that there are more matches
            Assert.IsTrue(_board.ThereAreMatches);
            Assert.AreEqual(1, _board.GetMatchesFromMovedPieces().Count);
            game.Process();

            // assert that pieces were removed
            Assert.IsTrue(_board.ThereAreRemovedPieces);
            game.Process();

            // assert that pieces moved down
            Assert.IsTrue(_board.ThereAreMovedPieces);
            game.Process();

            // assert that there are no matches
            Assert.IsFalse(_board.ThereAreMatches);
            game.Process();

            Assert.IsFalse(_board.IsSwapping);
            Assert.IsFalse(_board.ThereAreMatches);
            Assert.IsFalse(_board.ThereAreRemovedPieces);
            Assert.IsFalse(_board.ThereAreMovedPieces);

            // assert that the swap effects are over
            Assert.IsTrue(_board.IsReadyForInput);
        }

        [Test]
        public void FailedSwapFlowTest()
        {
            int targetX = 0;

            BoardPiece matchPiece = new BoardPiece(0);
            BoardPiece swapPiece = new BoardPiece(matchPiece);
            BoardPiece thirdPiece = new BoardPiece(matchPiece);
            _board.SetPieceAt(matchPiece, targetX, 0);
            _board.SetPieceAt(swapPiece, targetX + 1, 1);
            _board.SetPieceAt(thirdPiece, targetX + 3, 0);

            BoardPiece pieceAboveMatchPiece = new BoardPiece(2);
            BoardPiece pieceBelowSwapPiece = new BoardPiece(3);
            BoardPiece pieceAboveThirdPiece = new BoardPiece(4);
            _board.SetPieceAt(pieceAboveMatchPiece, targetX, 1);
            _board.SetPieceAt(pieceBelowSwapPiece, targetX + 1, 0);
            _board.SetPieceAt(pieceAboveThirdPiece, targetX + 2, 1);

            var game = new Match3Game(_board);

            // select a piece
            _board.SelectPieceAt(targetX + 1, 0);
            game.Process();

            // select a second piece
            _board.SelectPieceAt(targetX + 1, 1);
            game.Process();

            // assert that they swapped
            Assert.IsTrue(_board.IsSwapping);
            game.Process();

            // assert that there were matches
            Assert.IsFalse(_board.ThereAreMatches);
            game.Process();

            // assert that moved pieces got back to their places without moving anyone else
            Assert.IsFalse(_board.ThereAreMovedPieces);

            // assert that the swap effects are over
            Assert.IsTrue(_board.IsReadyForInput);
        }

        [Test]
        public void FailedToSwapFlowTest()
        {
            int targetX = 0;

            BoardPiece matchPiece = new BoardPiece(0);
            BoardPiece swapPiece = new BoardPiece(matchPiece);
            BoardPiece thirdPiece = new BoardPiece(matchPiece);
            _board.SetPieceAt(matchPiece, targetX, 0);
            _board.SetPieceAt(swapPiece, targetX + 1, 1);
            _board.SetPieceAt(thirdPiece, targetX + 3, 0);

            BoardPiece pieceAboveMatchPiece = new BoardPiece(2);
            BoardPiece pieceBelowSwapPiece = new BoardPiece(3);
            BoardPiece pieceAboveThirdPiece = new BoardPiece(4);
            _board.SetPieceAt(pieceAboveMatchPiece, targetX, 1);
            _board.SetPieceAt(pieceBelowSwapPiece, targetX + 1, 0);
            _board.SetPieceAt(pieceAboveThirdPiece, targetX + 2, 1);

            var game = new Match3Game(_board);

            // select a piece
            _board.SelectPieceAt(targetX + 1, 0);
            game.Process();

            // select a second piece
            _board.SelectPieceAt(targetX + 1, 2);
            game.Process();

            // assert that they swapped
            Assert.IsFalse(_board.IsSwapping);
            game.Process();
            
            // assert that moved pieces got back to their places without moving anyone else
            Assert.IsFalse(_board.ThereAreMovedPieces);

            // assert that the swap effects are over
            Assert.IsTrue(_board.IsReadyForInput);
        }
    }
}

public class Match3Game
{
    private Board _board;
    
    private enum EState
    {
        WaitingToSwap,
        Swapping,
        CheckingMatches,
        SwappingBack,
        RemovingMatchPieces,
        MovingDownPieces,
    }

    private EState _currentState;
    private Matches _currentMatches;

    public Match3Game(Board board)
    {
        _board = board;
        _currentState = EState.WaitingToSwap;
    }

    public void Process()
    {
        switch (_currentState)
        {
            case EState.WaitingToSwap:
                if (!_board.IsReadyToSwap)
                {
                    return;
                }

                _board.SwapCandidates();
                _currentState = EState.CheckingMatches;
                break;

            case EState.CheckingMatches:
                _currentMatches = _board.GetMatchesFromMovedPieces();
                if (_currentMatches.Count <= 0)
                {
                    if (_board.IsSwapping)
                    {
                        _currentState = EState.SwappingBack;
                    }
                    else
                    {
                        _board.ConfirmMovedPieces();
                        _currentState = EState.WaitingToSwap;
                    }
                }
                else
                {
                    if (_board.IsSwapping)
                    {
                        _board.ConfirmSwappedPieces();
                    }
                    _currentState = EState.RemovingMatchPieces;
                }
                break;

            case EState.SwappingBack:
                _board.SwapCandidates();
                _board.ConfirmMovedPieces();
                _currentState = EState.WaitingToSwap;
                break;

            case EState.RemovingMatchPieces:
                _board.RemovePiecesFromMatches(_currentMatches);
                _currentState = EState.MovingDownPieces;
                break;

            case EState.MovingDownPieces:
                _board.MovePiecesDown();
                _currentState = EState.CheckingMatches;
                break;
        }
    }
}

public class GameTimer
{
    public float RemainingTime { get; private set; }
    public bool TimeIsUp { get; private set; }

    public void SetTime(float time)
    {
        RemainingTime = time;
    }

    public void UpdateTimePassed(float timePassed)
    {
        RemainingTime -= timePassed;
        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            TimeIsUp = true;
        }
    }
}

public class Match : HashSet<BoardPiece>
{
    public int Type { get; private set; }

    public Match(int type) : base()
    {
        Type = type;
    }

    public override string ToString()
    {
        string items = string.Empty;
        bool first = true;
        foreach(var item in this)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                items += ", ";
            }
            items += item;
        }

        return string.Format("[Type={0}, Items={1}]",
            Type, items);
    }

    public override bool Equals(object obj)
    {
        var other = obj as Match;
        if (other == null)
        {
            return false;
        }

        return SetEquals(other);
    }

    public override int GetHashCode()
    {
        // TODO: evaluate this
        unchecked
        {
            int hash = 17;
            foreach (var item in this)
            {
                hash += item.GetHashCode();
            }
            return hash;
        }
    }
}

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

    public bool IsReadyToSwap { get { return (_selectedPiece != null && _swapCandidate != null); } }
    public bool IsSwapping { get; private set; }
    public bool ThereAreMatches { get { return _matches.Count > 0; } }
    public bool ThereAreRemovedPieces { get { return _removedList.Count > 0; } }
    public bool ThereAreMovedPieces { get { return _movedList.Count > 0; } }
    public bool IsReadyForInput { get { return !IsSwapping && !ThereAreRemovedPieces && !ThereAreMovedPieces && !ThereAreMatches; } }

    public Board(int width, int height, int minMatchSize)
    {
        _board = new BoardPiece[width, height];
        _minMatchSize = minMatchSize;
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
            
            if (horizontalMatch.Count >= _minMatchSize && verticalMatch.Count >= _minMatchSize)
            {
                horizontalMatch.UnionWith(verticalMatch);
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

    public void SwapCandidates()
    {
        Assert.IsNotNull(_selectedPiece);
        Assert.IsNotNull(_swapCandidate);

        Swap(_selectedPiece, _swapCandidate);
        IsSwapping = !IsSwapping;
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

public class Matches : HashSet<Match> { }

public class PossibleMatches : HashSet<PossibleMatch> { }

public class PossibleMatch
{
    public readonly Match Match;
    public readonly SwappedPieces SwappedPieces = new SwappedPieces();

    public int Type { get { return Match.Type; } }

    public PossibleMatch(Match match, BoardPiece swappedPiece1, BoardPiece swappedPiece2)
    {
        Match = match;
        SwappedPieces.Add(swappedPiece1);
        SwappedPieces.Add(swappedPiece2);
    }

    public override bool Equals(object obj)
    {
        var other = obj as PossibleMatch;
        if (other == null)
        {
            return base.Equals(obj);
        }

        return Match.SetEquals(other.Match) && SwappedPieces.SetEquals(other.SwappedPieces);
    }

    public override int GetHashCode()
    {
        // TODO: evaluate this
        unchecked
        {
            int hash = 17;
            hash = hash * 23 ^ Match.GetHashCode();
            hash = hash * 23 ^ SwappedPieces.GetHashCode();
            return hash;
        }
    }
}

public class SwappedPieces : HashSet<BoardPiece> { }