using NUnit.Framework;

public class BoardTests
{
    protected const int _width = 8;
    protected const int _height = 8;
    protected const int _minMatchSize = 3;
    protected Board _board;

    [SetUp]
    public void SetUp()
    {
        _board = new Board(_width, _height, _minMatchSize, new[] { 0, 1, 2 }, 0);
        _board.Fill(new BoardPiece(int.MinValue)); // TODO: uncouple this invalid value from int
                                                   // TODO: check sizes to make sure it builds what we set
    }
}
