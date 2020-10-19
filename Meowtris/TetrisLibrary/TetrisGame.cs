using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace TetrisLibrary
{
    public class TetrisGame
    {
        public const int NumberOfRows = 20;
        public const int NumberOfColumns = 10;
        public static readonly BlockMatrix[,] Matrix = new BlockMatrix[NumberOfRows, NumberOfColumns];
        private PositionedTetromino tetromino;
        public static Brush TetrionBackgroundColor = Brushes.Transparent;

        public delegate void DrawMatrixDel();
        public event DrawMatrixDel DrawMatrix;

        public delegate void EndGameDel();
        public event EndGameDel EndGame;

        public delegate void RefreshScoreDel(int score, int countSingleRows, int countDoubleRows, int countTripleRows, int countTetrises);
        public event RefreshScoreDel RefreshScore;
            
        public void InitializeMatrix()
        {
            for (var rowIndex = 0; rowIndex < NumberOfRows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                    Matrix[rowIndex, columnIndex] = new BlockMatrix(TetrionBackgroundColor, BlockType.Clear);
            }
        }

        public void StartGame()
        {
            InitializeMatrix();
            tetromino = PositionedTetromino.CreateRandom();
            tetromino.SetPosition(TetrominoPosition.Initial, 0, 3, 0, 0);
            DrawMatrix();
        }

        public void ProgressFallOneStep()
        {
            if (tetromino == null)
                return;

            bool canAddnewTetromino = true;
            bool isLanded = tetromino.IsInLandedPosition();

            if (isLanded == false)
                tetromino.Fall();
            else
            {
                tetromino.MarkAsLanded();
                int score = RemoveRowsCompletedUpdateScore(out int countSingleRows, out int countDoubleRows, out int countTripleRows, out int countTetrises);
                RefreshScore(score, countSingleRows, countDoubleRows, countTripleRows, countTetrises);

                canAddnewTetromino = tetromino.CanAddNewTetromino();
                if (canAddnewTetromino)
                {
                    tetromino = PositionedTetromino.CreateRandom();
                    tetromino.SetPosition(TetrominoPosition.Initial, 0, 3, 0, 0);
                }
                else
                    tetromino = null;
            }
            DrawMatrix();

            if (canAddnewTetromino == false)
                EndGame();
        }

        public void MoveTetromino(TetrominoDirection direction)
        {
            if (tetromino == null)
                return;

            tetromino.MoveTetromino(direction);
        }

        public void RotateTetromino()
        {
            if (tetromino == null)
                return;

            tetromino.RotateTetromino();
        }

        private int RemoveRowsCompletedUpdateScore(out int countSingleRows, out int countDoubleRows, out int countTripleRows, out int countTetrises)
        {
            countSingleRows = countDoubleRows = countTripleRows = countTetrises = 0;
            int score;
            List<int> completedRowsIndices = GetScoreCounts(out countSingleRows, out countDoubleRows, out countTripleRows, out countTetrises, out score);

            if (completedRowsIndices.Count == 0)
                return 0;

            // Remove all completed rows
            int counter = completedRowsIndices.Count - 1;
            for (int rowIndex = NumberOfRows - 1; rowIndex > 0 && counter >= 0; rowIndex--)
            {
                if (rowIndex == completedRowsIndices[counter])
                {
                    // Clear completed row
                    for (int columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                        Matrix[rowIndex, columnIndex] = new BlockMatrix(TetrionBackgroundColor, BlockType.Clear);

                    counter--;
                }
            }

            // Copy incomplete rows into clear rows
            for (var rowIndex = completedRowsIndices[0]; rowIndex < NumberOfRows; rowIndex++)
            {
                bool allBlocksInRowClear = AreAllBlocksInRowOfType(BlockType.Clear, rowIndex);
                if (allBlocksInRowClear)
                {
                    int copiedRowIndex = rowIndex - 1;
                    int targetRowIndex = rowIndex;
                    while (copiedRowIndex > 0 && IsAnyBlockOfTypeInRow(BlockType.Landed, copiedRowIndex))
                    {
                        // Copy
                        for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                            Matrix[targetRowIndex, columnIndex] = Matrix[copiedRowIndex, columnIndex];

                        // Delete
                        for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                            Matrix[copiedRowIndex, columnIndex] = new BlockMatrix(TetrionBackgroundColor, BlockType.Clear);

                        targetRowIndex = copiedRowIndex;
                        copiedRowIndex--;
                    }
                }
            }

            return score;
        }

        private bool IsAnyBlockOfTypeInRow(BlockType blockType, int rowIndex)
        {
            for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                if (Matrix[rowIndex, columnIndex].BlockType == blockType)
                    return true;

            return false;
        }

        private bool AreAllBlocksInRowOfType(BlockType blockType, int rowIndex)
        {
            for (var columnIndex = 0; columnIndex < NumberOfColumns; columnIndex++)
                if (Matrix[rowIndex, columnIndex].BlockType != blockType)
                    return false;

            return true;
        }

        private List<int> GetScoreCounts(out int countSingleRows, out int countDoubleRows, out int countTripleRows, out int countTetrises, out int totalScore)
        {
            countSingleRows = 0;
            countDoubleRows = 0;
            countTripleRows = 0;
            countTetrises = 0;
            totalScore = 0;
            List<int> completedRowsIndices = new List<int>();

            for (var rowIndex = 0; rowIndex < NumberOfRows; rowIndex++)
            {
                bool allBlocksInRowLanded = AreAllBlocksInRowOfType(BlockType.Landed, rowIndex);
                if (allBlocksInRowLanded)
                    completedRowsIndices.Add(rowIndex);
            }

            int counter = 0;
            while (counter < completedRowsIndices.Count)
            {
                int currentIndex = completedRowsIndices[counter];
                int numberOfConsecutiveRows = 0;
                while (counter < completedRowsIndices.Count && completedRowsIndices[counter] == currentIndex)
                {
                    counter++;
                    currentIndex++;
                    numberOfConsecutiveRows++;
                }

                int count = numberOfConsecutiveRows / 4;
                if (count > 0)
                    countTetrises += count;
                int remainder = numberOfConsecutiveRows % 4;

                count = remainder / 3;
                if (count > 0)
                    countTripleRows += count;
                remainder %= 3;

                count = remainder / 2;
                if (count > 0)
                    countDoubleRows += count;
                remainder %= 2;

                countSingleRows += remainder;
            }

            totalScore = countSingleRows * 40 + countDoubleRows * 100 + countTripleRows * 300 + countTetrises * 1200;
            return completedRowsIndices;
        }
    }
}
