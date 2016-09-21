using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CheckersGame
{
    public class Board
    {
        private Checker[] list = new Checker[24];
        private Checker active = null;
        public Team whoSteps = Team.White;

        public Board()//конструктор
        {
            int pointer = 0;
            for (int i = 1; i <= 8; i++)
            {
                if(i % 2 == 1)
                {
                    list[pointer++] = new CheckersGame.Checker(Team.Black, i, 1);
                    list[pointer++] = new CheckersGame.Checker(Team.Black, i, 3);
                    list[pointer++] = new CheckersGame.Checker(Team.White, i, 7);
                }
                else
                {
                    list[pointer++] = new CheckersGame.Checker(Team.White, i, 8);
                    list[pointer++] = new CheckersGame.Checker(Team.White, i, 6);
                    list[pointer++] = new CheckersGame.Checker(Team.Black, i, 2);
                }
            }
        }

        public Checker Active
        {
            get
            {
                return active;
            }
        }

        public Checker FindCheckerByCoords(int x, int y)//найти по координатам доски
        {
            Checker found=null;
            int i = 0;
            while(i<24 && found==null)
            {
                if (list[i].Position.X == x && list[i].Position.Y == y)
                    found = list[i];
                else
                    i++;
            }
            return found;
        }

        const int step = 50;
        const int size = 38;

        private float toPixels(int coord)//координату доски в пиксельную
        {
            return coord * step - 0.1F * step;
        }
        private int toCoord(int pixel)//пиксельную координату в координату доски
        {
            return (int)Math.Floor((pixel+0.1F*step)/step);
        }

        public Checker FindByPixels(int x, int y)//по пиксельным координатам найти шашку
        {
            Checker found = null;
            int i = 0;
            while(i<24 && found==null)
            {
                if (x > toPixels(list[i].Position.X) && x < toPixels(list[i].Position.X + 1) &&
                   y > toPixels(list[i].Position.Y) && y < toPixels(list[i].Position.Y + 1))
                    found = list[i];
                i++;
            }
            return found;
        }
        public void Activate(Checker checker)//выделить шашку
        {
            active = checker;
        }
        public bool ProceedClick(int x, int y)//обработчик клика
        {
            bool changed = false;
            Checker found = FindByPixels(x, y);//ищем шашку

            if (active == null && found != null)//выбрал шашку для хода
            {
                if (found.CheckerTeam != whoSteps) return false;
                Activate(found);
                changed = true;
            }
            else
            if (found != null && active == found)//снять выделение
            {
                Activate(null);
                changed = true;
            }
            else
            if (found == null && active != null)//если хочет сделать ход
            {
                if (ProceedStep(x, y))//обработчик хода
                    changed = true;
            }
            return changed;
        }

        public bool ProceedStep(int x, int y)
        {
            int cX = toCoord(x);
            int cY = toCoord(y);
            bool complete = false;
            bool beated = false;
            Checker neigbour;
            Team nTeam;

            if (cX < 1 || cX > 8 || cY < 1 || cY > 8) return false;

            if (active.Damka)//если дамка, то отдельный обработчик
                complete = ProceedDamka(cX, cY, ref beated);

            else//если просто шашка
            {
                int dX = cX - active.Position.X;
                int dY = cY - active.Position.Y;

                if (Math.Abs(dX) == 1 && Math.Abs(dY) == 1)//если лишь один шаг
                {
                    if (dY == 1 && active.CheckerTeam == Team.White) return false;//белым нельзя вниз
                    if (dY == -1 && active.CheckerTeam == Team.Black) return false;//черным нельзя вверх
                    active.MoveChecker(dX, dY);
                    complete = true;
                }

                else
                if (Math.Abs(dX) == 2 && Math.Abs(dY) == 2)//если хотим бить
                {
                    neigbour = FindCheckerByCoords(active.Position.X + dX / 2, active.Position.Y + dY / 2); //ищем кого бить
                    if (neigbour != null)
                    {
                        nTeam = neigbour.CheckerTeam;
                        if (nTeam != active.CheckerTeam)
                        {
                            active.MoveChecker(dX, dY);//двигаем шашку
                            neigbour.Active = false; //убиваем противника
                            complete = true;
                            beated = true;
                        }
                    }
                }
            }

            if (complete)//очередь другого игрока
            {
                bool change = true;
                change = (!CheckPossibleBeat() || !beated);
                if (change)
                {
                    active = null;
                    if (whoSteps == Team.Black)
                        whoSteps = Team.White;
                    else
                        whoSteps = Team.Black;
                }
            }
            return complete;
        }

        private bool ProceedDamka(int cX, int cY, ref bool beated)
        {
            bool complete = false;
            int dX = cX - active.Position.X;
            int dY = cY - active.Position.Y;

            if (Math.Abs(dX) != Math.Abs(dY)) return false;//если ход не по диагонали
            if (cX == active.Position.X && cY == active.Position.Y) return true;//если дошли точки
            //ggg
            int sX = dX / Math.Abs(dX);
            int sY = dY / Math.Abs(dY);

            Checker neigbour = FindCheckerByCoords(active.Position.X + sX, active.Position.Y + sY); //ищем кого бить
            if(neigbour !=null)
            {
                if (neigbour.CheckerTeam == active.CheckerTeam) return false; //если на пути своя шашка
                else
                {
                    neigbour.Active = false;
                    beated = true;
                }
            }
            active.MoveChecker(sX, sY);
            complete = ProceedDamka(cX, cY, ref beated);
            if (!complete)
            {
                active.MoveChecker(-sX, -sY);
                if (neigbour != null) neigbour.Active = true;
            }
                return complete;
        }

        public void myDraw(Graphics g)
        {
            g.Clear(Color.White);
            Pen myPen = new Pen(Color.Black);
            SolidBrush myBrush = new SolidBrush(Color.LightGray);
            Font myFont = new Font("Tahoma", 12, FontStyle.Bold);

            for(int i=1;i<=8;i++)
            {
                int j = 2-i%2;
                while(j<=8)
                {
                    g.FillRectangle(myBrush, toPixels(i), toPixels(j), step, step);
                    j += 2;
                }
            }

            myBrush.Color = Color.Black;
            for (int i = 1; i <= 9; i++)//доска
            {
                //horisontal
                g.DrawLine(myPen, toPixels(1), toPixels(i), toPixels(9), toPixels(i));
                //vertical
                g.DrawLine(myPen, toPixels(i), toPixels(1), toPixels(i), toPixels(9));

                if (i != 9)
                {
                    g.DrawString(i.ToString(), myFont, myBrush, i * step + 0.2F * step, 0.4F * step);
                    g.DrawString(i.ToString(), myFont, myBrush, 0.4F * step, i * step + 0.2F * step);
                }
            }

            for (int i = 0; i < 24; i++)//шашки
            {
                if (list[i].Active == true)
                {
                    if (list[i].Damka)
                    {
                        myPen.Color = Color.Red;
                        myPen.Width = 2;
                    }
                    if (list[i] != active)
                    {
                        if (list[i].CheckerTeam == Team.Black)
                            myBrush.Color = Color.Black;
                        else
                            myBrush.Color = Color.White;
                    }
                    else
                        if (list[i].CheckerTeam == Team.Black)
                        myBrush.Color = Color.DarkRed;
                    else
                        myBrush.Color = Color.PaleVioletRed;
                    g.FillEllipse(myBrush, list[i].Position.X * step, list[i].Position.Y * step, size, size);
                    g.DrawEllipse(myPen, list[i].Position.X * step, list[i].Position.Y * step, size, size);
                    myPen.Color = Color.Black;
                    myPen.Width = 1;
                }
            }

            //чей ход
            myBrush.Color = Color.Black;
            g.DrawString("Ходит:", myFont, myBrush, 9.5F * step, 0.5F * step);
            if (whoSteps == Team.Black)
                myBrush.Color = Color.Black;
            else
                myBrush.Color = Color.White;
            g.FillEllipse(myBrush, 11*step, 0.4F*step, size, size);
            g.DrawEllipse(myPen, 11*step, 0.4F*step, size, size);
        }
        public bool CheckPossibleBeat()
        {
            bool okay = false;
            int i = 0;
            while (i < 4 && !okay)
            {
                int x = active.Position.X;
                int y = active.Position.Y;
                int dX = 0, dY = 0;
                while (x >= 1 && x <= 8 && y >= 1 && y <= 8 && !okay)
                {
                    switch (i)
                    {
                        case 0:
                            dX = 1;
                            dY = 1;
                            break;
                        case 1:
                            dX = 1;
                            dY = -1;
                            break;
                        case 2:
                            dX = -1;
                            dY = -1;
                            break;
                        case 3:
                            dX = -1;
                            dY = 1;
                            break;

                    }
                    x += dX;
                    y += dY;
                    if (x+dX < 1 || x+dX > 8 || y+dY < 1 || y+dY > 8) okay = false;
                    else
                    {
                        Checker whoToBeat = FindCheckerByCoords(x, y);
                        Checker whereToStop = FindCheckerByCoords(x + dX, y + dY);
                        if (whoToBeat != null && whereToStop == null && whoToBeat.CheckerTeam!=active.CheckerTeam) okay = true;
                        else if (whoToBeat != null && whereToStop != null) okay = false;
                    }
                }
                i++;
            }
            return okay;
        }
    }

}
