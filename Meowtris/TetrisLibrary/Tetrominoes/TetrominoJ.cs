using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TetrisLibrary.Tetrominoes
{
    internal class TetrominoJ : PositionedTetromino
    {
        internal override Brush TetrominoColor { get; set; } = Brushes.Blue;

        internal override TetrominoType Type { get; set; } = TetrominoType.J;


        internal override int[][] GetPositionIndices(TetrominoPosition position)
        {
            switch (position)
            {
                case TetrominoPosition.Initial:
                    return new int[][] {
                        new int[] { 0, 0 },
                        new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, 2 }
                    };

                case TetrominoPosition.Degree90:
                    return new int[][] {
                        new int[] { 0, 1 }, new int[] { 0, 2 },
                        new int[] { 1, 1 },
                        new int[] { 2, 1 }
                    };

                case TetrominoPosition.Degree180:
                    return new int[][] {
                        new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, 2 },
                                                                new int[] { 2, 2 }
                    };

                case TetrominoPosition.Degree270:
                    return new int[][] {
                                            new int[] { 0, 1 },
                                            new int[] { 1, 1 },
                        new int[] { 2, 0 }, new int[] { 2, 1 }
                    };
            }

            return null;
        }
    }
}