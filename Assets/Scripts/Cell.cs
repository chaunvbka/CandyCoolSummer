#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    public class Cell
    {
        public Candy ContainingCandy;
        public Candy IncomingCandy;
        public Obstacle Obstacle;

        public bool Locked = false;

        public bool BlockFall => Locked || (ContainingCandy != null && !ContainingCandy.CanMove) || (ContainingCandy != null && ContainingCandy.CurrentMatch != null);
        public bool CanFall => !Locked && ContainingCandy != null && ContainingCandy.CanMove && ContainingCandy.CurrentMatch == null;
        public bool CanBeMoved => !Locked && ContainingCandy != null && ContainingCandy.CanMove && ContainingCandy.CurrentMatch == null;

        public bool CanMatch()
        {
            return ContainingCandy != null && ContainingCandy.CurrentMatch == null;
        }

        public bool CanDelete()
        {
            return !Locked;
        }

        public bool IsEmpty()
        {
            return ContainingCandy == null && IncomingCandy == null;
        }
    }
}

