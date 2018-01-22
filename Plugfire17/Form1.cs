using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugfire17
{
    public partial class Form1 : Form
    {
        private Bitmap _bpm;
        private Stopwatch _sw = new Stopwatch();
        Plugfire _plugfire = new Plugfire();

        public Form1()
        {
            InitializeComponent();

            Width = Height = 1024;
            if (false)
            {
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.TopMost = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }

            _bpm = new Bitmap(Plugfire.WIDTH, Plugfire.HEIGHT, PixelFormat.Format32bppPArgb);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override unsafe void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!_sw.IsRunning)
            {
                _sw.Start();
            }

            _plugfire.Frame(_sw.Elapsed.TotalMilliseconds, _sw.Elapsed.TotalMilliseconds);

            // Fill bitmap
            var front = _bpm.LockBits(new Rectangle(0, 0, Plugfire.WIDTH, Plugfire.HEIGHT), ImageLockMode.ReadWrite, _bpm.PixelFormat);
            var pFront = (uint*)front.Scan0;
            for (int i = 0; i < Plugfire.WIDTH * Plugfire.HEIGHT; i++)
                pFront[i] = (uint)(_plugfire._bFront[i*4+2] * 0x100) + 0xFF000000;
            _bpm.UnlockBits(front);

            // Show bitmap
            e.Graphics.DrawImage(_bpm, new Rectangle(0, 0, Width, Height));

            //Thread.Sleep(1000 / 60);
            this.Invalidate();
        }
    }
}
