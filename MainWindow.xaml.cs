using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board board;
        Difficulty currentDifficulty;
        Button btnRestart = new Button();
        public MainWindow()
        {
            InitializeComponent();

            Image btnRestartImage = new Image();
            btnRestartImage.Source = new BitmapImage(new Uri("img/face_unpressed.png", UriKind.Relative));
            btnRestart.Content = btnRestartImage;
            btnRestart.Click += btnRestart_Click;
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            spMenu.Visibility = Visibility.Hidden;
            Button? button = sender as Button;
            switch (button.Content.ToString())
            {
                case "Beginner":
                    board = new Board(gridMain, Difficulty.Beginner, btnRestart);
                    currentDifficulty = Difficulty.Beginner;
                    break;
                case "Intermediate":
                    board = new Board(gridMain, Difficulty.Intermediate, btnRestart);
                    currentDifficulty = Difficulty.Intermediate;
                    break;
                case "Expert":
                    board = new Board(gridMain, Difficulty.Expert, btnRestart);
                    currentDifficulty = Difficulty.Expert;
                    break;
                default:
                    spMenu.Visibility = Visibility.Visible;
                    gridMain.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            gridMain.Children.Clear();
            gridMain.RowDefinitions.Clear();
            gridMain.ColumnDefinitions.Clear();
            board = new Board(gridMain, currentDifficulty, btnRestart);
        }
    }
}
