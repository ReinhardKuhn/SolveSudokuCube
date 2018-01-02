using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SolveSudokuCube
{
    public partial class Form1 : Form
    {
        private static int[] bounds = new[] { 1, 2, 3 };

        private Button[,,] Buttons = new Button[4, 4, 4];

        private Dictionary<int, Color> ColorMap = new Dictionary<int, Color> { { 0, Color.Beige },
                                                                               { 1, Color.Red },
                                                                               { 2, Color.DarkBlue },
                                                                               { 3, Color.Yellow },
                                                                               { 4, Color.LightGreen },
                                                                               { 5, Color.Orange },
                                                                               { 6, Color.DarkGreen },
                                                                               { 7, Color.Purple },
                                                                               { 8, Color.Brown },
                                                                               { 9, Color.LightBlue } };

        private Solution CurrentSolution;
        private double totalTries;

        public Form1()
        {
            InitializeComponent();
        }

        private static void ApplyToAll(Action<int, int, int> work)
        {
            foreach (var i in bounds)
                ApplyToLayer(i, work);
        }

        private static void ApplyToLayer(int layer, Action<int, int, int> work)
        {
            foreach (var j in bounds)
                foreach (var k in bounds)
                    work(layer, j, k);
        }

        private void AssignButtons()
        {
            ApplyToAll(
                (i, j, k) =>
                    this.Buttons[i, j, k] = (Button)this.Controls.Find("b" + i + j + k, true)[0]
            );
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var timer = new Stopwatch();
            timer.Start();

            Init();

            var Solution = Solve(new Position(1, 1, 1));

            timer.Stop();
            this.txtTime.Text = $"({timer.ElapsedMilliseconds}) ms : ({this.totalTries}) calculations"; ;
            HighlightSolution(this.CurrentSolution);
        }

        private void HighlightSolution(Solution solution)
        {
            ApplyToAll(
                (l, x, y) =>
                {
                    this.Buttons[l, x, y].BackColor = Color.Red;
                    Thread.Sleep(100);
                    Application.DoEvents();
                    this.Buttons[l, x, y].BackColor = this.ColorMap[solution.PickedColor[l, x, y]];
                }
            );
        }

        private void Init()
        {
            AssignButtons();

            this.CurrentSolution = new Solution();
        }

        private Solution Solve(Position position)
        {
            if (position == null)
                return this.CurrentSolution;

            this.totalTries += 1;

            var oldSolution = this.CurrentSolution;

            for (var color = 1; color < 10; color++)
            {
                this.CurrentSolution = new Solution(this.CurrentSolution);

                if (this.CurrentSolution.ColorOptions[position.l, position.x, position.y].Contains(color))
                {
                    this.CurrentSolution.SetColor(position.l, position.x, position.y, color);

                    if (this.CurrentSolution.CheckFaces(position))
                    {
                        var FinalSolution = Solve(position.Next());

                        if (FinalSolution != null)
                            return FinalSolution;
                    }
                }

                this.CurrentSolution = oldSolution;
            }

            return null;
        }

        private class Position
        {
            public int l;
            public int x;
            public int y;

            public Position(int l, int x, int y)
            {
                this.l = l;
                this.x = x;
                this.y = y;
            }

            public Position Next()
            {
                if (this.y != 3)
                    return new Position(this.l, this.x, this.y + 1);

                if (this.x != 3)
                    return new Position(this.l, this.x + 1, 1);

                if (this.l != 3)
                    return new Position(this.l + 1, 1, 1);

                return null;
            }
        }
    }
}