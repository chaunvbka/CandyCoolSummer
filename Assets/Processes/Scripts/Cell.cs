#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    public class Cell
    {
        public Candy ContainingCandy;
        public Candy IncomingCandy;
        public Obstacle Obstacle;

        public bool Locked = false;

        public bool BlockFall => Locked || (ContainingCandy != null && !ContainingCandy.CanMove);
        public bool CanFall => !Locked && ContainingCandy != null && ContainingCandy.CanMove;

        public bool CanMatch()
        {
            return ContainingCandy != null;
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

