using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TetrisLibrary;

namespace Meowtris
{
    public partial class MainWindow : Window
    {
        private Timer timer = null;
        private int interval = 1000;

        TetrisGame game;

        public MainWindow()
        {
            InitializeComponent();

            InitializeGrid();
            game = new TetrisGame();
            game.DrawMatrix += Game_DrawMatrix;
            game.EndGame += Game_EndGame;
            game.RefreshScore += Game_RefreshScore;
            game.StartGame();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                timer = new Timer(new TimerCallback(Tick), null, 0, interval);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void Tick(object state)
        {
            try
            {
                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        game.ProgressFallOneStep();
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Game_RefreshScore(int score)
        {
            try
            {
                if (score > 0)
                {
                    int.TryParse(lblScore.Content.ToString(), out int totalScore);
                    int num1 = totalScore / 10;

                    lblScore.Content = totalScore += score;

                    int num2 = totalScore / 10;
                    if (interval > 100 && num1 != num2)
                    {
                        interval -= 100;
                        timer.Change(0, interval);
                    }

                    // And of course, play meow! :)
                    Uri uri = new Uri(@"pack://application:,,,/Meowtris;component/Resources/meow.wav");
                    using (var stream = Application.GetResourceStream(uri).Stream)
                    {
                        SoundPlayer player = new SoundPlayer(stream);
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Game_EndGame()
        {
            try
            {
                lblMessage.Content = "Game over";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Game_DrawMatrix()
        {
            try
            {
                DrawGridFromMatrix();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DrawGridFromMatrix()
        {
            for (var rowIndex = 0; rowIndex < TetrisGame.NumberOfRows; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < TetrisGame.NumberOfColumns; columnIndex++)
                {
                    var rect = tetrisGrid.Children
                        .Cast<Rectangle>()
                        .First(e => Grid.GetRow(e) == rowIndex && Grid.GetColumn(e) == columnIndex);

                    rect.Fill = TetrisGame.Matrix[rowIndex, columnIndex].Color;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Left:
                        game.MoveTetromino(TetrominoDirection.Left);
                        DrawGridFromMatrix();
                        break;

                    case Key.Right:
                        game.MoveTetromino(TetrominoDirection.Right);
                        DrawGridFromMatrix();
                        break;

                    case Key.Down:
                        game.ProgressFallOneStep();
                        DrawGridFromMatrix();
                        break;

                    case Key.A:
                        game.RotateTetromino();
                        DrawGridFromMatrix();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetGrid();
                game.StartGame();

                lblMessage.Content = string.Empty;
                lblScore.Content = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AboutWindow windowAbout = new AboutWindow();
                windowAbout.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
