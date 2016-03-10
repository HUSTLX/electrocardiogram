using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MsPaint
{
    class draw
    {


        public Bitmap mybitmap;//用于双缓冲的位图，和画布等大

        Random rm = new Random();//随机数产生器


        public void DrawLineS(Color color, float Xmax, float Ymax, PictureBox picbox, Point[] ptlist)
        {
            mybitmap = new Bitmap(picbox.Width, picbox.Height);//设定位图大小
            Graphics doublebufferg = Graphics.FromImage(mybitmap);//从位图上获取“画布”
            doublebufferg.Clear(Color.White);//用背景色刷新

            //pictureBox1填充为白色，便于显示图像  500*300
            Rectangle rect = new Rectangle(0, 0, picbox.Width, picbox.Height);
            doublebufferg.FillRectangle(new SolidBrush(Color.White), rect);


            //画X和Y轴
            DrawXY(ref doublebufferg, picbox);
            //X轴上的刻度
            SetYAxis(ref doublebufferg, picbox, Ymax);
            //Y轴上的刻度
            SetXAxis(ref doublebufferg, picbox, Xmax);

            //要显示的实时曲线部分
            Point temp = new Point();
            for (int j = 0; j < picbox.Width / 5 - 1; j++)
            {
                temp = ptlist[j + 1];
                ptlist[j] = new Point(temp.X - 5, temp.Y);
            }

            Point lastpt = new Point();
            lastpt.X = picbox.Width;
            lastpt.Y = rm.Next(DateTime.Now.Millisecond) % picbox.Height;
            ptlist[picbox.Width / 5 - 1] = lastpt;
            doublebufferg.DrawLines(new Pen(color, 1), ptlist);


            //将缓冲中的位图绘制到我们的窗体上
            Graphics g1 = picbox.CreateGraphics();//创建 PictureBox窗体的画布

            g1.Clear(Color.White);
            g1.DrawImage(mybitmap, 0, 0);

        }


        //完成X轴和Y轴的基本部分
        public void DrawXY(ref Graphics g, PictureBox picbox)
        {
            Pen pen = new Pen(Color.Black, 2);//画笔
            SolidBrush sb = new SolidBrush(Color.Black);//话刷

            //X轴的箭头，实际上是绘制了一个三角形
            Point[] xpts = new Point[3] { 
                new Point(picbox.Width - 35, picbox.Height - 32),
                new Point(picbox.Width - 35, picbox.Height - 28), 
                new Point(picbox.Width - 30, picbox.Height - 30) 
                                        };

            g.DrawLine(pen, 30, picbox.Height - 30, picbox.Width - 30, picbox.Height - 30);
            g.DrawPolygon(pen, xpts);
            g.DrawString("X", new Font("宋体", 9), sb, picbox.Width - 25, picbox.Height - 35);



            //Y轴的箭头，实际上是绘制了一个三角形
            Point[] ypts = new Point[3] { 
                     new Point(28, 35), 
                     new Point(30, 30), 
                     new Point(32, 35) };

            g.DrawLine(pen, 30, picbox.Height - 30, 30, 30);
            g.DrawPolygon(pen, ypts);
            g.DrawString("Y", new Font("宋体", 9), sb, 15, 30);

        }


        //绘制Y轴上的刻度
        public void SetYAxis(ref Graphics g, PictureBox picbox, float YMAX)
        {
            Pen p1 = new Pen(Color.Goldenrod, 1);
            Pen p2 = new Pen(Color.Black, 2);
            SolidBrush sb = new SolidBrush(Color.Black);

            float ykedu = YMAX / 200;//给定的最大刻度与实际像素的比例关系

            //第一个刻度的两个端点
            float xl = 27, yl = picbox.Height - 30, xr = 33, yr = picbox.Height - 30;

            for (int j = 0; j < picbox.Height - 60; j += 10)
            {

                if (j % 50 == 0)//一个大的刻度，黑色，每隔50像素一个
                {
                    g.DrawLine(p2, xl, yl - j, xr, yl - j);//刻度线
                    string tempy = (j * ykedu).ToString();
                    g.DrawString(tempy, new Font("宋体", 8), sb, xl - 20, yl - j - 5);
                }
                else//小刻度，金黄色，10像素一个
                { g.DrawLine(p1, xl, yl - j, xr, yl - j); }
            }
        }



        //绘制y轴上的刻度
        public void SetXAxis(ref Graphics g, PictureBox picbox, float XMAX)
        {
            Pen p1 = new Pen(Color.Goldenrod, 1);
            Pen p2 = new Pen(Color.Black, 2);
            SolidBrush sb = new SolidBrush(Color.Black);

            float xkedu = XMAX / 400;
            float xt = 30, yt = picbox.Height - 33, xb = 30, yb = picbox.Height - 27;

            for (int i = 0; i < picbox.Width - 60; i += 10)
            {

                if (i % 50 == 0)
                {
                    g.DrawLine(p2, xt + i, yt, xb + i, yb);
                    string tempx = (i * xkedu).ToString();
                    g.DrawString(tempx, new Font("宋体", 8), sb, xt + i - 7, picbox.Height - 25);
                }
                else { g.DrawLine(p1, xt + i, yt, xb + i, yb); }
            }
        }
    }
}

