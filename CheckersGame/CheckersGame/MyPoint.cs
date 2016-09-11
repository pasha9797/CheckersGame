using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame
{
    public class MyPoint
    {
        private int x, y;
        public MyPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int X
        {
            get { return x; }
            set
            {
                if (x > 0)
                    x = value;
            }
        }
        public int Y
        {
            get { return y; }
            set
            {
                if (y > 0)
                    y = value;
            }
        }
    }
}
