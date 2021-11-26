using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Universe
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private int resolution;
        private bool [,] field;
        private int rows, cols;

        private int cGeneration = 0;

        public Form1()
        {
            InitializeComponent();
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            // Добавление живых клеток
            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if (ValidateMousePosition(x, y))
                    field[x, y] = true;
            }

            // Удаление живых клеток
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if (ValidateMousePosition(x, y))
                    field[x, y] = false;
            }
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        /* =============== Вспомогательные методы =============== */

        private void StartGame()
        {
            cGeneration = 0;
            Text = $"Generation {cGeneration}";

            if (timer1.Enabled)
                return;

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            bStart.Enabled = false;
            bStop.Enabled = true;

            resolution = (int)nudResolution.Value;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            field = new bool[cols, rows];

            Random r = new Random();

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = r.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;

                }
            }

            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;

            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;

            bStop.Enabled = false;
            bStart.Enabled = true;
        }

        private void NextGeneration()
        {
            Text = $"Generation {++cGeneration}";

            g.Clear(Color.Black);

            var newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];


                    if (hasLife)
                        g.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);

                }
            }

            field = newField;
            pictureBox1.Refresh();
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x <= cols && y <= rows;
        }
    }
}
