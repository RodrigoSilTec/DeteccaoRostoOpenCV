using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeteccaoRosto
{
    public partial class FrmDeteccaoRosto : Form
    {
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        VideoCapture capture;

        public FrmDeteccaoRosto()
        {
            InitializeComponent();
        }

        private void ProcesarImagem(Bitmap btm)
        {
            Bitmap bitmap = (Bitmap)btm.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.4, 1);
            foreach (var rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }
            picImagem.Image = bitmap;
        }
private void abrirImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, Filter = "JPEG|*.jpg"})
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    ProcesarImagem(new Bitmap(Image.FromFile(ofd.FileName)));
                }
            }
        }

        private void iniciarWebcamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(capture != null)
            {
                MessageBox.Show("A webcam já está iniciada!");
                return;
            }
            capture = new VideoCapture(0);
            capture.ImageGrabbed += Capture_ImageGrabbed;
            capture.Start();
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                ProcesarImagem(m.Bitmap);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pararWebcamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture == null)
            {
                MessageBox.Show("A webcam já está desligada!");
                return;
            }

            capture.ImageGrabbed += Capture_ImageGrabbed;
            capture.Stop();
            capture.Dispose();
            capture = null;
            picImagem.Image = null;
        }
    }
}
