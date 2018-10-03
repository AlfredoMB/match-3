using System.Collections.Generic;

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
