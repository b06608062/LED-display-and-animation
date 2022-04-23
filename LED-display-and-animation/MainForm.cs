using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LED_display_and_animation
{
    public partial class MainForm : Form
    {
        PictureBox[,] LEDs;  // 16x32 LED 
        Bitmap onBmp, offBmp;
        List<int[]> frames = new List<int[]>();
        int check = 1 << 31;
        int idx = 0;
        int frameNumber;

        public MainForm()
        {
            InitializeComponent();
            onBmp = Properties.Resources.on;
            offBmp = Properties.Resources.off;
            LEDs = new PictureBox[16, 32];
            int w = ClientRectangle.Width / 32;
            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 32; c++)
                {
                    LEDs[r, c] = new PictureBox
                    {
                        Width = w,
                        Height = w,
                        Left = 8 + (31 - c) * w,
                        Top = 90 + r * w,
                        Image = offBmp,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                    };
                    LEDs[r, c].Click += LED_Click;
                    this.Controls.Add(LEDs[r, c]);
                }
            }
        }

        int[] RecordAFrame()
        {
            int[] currentFrame = new int[16];
            int mask;
            for (int r = 0; r < 16; r++)
            {
                currentFrame[r] = 0;
                for (int c = 0; c < 32; c++)
                {
                    if (LEDs[r, c].Image == onBmp)
                    {
                        mask = 1 << c;
                        currentFrame[r] = currentFrame[r] | mask;
                    }
                }
            }

            return currentFrame;
        }

        void SetFrameNumber()
        {
            frameNumber = frames.Count;
            label1.Text = $"Total number of frames is {frameNumber}";
            numericUpDown1.Maximum = frameNumber;
            numericUpDown1.Minimum = frameNumber > 0 ? 1 : 0;
        }

        void ShowAFrame(int[] frame)
        {
            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 32; c++)
                {
                    int mask = 1 << c;
                    LEDs[r, c].Image = (frame[r] & mask) == mask ? onBmp : offBmp;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frames.Add(RecordAFrame());
            SetFrameNumber();
            numericUpDown1.Value = frameNumber;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowAFrame(frames[++idx % frameNumber]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (frameNumber < 1) return;

            timer1.Enabled = !timer1.Enabled;
            idx = 0;
            if (timer1.Enabled)
            {
                button2.Text = "Stop";
                button2.ForeColor = System.Drawing.Color.Red;
                button1.Visible = button3.Visible = button4.Visible = button5.Visible = button7.Visible = button8.Visible = numericUpDown1.Visible = label2.Visible = false;
                pictureBox1.Visible = pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible = false;
            }
            else
            {
                button2.Text = "Run Animation";
                button2.ForeColor = System.Drawing.SystemColors.ControlText;
                button1.Visible = button3.Visible = button4.Visible = button5.Visible = button7.Visible = button8.Visible = numericUpDown1.Visible = label2.Visible = true;
                pictureBox1.Visible = pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible = true;
                ShowAFrame(frames[frameNumber - 1]);
                numericUpDown1.Value = frameNumber;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            frames.Clear();
            StreamReader sr = new StreamReader(openFileDialog1.FileName);
            frameNumber = Convert.ToInt32(sr.ReadLine());
            string[] data;
            for (int i = 0; i < frameNumber; i++)
            {
                data = sr.ReadLine().Split(' ');
                int[] frame = new int[16];
                for (int j = 0; j < 16; j++)
                {
                    frame[j] = Convert.ToInt32(data[j]);
                }
                frames.Add(frame);
            }

            sr.Close();
            SetFrameNumber();
            ShowAFrame(frames[0]);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
            sw.WriteLine(frameNumber);
            for (int i = 0; i < frameNumber; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    sw.Write(frames[i][j]);
                    sw.Write(" ");
                }
                sw.WriteLine();
            }

            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 32; c++)
                {
                    LEDs[r, c].Image = offBmp;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int[] tmp = new int[32];
            for (int i = 0; i < 32; i++)
            {
                tmp[i] = LEDs[0, i].Image == onBmp ? 1 : 0;
            }

            for (int r = 1; r < 16; r++)
            {
                for (int c = 0; c < 32; c++)
                {
                    LEDs[r - 1, c].Image = LEDs[r, c].Image;
                }
            }

            for (int i = 0; i < 32; i++)
            {
                LEDs[15, i].Image = tmp[i] == 1 ? onBmp : offBmp;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int[] tmp = new int[32];
            for (int i = 0; i < 32; i++)
            {
                tmp[i] = LEDs[15, i].Image == onBmp ? 1 : 0;
            }

            for (int r = 15; r > 0; r--)
            {
                for (int c = 0; c < 32; c++)
                {
                    LEDs[r, c].Image = LEDs[r - 1, c].Image;
                }
            }

            for (int i = 0; i < 32; i++)
            {
                LEDs[0, i].Image = tmp[i] == 1 ? onBmp : offBmp;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            int[] tmp = RecordAFrame();
            for (int i = 0; i < 16; i++)
            {
                tmp[i] = (tmp[i] & check) != 0 ? tmp[i] << 1 | 1 : tmp[i] <<= 1;
            }

            ShowAFrame(tmp);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            int[] tmp = new int[16];
            for (int i = 0; i < 16; i++)
            {
                tmp[i] = LEDs[i, 0].Image == onBmp ? 1 : 0;
            }

            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 31; c++)
                {
                    LEDs[r, c].Image = LEDs[r, c + 1].Image;
                }
            }

            for (int i = 0; i < 16; i++)
            {
                LEDs[i, 31].Image = tmp[i] == 1 ? onBmp : offBmp;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frames.Clear();
            for (int r = 0; r < 16; r++)
            {
                for (int c = 0; c < 32; c++)
                {
                    LEDs[r, c].Image = offBmp;
                }
            }

            SetFrameNumber();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int[] tmp = RecordAFrame();
            for (int i = 0; i < 16; i++)
            {
                tmp[i] = (tmp[i] & check) != 0 ? tmp[i] << 1 | 1 : tmp[i] <<= 1;
            }

            ShowAFrame(tmp);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer2.Enabled = !timer2.Enabled;
            if (timer2.Enabled)
            {
                button5.Text = "Stop";
                button5.ForeColor = System.Drawing.Color.Red;
                button1.Visible = button2.Visible = button3.Visible = button4.Visible = button7.Visible = button8.Visible = numericUpDown1.Visible = label2.Visible = false;
            }
            else
            {
                button5.Text = "Cycle";
                button5.ForeColor = System.Drawing.SystemColors.ControlText;
                button1.Visible = button2.Visible = button3.Visible = button4.Visible = button7.Visible = button8.Visible = numericUpDown1.Visible = label2.Visible = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (frameNumber == 0) return;
            ShowAFrame(frames[Convert.ToInt32(numericUpDown1.Value) - 1]);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int tmp = Convert.ToInt32(numericUpDown1.Value);
            if (tmp == 0) return;
            frames.RemoveAt(tmp - 1);
            SetFrameNumber();

            if (frameNumber > tmp - 1)
            {
                ShowAFrame(frames[tmp - 1]);
            }
            else if (frameNumber > 0)
            {
                ShowAFrame(frames[frameNumber - 1]);
            } else
            {
                for (int r = 0; r < 16; r++)
                {
                    for (int c = 0; c < 32; c++)
                    {
                        LEDs[r, c].Image = offBmp;
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (frameNumber == 0) return;
            frames[Convert.ToInt32(numericUpDown1.Value) - 1] = RecordAFrame();
        }

        private void LED_Click(object sender, EventArgs e)
        {
            PictureBox theLED = (PictureBox)sender;
            theLED.Image = theLED.Image == onBmp ? offBmp : onBmp;
        }
    }
}
