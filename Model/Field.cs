using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper.Model
{
    internal class Field
    {
        public const int btnSize = 40;

        public const int TYPE_VALUE = 2;
        public const int NUMBER_VALUE = 1;
        public const int STATUS_VALUE = 0;

        //status
        public const int UNREVEALED = 0;
        public const int REVEALED = 1;
        public const int FLAGGED = 2;

        //type
        public const int EMPTY = 0;
        public const int NUMBER = 1;
        public const int MINE = 2;
        private int[] info = new int[3];
        private Image btnImage = new Image();
        public int[] Info { get { return info; } set { info = value; } }

        private Button button = new Button();
        public Button Button { get { return button; } set { button = value; } }

        public Field(Grid gridMain, int row, int column, int boardWidth)
        {
            info[STATUS_VALUE] = UNREVEALED;

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            gridMain.Children.Add(button);

            button.Width = button.Height = btnSize;
            button.FontWeight = FontWeights.Bold;
            button.FontSize = 18;

            btnImage.Source = new BitmapImage(new Uri("img/closed.png", UriKind.Relative));
            button.Content = btnImage;
            button.Uid = $"{(row - 2)* boardWidth + column}";
        }

        public void SetImage(string name)
        {
            btnImage.Source = new BitmapImage(new Uri($"img/{name}.png", UriKind.Relative));
            button.Content = btnImage;
        }

    }
}
