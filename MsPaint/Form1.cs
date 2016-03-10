using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;//在MWArray.dll，最常用的 
using MathWorks.MATLAB.NET.Utility;// 在MWArray.dll，最常用的 
using third;
namespace MsPaint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        double[,] rbegin;
        double[,] pend;
        double[,] rpk;
        double qt_mean;
        double[] ce = new double[5];
        MWArray d1;
        double[,] f1;
        MWArray[] agrsOut = new MWArray[4];//两个输出参数，一定要写数量
        public Point[] ptlist;//存放点的数组
        Point[] data = new Point[12000];
        //Random rm = new Random();//随机数产生器
        Timer mytimer = new Timer();//定时器
        third.Class1 pcl = new Class1();
        int 网格间距 = 12; //网格间距
        int 网格偏移 = 0;   //网格偏移
        Pen 网格颜色 = new Pen(Color.FromArgb(0x00, 0x80, 0x40));
        Pen 曲线颜色 = new Pen(Color.Lime);
        Pen R颜色 = new Pen(Color.Red,1);
        Pen Q颜色 = new Pen(Color.DeepSkyBlue, 1);
        Pen T颜色 = new Pen(Color.DeepPink, 1);
        int time_count = 20;
        //窗口加载时调用
        private void Form1_Load(object sender, EventArgs e)
        {
            //设置控件的样式和行为，以减少图像的闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        draw drawtest = new draw();//创建类 draw 的实例


        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "(*.mat)|*.mat|所有文件(*.*)|*.*"; if (d.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = File.OpenRead(d.FileName); StreamReader sr = new StreamReader(fs); string s;
                string filename = d.FileName;
                d1 =pcl.loaddata(filename);
               // MessageBox.Show(filename);
                MWArray[] agrsIn = new MWArray[] {d1}; 
                pcl.pces(4, ref agrsOut, agrsIn);
                MWNumericArray x1 = agrsOut[0] as MWNumericArray;
                MWNumericArray x2 = agrsOut[1] as MWNumericArray;
                MWNumericArray x3 = agrsOut[2] as MWNumericArray;
                MWNumericArray x4 = agrsOut[3] as MWNumericArray;
                rbegin = (double[,])x1.ToArray();
                pend = (double[,])x2.ToArray();
                rpk = (double[,])x3.ToArray();
                f1 = (double[,])d1.ToArray();
                qt_mean = x4.ToScalarDouble();
                textBox4.Text = qt_mean.ToString();
                for (int i = 0; i < 12000; i++)
                {
                    data[i].X = (int)i;//强制类型转换，将double转为int，可能会丢失数据
                    data[i].Y = (int)((1000-f1[0, i*5]) * 250 / 4500 + 100);
                }
                this.timer1.Enabled = true;//可以使用
                this.timer1.Interval = 100;//定时时间为100毫秒
                this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

                this.timer1.Start();//启动定时器
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //动态添加一个定时器
            timer1.Start();//启动定时器
            textBox1.Enabled = false;
            textBox5.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            textBox1.Enabled = true;
            textBox5.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (time_count == 1100)
                time_count = 0;
            else
                time_count++;
           
           网格偏移 = (网格偏移 + 1000) % 网格间距;
            Invalidate();
            Point[] temp = new Point[600];
            for (int i = 0; i < 600; i++)
            {
                temp[i].X = i;
                temp[i].Y = data[time_count * 10 + i].Y;
            }
            //调用绘图函数,这里的参数可以根据不同的测量给定不同的实参
            //drawtest.DrawLineS(Color.Blue, 1200, 600, pictureBox1, ptlist);
            int index = (int)time_count / 20;
            double QT = (pend[0, index]-rbegin[0, index])/1000;
            Graphics g2 = pictureBox2.CreateGraphics();//创建 PictureBox窗体的画布
            if ((QT < double.Parse(textBox1.Text)) || (QT > double.Parse(textBox5.Text)))
            {
                g2.FillRectangle(Brushes.Red, g2.ClipBounds);
            }
            else
            {
                g2.FillRectangle(Brushes.LightGreen, g2.ClipBounds);
            }
            textBox2.Text = Convert.ToString(QT);
            double QR = (rpk[0, index]-rbegin[0, index])/1000;
            textBox3.Text = Convert.ToString(QR);
           // drawtest.DrawLineS(Color.Blue, 400, 2400, pictureBox1, data);
            Graphics g1 = pictureBox1.CreateGraphics();//创建 PictureBox窗体的画布
            g1.FillRectangle(Brushes.Black, g1.ClipBounds);
            //绘制纵线 从右向左绘制
            for (int i = pictureBox1.Width - 网格偏移; i >= 0; i -= 网格间距)
                g1.DrawLine(网格颜色, i, 0, i, pictureBox1.Height);
            //绘制横线
            for (int i = pictureBox1.Height; i >= 0; i -= 网格间距)
                g1.DrawLine(网格颜色, 0, i, pictureBox1.Width, i);
            for (int i = 0; i < 60; i++)
            {
                    int line = (int)(rpk[0, i]/5-time_count * 10);
                    g1.DrawLine(R颜色, line, 0, line, pictureBox1.Height);
            }
            for (int i = 0; i < 60; i++)
            {
                int line = (int)(pend[0, i] / 5 - time_count * 10);
                g1.DrawLine(T颜色, line, 0, line, pictureBox1.Height);
            }
            for (int i = 0; i < 60; i++)
            {
                int line = (int)(rbegin[0, i] / 5 - time_count * 10);
                g1.DrawLine(Q颜色, line, 0, line, pictureBox1.Height);
            }
            //绘制曲线 若想曲线从右向左移动，则必须先绘制后面的       
            Pen p = new Pen(Color.Yellow, 1);//画笔
                g1.DrawLines(p, temp);//五点绘图，直线连接
        }
    }
}