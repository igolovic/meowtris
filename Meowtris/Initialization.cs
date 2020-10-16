using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TetrisLibrary;

namespace Meowtris
{
    public partial class MainWindow
    {
        private void InitializeGrid()
        {
            int counterRowIndex = 0;
            while (counterRowIndex < TetrisGame.NumberOfRows)
            {
                tetrisGrid.RowDefinitions.Add(new RowDefinition()); ;
                counterRowIndex++;
            }

            int counterColIndex = 0;
            while (counterColIndex < TetrisGame.NumberOfColumns)
            {
                tetrisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                counterColIndex++;
            }

            for (var rowIndex = 0; rowIndex < TetrisGame.NumberOfRows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < TetrisGame.NumberOfColumns; columnIndex++)
                {
                    Rectangle rect = new Rectangle();
                    Grid.SetColumn(rect, columnIndex);
                    Grid.SetRow(rect, rowIndex);

                    tetrisGrid.Children.Add(rect);
                }
            }
        }

        private void ResetGrid()
        {
            for (var rowIndex = 0; rowIndex < TetrisGame.NumberOfRows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < TetrisGame.NumberOfColumns; columnIndex++)
                {
                    var rect = tetrisGrid.Children
                        .Cast<Rectangle>()
                        .First(e => Grid.GetRow(e) == rowIndex && Grid.GetColumn(e) == columnIndex);

                    rect.Fill = TetrisGame.TetrionBackgroundColor;
                }
            }
        }
    }
}
