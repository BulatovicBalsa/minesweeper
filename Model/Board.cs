using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Schema;

namespace Minesweeper.Model
{
    enum Difficulty
    {
        Beginner, Intermediate, Expert
    }
    internal class Board
    {
        private Field[] fields;
        Button dummyButtonLeft = new Button();
        Button dummyButtonRight = new Button();
        Stopwatch stopwatch = new Stopwatch();
        DispatcherTimer timer = new DispatcherTimer();

        public Field[] Fields { get { return fields; } set { fields = value; } }

        private int numberOfMines;

        public int NumberOfMines { get { return numberOfMines; } set { numberOfMines = value; } }

        public int width;

        public int height;
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }


        public Board(Grid gridMain, Difficulty difficulty, Button btnRestart) {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            gridMain.Visibility = System.Windows.Visibility.Visible;
            dummyButtonLeft.FontWeight = dummyButtonRight.FontWeight = FontWeights.Bold;
            dummyButtonLeft.FontSize = dummyButtonRight.FontSize = 45;

            switch (difficulty)
            {
                case Difficulty.Beginner:
                    width = height = 9;
                    numberOfMines = 10;
                    break;
                case Difficulty.Intermediate:
                    width = height = 16;
                    numberOfMines = 40;
                    break;
                case Difficulty.Expert:
                    width = 30;
                    height = 16;
                    numberOfMines = 99;
                    break;
            }
            fields = new Field[width*height];

            for (int i = 0; i < width; i++)
            {
                gridMain.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < height + 2; i++)
            {
                gridMain.RowDefinitions.Add(new RowDefinition());
            }
            gridMain.Width = width * Field.btnSize;
            gridMain.Height = (height + 2) * Field.btnSize;

            dummyButtonLeft.Focusable = false;
            dummyButtonRight.Focusable = false;
            dummyButtonLeft.Background = dummyButtonRight.Background = btnRestart.Background = Brushes.Transparent;
            btnRestart.BorderThickness = dummyButtonRight.BorderThickness = dummyButtonLeft.BorderThickness = new Thickness(0);

            Grid.SetRowSpan(dummyButtonLeft, 2);
            Grid.SetRow(dummyButtonLeft, 0);
            Grid.SetColumnSpan(dummyButtonLeft, width/2-1);
            Grid.SetColumn(dummyButtonLeft, 0);
            gridMain.Children.Add(dummyButtonLeft);

            Grid.SetRowSpan(btnRestart, 2);
            Grid.SetRow(btnRestart, 0);
            Grid.SetColumnSpan(btnRestart, 2 + width % 2);
            Grid.SetColumn(btnRestart, width/2-1);
            gridMain.Children.Add(btnRestart);

            Grid.SetRowSpan(dummyButtonRight, 2);
            Grid.SetRow(dummyButtonRight, 0);
            Grid.SetColumnSpan(dummyButtonRight, width / 2 - 1);
            Grid.SetColumn(dummyButtonRight, width/2+1 + width % 2);
            gridMain.Children.Add(dummyButtonRight);

            dummyButtonLeft.Height = dummyButtonRight.Height = btnRestart.Height = 2*Field.btnSize;
            dummyButtonLeft.Content = this.numberOfMines;


            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    fields[i*width+j] = new Field(gridMain, i+2, j, width);
                    fields[i * width + j].Button.MouseRightButtonUp += Button_MouseRightButtonUp;
                    fields[i * width + j].Button.AddHandler(FrameworkElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), true);
                }
            }
            
            SetMineFields();
            SetNumberFields();

            stopwatch.Start();

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            dummyButtonRight.Content = Convert.ToInt32(stopwatch.Elapsed.TotalSeconds);
        }

        private void SetNumberFields()
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Field field = fields[i];
                if (field.Info[Field.TYPE_VALUE] == Field.MINE) continue;
                HashSet<Field> nearFields = GetNearFields(i);
                foreach (Field f in nearFields)
                {
                    if (f.Info[Field.TYPE_VALUE] == Field.MINE)
                        field.Info[Field.NUMBER_VALUE]++;
                }
                if (field.Info[Field.NUMBER_VALUE] != 0)
                    field.Info[Field.TYPE_VALUE] = Field.NUMBER;
                //field.Button.Content = field.Info[Field.NUMBER_VALUE];

            }
        }

        private HashSet<Field> GetNearFields(int i)
        {
            HashSet<Field> nearFields = new HashSet<Field>();
            int row = 0, column = 0;
            if (i != 0)
            {
                row = i / width;
                column = i % width;
            }

            #pragma warning disable CS8601 // Possible null reference assignment.
            Field[] candidates = new Field[] 
            { GetField(row + 1, column), GetField(row, column + 1), GetField(row - 1, column),
              GetField(row, column - 1), GetField(row + 1, column + 1), GetField(row - 1, column - 1),
              GetField(row + 1, column - 1), GetField(row - 1, column + 1) 
            };

            #pragma warning restore CS8601 // Possible null reference assignment.
            foreach (Field field in candidates)
            {
               if (field != null) nearFields.Add(field);
            }
            return nearFields;
        }

        private Field? GetField(int row, int column)
        {
            try
            {
                if (row < 0 || column < 0) return null;
                if (row >= height || column >= width) return null;
                return fields[row * width + column];
            }
            catch (Exception)
            {

                return null;
            }
        }

        private void SetMineFields()
        {
            HashSet<int> minePositions = GetMinePositions();
            foreach (int position in minePositions)
            {
                fields[position].Info[Field.TYPE_VALUE] = Field.MINE;
                //fields[position].Button.Content = "M";
            }
        }

        private HashSet<int> GetMinePositions()
        {
            HashSet<int> positions = new HashSet<int>();
            Random random = new Random();
            while(positions.Count < numberOfMines)
            {
                positions.Add(random.Next(0, fields.Length));
            }
            return positions;
        }

        private int FieldToIndex(Field field)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (field == fields[i]) return i;
            }
            return -1;
        }

        public void RevealConnectedEmptyFields(Field emptyField)
        {
            List<Field> fieldsToReveal = new List<Field>();
            fieldsToReveal.Add(emptyField);

            while(fieldsToReveal.Count > 0)
            {
                Field field = fieldsToReveal.First();
                foreach (Field adjacentField in GetNearFields(FieldToIndex(field)))
                {
                    if (adjacentField == null) continue;
                    if (adjacentField.Info[Field.TYPE_VALUE] == Field.EMPTY && adjacentField.Info[Field.STATUS_VALUE] == Field.UNREVEALED)
                    {
                        fieldsToReveal.Add(adjacentField);
                        adjacentField.Info[Field.STATUS_VALUE] = Field.REVEALED;
                        adjacentField.SetImage("pressed");
                    }
                    if (adjacentField.Info[Field.TYPE_VALUE] == Field.NUMBER_VALUE)
                    {
                        adjacentField.Info[Field.STATUS_VALUE] = Field.REVEALED;
                        adjacentField.Button.Content = adjacentField.Info[Field.NUMBER];
                    }
                }
                fieldsToReveal.Remove(field);
            }
        }

        private void Button_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int position = Convert.ToInt32(clickedButton.Uid);
            Field field = fields[position];
            if (field.Info[Field.STATUS_VALUE] != Field.UNREVEALED) { return; }
            field.Info[Field.STATUS_VALUE] = Field.REVEALED;
            switch (field.Info[Field.TYPE_VALUE])
            {
                case Field.EMPTY:
                    field.SetImage("pressed");
                    RevealConnectedEmptyFields(field);
                    break;
                case Field.MINE:
                    field.SetImage("mine_red");
                    GameOver(field);
                    break;
                case Field.NUMBER:
                    clickedButton.Content = field.Info[Field.NUMBER_VALUE];
                    return;
                default:
                    throw new Exception();
            }
        }

        private void GameOver(Field lastClicked)
        {
            foreach (var field in fields)
            {
                if (field.Info[Field.TYPE_VALUE] == Field.MINE)
                {
                    if (field.Info[Field.STATUS_VALUE] != Field.FLAGGED && field != lastClicked)
                    {
                        field.SetImage("mine");
                    }
                }
                if (field.Info[Field.TYPE_VALUE] == Field.NUMBER)
                {
                    field.Button.Content = field.Info[Field.NUMBER_VALUE];
                }
                if (field.Info[Field.TYPE_VALUE] == Field.EMPTY)
                {
                    field.SetImage("pressed");
                }
            }
        }

        private void Button_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int position = Convert.ToInt32(clickedButton.Uid);
            Field field = fields[position];
            if (field.Info[Field.STATUS_VALUE] == Field.REVEALED) { return; }
            if (field.Info[Field.STATUS_VALUE] == Field.FLAGGED)
            {
                field.SetImage("closed");
                field.Info[Field.STATUS_VALUE] = Field.UNREVEALED;
                this.numberOfMines++;
                dummyButtonLeft.Content = this.numberOfMines;
                return;
            }
            field.SetImage("flag");
            field.Info[Field.STATUS_VALUE] = Field.FLAGGED;
            this.numberOfMines--;
            dummyButtonLeft.Content = this.numberOfMines;
        }

        private void PaintEmptyFields()
        {
            foreach (var item in fields)
            {
                if (item.Info[Field.TYPE_VALUE] == Field.EMPTY)
                {
                    item.Button.Background = Brushes.Red;
                    item.Button.Content = "";
                }
            }
        }
    }
}
