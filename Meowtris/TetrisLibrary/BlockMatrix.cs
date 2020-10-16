using System.Windows.Media;

namespace TetrisLibrary
{
    public struct BlockMatrix
    {
        public Brush Color { get; private set; }

        public BlockType BlockType { get; set; }

        public BlockMatrix(Brush color, BlockType type)
        {
            Color = color;
            BlockType = type;
        }

        public override string ToString()
        {
            return $"{Color} {BlockType}";
        }
    }
}
