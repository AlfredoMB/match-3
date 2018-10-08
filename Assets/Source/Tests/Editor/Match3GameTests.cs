using NUnit.Framework;

public class Match3GameTests : BoardTests
{/*
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
    }*/
}
