using System.Collections.Generic;
using System.Diagnostics;

namespace SolveSudokuCube
{
    public partial class Form1
    {
        private class Solution
        {
            public HashSet<int>[,,] ColorOptions = new HashSet<int>[4, 4, 4];

            public int[,,] PickedColor = new int[4, 4, 4];

            private static int[,,] Faces = new[, ,]  {
                                                         { {1,2}, {1,-1}, {1,4} },
                                                         { {2,-1},{-1,-1},{4,-1}},
                                                         { {3,2}, {3,-1}, {3,4} }
                                                     };

            public Solution()
            {
                InitColorOptions();
            }

            public Solution(Solution OldSolution)
            {
                ApplyToAll(
                    (i, j, k) =>
                    this.ColorOptions[i, j, k] = new HashSet<int>(OldSolution.ColorOptions[i, j, k])
                );

                ApplyToAll(
                    (i, j, k) =>
                    this.PickedColor[i, j, k] = OldSolution.PickedColor[i, j, k]
                );
            }

            public bool CheckFaces(Position position)
            {
                var x = position.x - 1;
                var y = position.y - 1;
                var relevantFaces = new List<int>() { Faces[x, y, 0], Faces[x, y, 1] };

                foreach (var face in relevantFaces)
                {
                    if ((face > 0) && (!CheckFace(face)))
                        return false;
                }

                return true;
            }

            public void SetColor(int layer, int x, int y, int pickedColor)
            {
                this.PickedColor[layer, x, y] = pickedColor;

                ApplyToLayer(layer,
                    (i, j, k) =>
                    this.ColorOptions[i, j, k].Remove(pickedColor)
                );
            }

            private bool Check(int l, int x, int y, HashSet<int> Bucket)
            {
                return ((this.PickedColor[l, x, y] != 0) && (!Bucket.Add(this.PickedColor[l, x, y])));
            }

            private bool CheckFace(int face)
            {
                var Bucket = new HashSet<int>();
                var valid = true;

                switch (face)
                {
                    // face 1 : front : {l,1,y}
                    case 1:
                        ApplyToAll(
                            (l, x, y) => { if (x == 1) { if ((this.PickedColor[l, x, y] != 0) && (!Bucket.Add(this.PickedColor[l, x, y]))) valid = false; } }
                        );
                        break;

                    // face 2 : left : {l,x,1}
                    case 2:
                        ApplyToAll(
                            (l, x, y) => { if (y == 1) { if ((this.PickedColor[l, x, y] != 0) && (!Bucket.Add(this.PickedColor[l, x, y]))) valid = false; } }
                        );
                        break;

                    // face 3 : back : {l,3,y}
                    case 3:
                        ApplyToAll(
                            (l, x, y) => { if (x == 3) { if ((this.PickedColor[l, x, y] != 0) && (!Bucket.Add(this.PickedColor[l, x, y]))) valid = false; } }
                        );
                        break;

                    // face 4 : right : {l,x,3}
                    case 4:
                        ApplyToAll(
                            (l, x, y) => { if (y == 3) { if ((this.PickedColor[l, x, y] != 0) && (!Bucket.Add(this.PickedColor[l, x, y]))) valid = false; } }
                        );
                        break;
                }

                return valid;
            }

            private void InitColorOptions()
            {
                ApplyToAll(
                    (i, j, k) =>
                           this.ColorOptions[i, j, k] = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                );
            }
        }
    }
}