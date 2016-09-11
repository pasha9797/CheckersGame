using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame
{
    public enum Team { Black, White };
    public enum Direction {UpRight, DownRight, DownLeft, UpLeft };

    public class Checker
    {     
        private Team team;
        private bool active;
        private MyPoint position;
        private MyPoint oldPosition;
        private bool damka = false;

        public bool Damka
        {
            get
            {
                return damka;
            }
        }
        public MyPoint Position
        {
            get
            {
                return position;
            }
        }
        public Team CheckerTeam
        {
            get
            {
                return team;
            }
        }
        public bool Active
        {
            set
            {
                active = value;
                if (!active)
                {
                    oldPosition = position;
                    position = new MyPoint(0, 0);
                }
                else
                {
                    position = oldPosition;
                }
            }
            get
            {
                return active;
            }
        }


        public bool MoveChecker(int dx, int dy)
        {
            if (position.X + dx > 0 && position.X + dx <= 8 && position.Y + dy > 0 && position.Y + dy <= 8)
            {
                position.X += dx;
                position.Y += dy;
                if ((CheckerTeam == Team.Black && Position.Y == 8) || (CheckerTeam == Team.White && Position.Y == 1))
                    damka = true;
                return true;
            }
            else
                return false;
        }

        public Checker(Team team, int x, int y)
        {
            this.team = team;
            if (x > 0 && x <= 8 && y > 0 && y <= 8)
            {
                position = new MyPoint(x, y);
                active = true;
            }
            else
            {
                position = new MyPoint(0, 0);
                active = false;
            }
            oldPosition = position;
        }
    }
}
