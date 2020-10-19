using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using TetrisLibrary.Tetrominoes;

namespace TetrisLibrary
{
    internal abstract class PositionedTetromino
    {
        private TetrominoPosition currentPosition { get; set; }

        private List<BlockPosition> blocks { get; set; } = new List<BlockPosition>();

        internal abstract Brush TetrominoColor { get; set; }

        internal abstract TetrominoType Type { get; set; }

        internal abstract int[][] GetPositionIndices(TetrominoPosition position);

        internal void RotateTetromino()
        {
            GetCurrentOffset(out int rowOffset1, out int columnOffset1, out int rowOffset2, out int columnOffset2);

            // Set next position as current if rotation succeeded, if not just move on
            var nextPosition = GetNextPosition();
            if (SetPosition(nextPosition, rowOffset1, columnOffset1, rowOffset2, columnOffset2))
                currentPosition = nextPosition;
        }

        internal bool SetPosition(TetrominoPosition nextPosition, int rowOffset1, int columnOffset1, int rowOffset2, int columnOffset2)
        {
            int[][] indices = null;

            if (rowOffset2 > 0 || columnOffset2 > 0)
            {
                for (int i = 0; i < blocks.Count(); i++)
                {
                    var rowIndex = blocks[i].Row + rowOffset2;
                    var columnIndex = blocks[i].Column + columnOffset2;

                    // Check there are no collisions with "landed" tetrominoes during rotation
                    if (rowIndex >= TetrisGame.NumberOfRows)
                        return false;

                    if (rowIndex < TetrisGame.NumberOfRows
                        && columnIndex < TetrisGame.NumberOfColumns
                        && TetrisGame.Matrix[rowIndex, columnIndex].BlockType == BlockType.Landed)
                    {
                        return false;
                    }
                }
            }

            indices = GetPositionIndices(nextPosition);

            if (rowOffset1 > 0 || columnOffset1 > 0)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i][0] += rowOffset1;
                    indices[i][1] += columnOffset1;

                    // Wall kick from left wall
                    if (indices[i][1] < 0)
                        return SetPosition(nextPosition, rowOffset1, ++columnOffset1, rowOffset2, columnOffset2);

                    // Wall kick from right wall
                    if (indices[i][1] > 9)
                        return SetPosition(nextPosition, rowOffset1, --columnOffset1, rowOffset2, columnOffset2);
                }
            }

            ClearTetrominoBlocks();
            blocks.Clear();

            var block = new BlockMatrix(TetrominoColor, BlockType.Falling);

            TetrisGame.Matrix[indices[0][0], indices[0][1]] = block;
            blocks.Add(new BlockPosition() { Row = indices[0][0], Column = indices[0][1] });

            TetrisGame.Matrix[indices[1][0], indices[1][1]] = block;
            blocks.Add(new BlockPosition() { Row = indices[1][0], Column = indices[1][1] });

            TetrisGame.Matrix[indices[2][0], indices[2][1]] = block;
            blocks.Add(new BlockPosition() { Row = indices[2][0], Column = indices[2][1] });

            TetrisGame.Matrix[indices[3][0], indices[3][1]] = block;
            blocks.Add(new BlockPosition() { Row = indices[3][0], Column = indices[3][1] });

            return true;
        }

        internal bool CanAddNewTetromino()
        {
            for (int rowIndex = 0; rowIndex < 2; rowIndex++)
            {
                for (int columnIndex = 3; columnIndex < 7; columnIndex++)
                    if (TetrisGame.Matrix[rowIndex, columnIndex].BlockType != BlockType.Clear)
                        return false;
            }

            return true;
        }

        internal void MoveTetromino(TetrominoDirection direction)
        {
            int columnIndexOffset = 0;
            if (direction == TetrominoDirection.Left)
                columnIndexOffset = -1;
            else
                columnIndexOffset = 1;

            // Check if move allowed
            for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
            {
                var rowIndex = blocks[blockIndex].Row;
                var columnIndex = blocks[blockIndex].Column;

                if (columnIndex + columnIndexOffset < 0 || columnIndex + columnIndexOffset > TetrisGame.NumberOfColumns - 1)
                    return;

                if (TetrisGame.Matrix[rowIndex, columnIndex + columnIndexOffset].BlockType == BlockType.Landed)
                    return;
            }
            
            ClearTetrominoBlocks();

            // Set new positions as Falling
            for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
            {
                var rowIndex = blocks[blockIndex].Row;
                var columnIndex = blocks[blockIndex].Column;

                if (columnIndex + columnIndexOffset >= 0 && columnIndex + columnIndexOffset <= TetrisGame.NumberOfColumns - 1)
                {
                    blocks[blockIndex] = new BlockPosition()
                    {
                        Row = rowIndex,
                        Column = columnIndex + columnIndexOffset
                    };
                    TetrisGame.Matrix[rowIndex, columnIndex + columnIndexOffset] = new BlockMatrix(TetrominoColor, BlockType.Falling);
                }
            }
        }

        internal bool IsInLandedPosition()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                int rowIndexTetromino = blocks[i].Row;
                int columnIndexTetromino = blocks[i].Column;

                for (int rowIndex = rowIndexTetromino; rowIndex < TetrisGame.NumberOfRows; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < TetrisGame.NumberOfColumns; columnIndex++)
                    {
                        if (TetrisGame.Matrix[rowIndex, columnIndex].BlockType == BlockType.Falling
                            &&
                            (rowIndex == (TetrisGame.NumberOfRows - 1) // First tetromino falling on bottom
                            || TetrisGame.Matrix[rowIndex + 1, columnIndex].BlockType == BlockType.Landed) // Touching landed tetromino
                            )
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal void MarkAsLanded()
        {
            for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
                TetrisGame.Matrix[blocks[blockIndex].Row, blocks[blockIndex].Column] = new BlockMatrix(TetrominoColor, BlockType.Landed);
        }

        internal void Fall()
        {
            ClearTetrominoBlocks();
            int rowsToFallCount = 1;

            // Set new positions as Falling
            for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
            {
                var rowIndex = blocks[blockIndex].Row;
                var columnIndex = blocks[blockIndex].Column;

                if (rowIndex + rowsToFallCount < TetrisGame.NumberOfRows)
                {
                    blocks[blockIndex] = new BlockPosition()
                    {
                        Row = rowIndex + rowsToFallCount,
                        Column = columnIndex
                    };
                    TetrisGame.Matrix[rowIndex + rowsToFallCount, columnIndex] = new BlockMatrix(TetrominoColor, BlockType.Falling);
                }
            }
        }

        internal static PositionedTetromino CreateRandom()
        {
            Random rnd = new Random();
            var type = (TetrominoType)rnd.Next(0, 7);
            switch (type)
            {
                case TetrominoType.I:
                    return new TetrominoI();

                case TetrominoType.J:
                    return new TetrominoJ();

                case TetrominoType.L:
                    return new TetrominoL();

                case TetrominoType.O:
                    return new TetrominoO();

                case TetrominoType.S:
                    return new TetrominoS();

                case TetrominoType.T:
                    return new TetrominoT();

                case TetrominoType.Z:
                    return new TetrominoZ();
            }

            return null;
        }

        private void ClearTetrominoBlocks()
        {
            // Set old positions as Clear
            for (int blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
            {
                var rowIndex = blocks[blockIndex].Row;
                var columnIndex = blocks[blockIndex].Column;

                TetrisGame.Matrix[rowIndex, columnIndex] = new BlockMatrix(TetrisGame.TetrionBackgroundColor, BlockType.Clear);
            }
        }

        private TetrominoPosition GetNextPosition()
        {
            switch (currentPosition)
            {
                case TetrominoPosition.Initial:
                    return TetrominoPosition.Degree90;

                case TetrominoPosition.Degree90:
                    return TetrominoPosition.Degree180;

                case TetrominoPosition.Degree180:
                    return TetrominoPosition.Degree270;

                case TetrominoPosition.Degree270:
                    return TetrominoPosition.Initial;
            }

            return TetrominoPosition.Initial;
        }

        private void GetCurrentOffset(out int rowOffset1, out int columnOffset1, out int rowOffset2, out int columnOffset2)
        {
            var rowOffsets = blocks.Select(b => b.Row);
            rowOffset1 = rowOffsets.Min();
            rowOffset2 = 0;

            var columnOffsets = blocks.Select(b => b.Column);
            columnOffset1 = columnOffsets.Min();
            columnOffset2 = 0;

            // Offset of tetromino's 4x4 or 3x3 "box" needs to be adjusted
            switch (Type)
            {
                case TetrominoType.I:
                    switch (currentPosition)
                    {
                        case TetrominoPosition.Initial:
                            rowOffset1--;
                            rowOffset2 = 2;
                            break;

                        case TetrominoPosition.Degree90:
                            columnOffset1 -= 2;
                            columnOffset2 = 1;
                            break;

                        case TetrominoPosition.Degree180:
                            rowOffset1 -= 2;
                            rowOffset2 = 1;
                            break;

                        case TetrominoPosition.Degree270:
                            columnOffset1--;
                            columnOffset2 = 2;
                            break;
                    }
                    break;

                case TetrominoType.J:
                case TetrominoType.L:
                case TetrominoType.S:
                case TetrominoType.T:
                case TetrominoType.Z:

                    switch (currentPosition)
                    {
                        case TetrominoPosition.Initial:
                            rowOffset2 = 1;
                            break;

                        case TetrominoPosition.Degree90:
                            columnOffset1--;
                            break;

                        case TetrominoPosition.Degree180:
                            rowOffset1--;
                            break;

                        case TetrominoPosition.Degree270:
                            columnOffset2 = 1;
                            break;
                    }
                    break;

                case TetrominoType.O:
                    columnOffset1--;
                    rowOffset2 = 1;
                    columnOffset2 = 1;
                    break;
            }
        }
    }
}
