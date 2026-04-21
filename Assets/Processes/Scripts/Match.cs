#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;

    public class Match
    {
        public List<Candy> MatchingCandy = new();

        public void Add(Candy candy)
        {
            if (candy.CurrentMatch != null)
                return;

            MatchingCandy.Add(candy);
            candy.CurrentMatch = this;
        }
    }
}