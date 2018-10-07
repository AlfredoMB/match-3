using NUnit.Framework;

public class BoardInitializationTests : BoardTests
{
    [Test]
    public void FillBoard()
    {
        Assert.NotNull(_board.GetPieceAt(0, 0));
    }
    /*
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
    }*/
}
