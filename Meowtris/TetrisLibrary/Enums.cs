using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisLibrary
{
    public enum TetrominoType
    {
        I = 0,
        J = 1,
        L = 2,
        O = 3,
        S = 4,
        T = 5,
        Z = 6
    }

    public enum BlockType
    {
        Clear = 0,
        Falling = 1,
        Landed = 2
    }

    public enum TetrominoDirection
    {
        Left = 0,
        Right = 1
    }

    public enum TetrominoPosition
    {
        Initial = 0,
        Degree90 = 1,
        Degree180 = 2,
        Degree270 = 3
    }
}
