﻿using NUnit.Framework;

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
