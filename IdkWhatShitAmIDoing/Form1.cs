using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
//using Accord.Vision.Detection;
//using Accord.Vision.Detection.Cascades;

namespace IdkWhatShitAmIDoing
{
    public partial class Form1 : Form
    {
        Bitmap template2;
        Bitmap[] searching= new Bitmap[100];
        int _findLen = 0;
        int imageX = 0;
        int imageY = 0;
       
      

        public Bitmap ConvertToFormat(System.Drawing.Image img, System.Drawing.Imaging.PixelFormat format)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Bitmap copy = new Bitmap(img.Width, img.Height, format);
            using (Graphics gr = Graphics.FromImage(copy))
            {
                gr.DrawImage(img, new Rectangle(0, 0, copy.Width, copy.Height));
            }
            return copy;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);
       

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        


        bool StartCheck = false;
        public Form1()
        {
            InitializeComponent();
            trackBar1.Value = 91;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
          
        }

        public void Checking(Bitmap screenshot, Bitmap searchingObj)
        {
            const int divisor = 4;
          
            ExhaustiveTemplateMatching shit = new ExhaustiveTemplateMatching(0.925f);
            
                TemplateMatch[] matching = shit.ProcessImage(
                    new ResizeNearestNeighbor(screenshot.Width / divisor, screenshot.Height / divisor).Apply(screenshot),
                    new ResizeNearestNeighbor(searchingObj.Width / divisor, searchingObj.Height / divisor).Apply(searchingObj));
            
                if (matching.Length == 1)
                {
                    Rectangle rect = matching[0].Rectangle;
                imageX = rect.Location.X * divisor;
                imageY = rect.Location.Y* divisor;

                Cursor.Position = new System.Drawing.Point(imageX, imageY);
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, Convert.ToUInt32(imageX), Convert.ToUInt32(imageY), 0, 0);

            }
                else
                {
                   

                }
            }
           
        


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(10);
            int screenWidth = Screen.GetBounds(new System.Drawing.Point(0, 0)).Width;
            int screenHeight = Screen.GetBounds(new System.Drawing.Point(0, 0)).Height;
            Bitmap bmpScreenShot = new Bitmap(screenWidth, screenHeight);
            Graphics gfx = Graphics.FromImage(bmpScreenShot);
            gfx.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));

            Bitmap template = ConvertToFormat(bmpScreenShot, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            template2 = ConvertToFormat(bmpScreenShot, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            pictureBox1.Image = null;
            pictureBox1.Image = template;
            bmpScreenShot.Dispose();

            gfx.Dispose();
            StartCheck = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();

            System.Threading.Thread.Sleep(100);
            backgroundWorker2.RunWorkerAsync();

            if(!timer1.Enabled)
                timer1.Enabled = true;
            else
            {
                timer1.Enabled = false;
            }
         
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          

           
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


           
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (!File.Exists(Application.StartupPath + @"\find0.bmp"))
            {
                MessageBox.Show("splah0.bmp не найден... \n файл должен быть во дной директории с ботом ", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else textBox1.Text = "Нормас";
            _findLen = 0;
         
            for (int i = 0; ; i++)
            {
                string path = Application.StartupPath + @"\find" + i.ToString() + ".bmp";
                if (!File.Exists(path))
                {
                    break;
                }
                Bitmap finding = new Bitmap(path);
                searching[i] = ConvertToFormat(finding, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
               
                _findLen++;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (imageX == 0 && imageY == 0)
            {
                return;
            }

            Rectangle ee = new Rectangle(Convert.ToInt32(imageX/4.1)+2, Convert.ToInt32(imageY/4.1)-11, 10, 10);
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, ee);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < _findLen; i++)
            {
                Checking(template2, searching[i]);
                //System.Threading.Thread.Sleep(00);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!backgroundWorker2.IsBusy)
            {
                backgroundWorker2.RunWorkerAsync();
            }
        }
    }
}
