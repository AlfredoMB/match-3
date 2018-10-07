using System;
using System.Collections.Generic;
using System.Text;

public class Matches : HashSet<Match>
{
    public int TotalCount()
    {
        int count = 0;
        foreach(var item in this)
        {
            count += item.Count;
        }
        return count;
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach(var match in this)
        {
            sb.Append(match);
        }
        return string.Format("[Matches={0}]", sb.ToString());
    }
}
