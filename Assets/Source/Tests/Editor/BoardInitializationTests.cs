using NUnit.Framework;

public class BoardInitializationTests : BoardTests
{
    [Test]
    public void FillBoard()
    {
        Assert.NotNull(_board.GetPieceAt(0, 0));
    }
}
