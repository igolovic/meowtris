using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TetrisLibrary.Tetrominoes
{
    internal class TetrominoO : PositionedTetromino
    {
        internal override Brush TetrominoColor { get; set; } = Brushes.Yellow;

        internal override TetrominoType Type { get; set; } = TetrominoType.O;

        internal override int[][] GetPositionIndices(TetrominoPosition position)
        {
            return new int[][] { new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 1, 1 }, new int[] { 1, 2 } };
        }
    }
}