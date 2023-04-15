using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dodger
{
    public partial class Form1 : Form
    {
        Process p;

        private int dodgedValue = 0;
        public int dodged
        {
            
            set
            {
                dodgedValue = value;
                progressBar2.Value = (value < 200 ? value : 200);
                label2.Text = value.ToString();
            }
            get
            {
                return this.dodgedValue;
            }
        }
        public Form1()
        {
            InitializeComponent();

            Process[] ps = Process.GetProcessesByName("chiaki");

            if (ps.Length == 0)
            {
                MessageBox.Show("Chiaki not found!");
                return;
            }

            p = ps[0];
        }

        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0101;
        const int VK_RETURN = 0x0D;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        public void SendKey()
        {
            PostMessage(p.MainWindowHandle, WM_KEYDOWN, VK_RETURN, 0);
            Thread.Sleep(50);

            PostMessage(p.MainWindowHandle, WM_KEYUP, VK_RETURN, 0);

            dodged++;
            
            Thread.Sleep(1500);
        }


        public double GetBrightness(Color color)
        {
            return (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(5000);
            Bitmap b = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

            while (!backgroundWorker1.CancellationPending)
            {
                Graphics g = Graphics.FromImage(b);
                g.CopyFromScreen(0, 0, 0, 0, b.Size);
                g.Dispose();

                double brightness = GetBrightness(b.GetPixel(100, 100));
                this.Text = brightness.ToString();
                if (brightness > 121)
                {
                    //MessageBox.Show("ding");
                    SendKey();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dodged = 0;
        }
    }
}
