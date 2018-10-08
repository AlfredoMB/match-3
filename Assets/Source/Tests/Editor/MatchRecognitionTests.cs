using NUnit.Framework;

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
    /*
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
        }*/
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
    /*
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
        }*/
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
        foreach (var possibleMatch in possibleMatches)
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
