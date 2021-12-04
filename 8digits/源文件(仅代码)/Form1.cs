using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //加载c++程序
        [DllImport("solve_ans.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static void solve_ans(int start_, int end_,bool hao);

        int[] nums = new int[10005];
        int[] use = new int[10];
        int wz = 0;
        int totSteps;
        string begStatus, endStatus, cntSearch, cntExpand, maxdeep, timeuse;


        private void printAnsText()
        {
            textBox3.Text = "解序列:\r\n";
            for (int i = 0; i < totSteps; i++)
            {
                int now = nums[i];
                for (int j = 8; j >= 0; j--)
                {
                    use[j] = now % 10;
                    now /= 10;
                }
                textBox3.Text += "第" + i.ToString() + "步:\r\n";
                for (int j = 0; j < 3; j++)
                {
                    textBox3.Text += use[j * 3].ToString() + " " + use[j * 3 + 1].ToString() + " " + use[j * 3 + 2].ToString() + "\r\n";
                }
                textBox3.Text += "\r\n\r\n";
            }
        }

        //初始化 初始状态，目标状态
        private void printInit()
        {
            
            int now = nums[0];
            for (int i = 8; i >= 0; i--)
            {
                use[i] = now % 10;
                now /= 10;
            }
            for (int i = 1; i <= 9; i++)
            {
                string str = "label_init" + i.ToString();
                Control ctl = Controls[str];
                if (use[i - 1] == 0)
                {
                    ctl.Text = " ";
                    ctl.BackColor = Color.SaddleBrown;
                    ctl.ForeColor = Color.Sienna;
                }
                else
                {
                    ctl.Text = use[i - 1].ToString();
                    ctl.BackColor = Color.Coral;
                    ctl.ForeColor = Color.Sienna;

                }
            }

            now = nums[totSteps - 1];
            for (int i = 8; i >= 0; i--)
            {
                use[i] = now % 10;
                now /= 10;
            }
            for (int i = 1; i <= 9; i++)
            {
                string str = "label_end" + i.ToString();
                Control ctl = Controls[str];
                if (use[i - 1] == 0)
                {
                    ctl.Text = " ";
                    ctl.BackColor = Color.SaddleBrown;
                    ctl.ForeColor = Color.Sienna;
                }
                else
                {
                    ctl.Text = use[i - 1].ToString();
                    ctl.BackColor = Color.Coral;
                    ctl.ForeColor = Color.Sienna;

                }
            }
            Controls["countSearch"].Text = cntSearch;
            Controls["countExpand"].Text = cntExpand;
            Controls["maxDeep"].Text = maxdeep;
            Controls["usetime"].Text = timeuse+"ms";
        }

        //下一步
        private void nextStep()
        {
            

            Controls["steps"].Text = wz.ToString();
            int now = nums[wz++];
            for (int i = 8; i >= 0; i--)
            {
                use[i] = now % 10;
                now /= 10;
            }
            for (int i = 1; i <= 9; i++)
            {
                string str = "label" + i.ToString();
                Control ctl = Controls[str];
                if (use[i - 1] == 0)
                {
                    ctl.Text = " ";
                    ctl.BackColor = Color.SaddleBrown;
                    ctl.ForeColor = Color.Sienna;
                }
                else
                {
                    ctl.Text = use[i - 1].ToString();
                    ctl.BackColor = Color.Coral;
                    ctl.ForeColor = Color.Sienna;

                }
            }
        }

        public Form1()
        {
            InitializeComponent();

        }

        //下一步
        private void button1_Click(object sender, EventArgs e)
        {
            if (wz == totSteps)
            {
                Control ctl = Controls["alert"];
                ctl.Text = "已结束！";
                ctl.ForeColor = Color.Red;
            }
            else
            {
                nextStep();
            }
        }

        //重置当前状态
        private void button2_Click(object sender, EventArgs e)
        {
            Controls["alert"].Text = " ";
            wz = 0;
            nextStep();
        }

        //重新计算答案
        private void button3_Click(object sender, EventArgs e)
        {
            begStatus = Controls["TextBox1"].Text;
            endStatus = Controls["TextBox2"].Text;
            //Controls["alert"].Text =begStatus+" "+endStatus;
            init(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            begStatus = Controls["TextBox1"].Text;
            endStatus = Controls["TextBox2"].Text;
            //Controls["alert"].Text =begStatus+" "+endStatus;
            init(false);
        }

        private void label19_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //this.Hide();  //调用Form1的Hide()方法隐藏Form1窗口
            Form2 form2 = new Form2(nums,totSteps); //生成一个Form2对象
            form2.ShowDialog();  //将Form2窗体显示为模式对话框。
        }

        private bool checkNoans()
        {
            int begcnt = 0, endcnt = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (begStatus[j] > begStatus[i] && begStatus[i]>'0')
                        begcnt++;
                    if (endStatus[j] > endStatus[i] && endStatus[i]>'0')
                        endcnt++;
                }
            }
            if (begcnt % 2 != endcnt % 2)
            {
                alert.Text = "无解";
                return true;
            }
            alert.Text = "有解";
            return false;
        }

        private bool checkNum()
        {
            if(begStatus.Length!=9 || endStatus.Length != 9)
            {
                MessageBox.Show("请输入9位0-8数字！", "提示信息！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            int[] vis = new int[9];

            for (int i = 0; i < 9; i++)
            {
                if (begStatus[i]<'0' || begStatus[i]>'8' || vis[begStatus[i]-'0']==1)
                {
                    MessageBox.Show("请输入0-8的数字！", "提示信息！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                vis[begStatus[i] - '0'] = 1;
            }
            for (int i = 0; i < 9; i++)
            {
                if (endStatus[i] < '0' || endStatus[i] > '8' || vis[endStatus[i] - '0'] == 2)
                {
                    MessageBox.Show("请输入0-8的数字！", "提示信息！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                vis[endStatus[i] - '0'] = 2;
            }
            return false;
        }

        //每次更换数据后重新计算答案，并显示在屏幕
        private void init(bool good)
        {
            alert.Text = " ";
            //Graphics g = panel1.CreateGraphics();
            //g.DrawLine(new Pen(Color.Sienna), 100, 100, 100, 200);
            //g.FillEllipse(new SolidBrush(Color.Sienna), 100, 100, 20, 20);

            wz = 0;
            if(checkNum() || checkNoans())
            {
                return;
            }
            calAnswer(good);
            readin();
            printInit();
            printAnsText();
            nextStep();
        }

        //读取文件中的答案信息
        private void readin()
        {
            totSteps = 0;
            //using (StreamReader sr = new StreamReader(@"C:\Users\Sevenenen\cread.txt"))
            using (StreamReader sr = new StreamReader(@"cread.txt"))
            {
                string line;
                begStatus = sr.ReadLine();
                endStatus = sr.ReadLine();
                cntSearch = sr.ReadLine();
                cntExpand = sr.ReadLine();
                maxdeep = sr.ReadLine();
                timeuse = sr.ReadLine();

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    nums[totSteps++] = Convert.ToInt32(line);
                }
            }
        }

        //通过调用c++程序来计算答案
        private void calAnswer(bool hao)
        {
            solve_ans(int.Parse(begStatus), int.Parse(endStatus), hao);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
