using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        const int width = 200;
        const int height = 60;
        const int radious = 20;
        const int gap = 20;

        int Pfrom, Pto, Hfrom, Hto;
        int Px = 0, Py = 0;
        int Hmax = 0;

        int[] num = new int[100000];	//各状态子节点数量
        int[] vH = new int[100000];		//各节点启发函数值
        int[] mat = new int[100000];	//各节点原值（9位整形）
        string[] str = new string[100000];	//各节点字符串值（9位string）
        int[,] edge = new int[100000, 4];	//各节点子节点离散化值
        int[] Wmy = new int[100000];		//各节点宽度
        int[,] Wson = new int[100000, 4];	//各节点子节点宽度

        int mpfrom, mpto;
        static Dictionary<int, int> mp = new Dictionary<int, int>();
        //mp用于离散化，将9位整形离散化为较小值
        static Dictionary<int, bool> mp2 = new Dictionary<int, bool>();
        //mp2记录各节点是否为解序列上节点
        int cnt = 1;	//用于9位整形的离散化


        private void init()
        {
            mp2.Clear();
            mp.Clear();
            for (int i = 1; i <= cnt; i++)
            {
                num[i] = 0;
            }
            cnt = 1;

        }
        public Form2(int[] nums, int tot)
        {
            init();

            for (int i = 0; i < tot; i++)
            {
                mp2[nums[i]] = true;
            }
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            readin();
            calW(1);
            Graphics g = panel1.CreateGraphics();
            g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            draw(g, 1, 50, width, -1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); //关闭From2窗体。 
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Py > -Hmax + 200)
                Py -= 100;
            else
                return;
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            g.TranslateTransform(Px, Py);
            draw(g, 1, 50, width, -1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (Py < 0)
                Py += 100;
            else
                return;
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            g.TranslateTransform(Px, Py);
            draw(g, 1, 50, width, -1);
        }

        private void Right_Click(object sender, EventArgs e)
        {
            if (Px < 0)
                Px += 100;
            else
                return;
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            g.TranslateTransform(Px, Py);
            draw(g, 1, 50, width, -1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Px > -Wmy[1] + 500)
                Px -= 100;
            else
                return;
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.White);
            g.TranslateTransform(Px, Py);
            draw(g, 1, 50, width, -1);
        }

        private void readin()
        {
            using (StreamReader sr = new StreamReader(@"edge.txt"))
            {
                string line;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    string[] points = line.Split(' ');
                    Pfrom = Convert.ToInt32(points[0]);
                    Pto = Convert.ToInt32(points[1]);
                    Hfrom = Convert.ToInt32(points[2]);
                    Hto = Convert.ToInt32(points[3]);
                    if (!mp.ContainsKey(Pfrom))
                    {
                        mp[Pfrom] = cnt;
                        cnt++;
                    }
                    if (!mp.ContainsKey(Pto))
                    {
                        mp[Pto] = cnt;
                        cnt++;
                    }
                    mpfrom = mp[Pfrom];
                    mpto = mp[Pto];
                    edge[mpfrom, num[mpfrom]] = mpto;
                    num[mpfrom]++;
                    vH[mpfrom] = Hfrom;
                    vH[mpto] = Hto;
                    mat[mpfrom] = Pfrom;
                    mat[mpto] = Pto;
                    str[mpfrom] = points[0];
                    str[mpto] = points[1];
                }
            }
        }

        private int calW(int now)
        {
            for (int i = 0; i < num[now]; i++)
            {
                Wson[now, i] = calW(edge[now, i]);
                Wmy[now] += Wson[now, i];
            }
            if (Wmy[now] == 0)
            {
                Wmy[now] = gap;
            }
            return Wmy[now];
        }

        //g画布，now当前节点，h当前高度，w当前宽度
        private void print(Graphics g, int now, int h, int w)
        {
            Font font = new Font("STXingkai", 7, FontStyle.Regular);//设置字体
            if (mp2.ContainsKey(mat[now]))
                g.DrawString(str[now].Substring(0, 3) + "\r\n" + str[now].Substring(3, 3) + "\r\n" +
                str[now].Substring(6, 3), font, Brushes.Red, w - 10, h);//Brushes.Red:字的颜色红色
            else
                g.DrawString(str[now].Substring(0, 3) + "\r\n" + str[now].Substring(3, 3) + "\r\n" +
                str[now].Substring(6, 3), font, Brushes.Black, w - 10, h);//Brushes.Red:字的颜色红色
        }


        //g画布，now当前节点，hnow当前高度，wbase当前节点左侧宽度，midfa父节点位置x坐标
        private void draw(Graphics g, int now, int hnow, int wbase, int midfa)
        {
            if (hnow > Hmax)
                Hmax = hnow;
            int mid = wbase + Wmy[now] / 2;
            int wnow = wbase;

            print(g, now, hnow, mid);   //画状态数组

            //画与父节点的连线
            if (midfa != -1)
            {
                if (mp2.ContainsKey(mat[now]))
                    g.DrawLine(new Pen(Color.Red), mid, hnow, midfa, hnow - 30);
                else
                    g.DrawLine(new Pen(Color.Black), mid, hnow, midfa, hnow - 30);
            }

            //启发函数值
            Font font = new Font("楷体", 10, FontStyle.Regular);//设置字体
            g.DrawString(vH[now].ToString(), font, Brushes.Brown, mid - radious / 2, hnow - radious / 3 * 2);//Brushes.Red:字的颜色红色

            for (int i = 0; i < num[now]; i++)	//递归画子节点
            {
                draw(g, edge[now, i], hnow + height, wnow, mid);
                wnow += Wson[now, i];
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
//236175480