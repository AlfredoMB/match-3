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
