using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public int Width = 8;
    public int Height = 8;
    public int MinMatchSize = 3;

    public PieceView[] PiecePrefabs;
    public BoardReferenceView BoardReferenceView;

    public Transform Transform;

    private Board _board;

    private IEnumerator Start()
    {
        _board = new Board(Width, Height, MinMatchSize);
        _board.RandomFillUp(Random.Range(int.MinValue, int.MaxValue), 0, 1, 2, 3, 4);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var boardPiece = _board.GetPieceAt(x, y);
                var piece = Instantiate(PiecePrefabs[boardPiece.Type], Transform);
                piece.SetReference(BoardReferenceView.GetReference(x, y, Width));
                yield return null;
            }
        }
    }
}