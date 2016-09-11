using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class Form1 : Form
    {
        public Board myBoard = new Board();
        private Graphics g;

        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            myBoard.myDraw(g);

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (myBoard.ProceedClick(e.X, e.Y)) //Отправляем инфу о клике обработчику клика
                myBoard.myDraw(g); //если был ход, перерисуем
        }
    }
}
