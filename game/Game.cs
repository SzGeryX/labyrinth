using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Printing;

namespace game
{
    class Game
    {
        Random rnd = new Random();

        private List<List<Tile>> tiles;
        private Point player;
        private List<Tile> exits;
        public int MapHeight { get => tiles.Count; }
        public int MapWidth { get => tiles[0].Count; }
        private Grid gameArea;
        private int score = 0;


        public Game(string name, UniformGrid map, Grid gameArea)
        {
            tiles = readMapFile(name);

            initMap(map);

            exits = calculateExits();

            Tile rndExit = exits[rnd.Next(exits.Count())];

            player = new Point(Grid.GetColumn(rndExit.Label), Grid.GetRow(rndExit.Label));
            this.gameArea = gameArea;

            colorTileBackgrounds();
        }

        private List<Tile> calculateExits()
        {
            List<Tile> exits = new List<Tile>();

            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (tiles[i][j].Directions.Count == 0) continue;

                    if (i == 0 && tiles[i][j].Directions.Contains(game.Directions.North)) exits.Add(tiles[i][j]);
                    if (i == MapHeight - 1 && tiles[i][j].Directions.Contains(game.Directions.South)) exits.Add(tiles[i][j]);
                    if (j == 0 && tiles[i][j].Directions.Contains(game.Directions.West)) exits.Add(tiles[i][j]);
                    if (j == MapWidth - 1 && tiles[i][j].Directions.Contains(game.Directions.East)) exits.Add(tiles[i][j]);
                }
            }

            return exits;
        }

        private List<List<Tile>> readMapFile(string name)
        {
            List<List<Tile>> tiles = new();

            List<string> file = File.ReadAllLines(name).ToList();

            foreach (string line in file)
            {
                List<Tile> temp = new List<Tile>();
                char[] parts = line.ToCharArray();

                foreach (char c in parts)
                {
                    temp.Add(new Tile(c));
                }

                tiles.Add(temp);
            }

            return tiles;
        }

        private void initMap(UniformGrid map)
        {
            map.Columns = MapWidth;
            map.Rows = MapHeight;


            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    Label current = new Label();
                    
                    tiles[i][j].Label = current;

                    current.Visibility = Visibility.Hidden;

                    current.Content = tiles[i][j].Icon;

                    current.Padding = new Thickness(0);
                    current.Margin = new Thickness(0);

                    current.FontFamily = new FontFamily("Consolas");

                    current.FontSize = 150;

                    map.Children.Add(current);

                    Grid.SetColumn(current, j);
                    Grid.SetRow(current, i);
                }
            }
        }

        private void colorTileBackgrounds()
        {
            foreach (List<Tile> row in tiles)
            {
                foreach (Tile col in row)
                {
                    if (col.Label == null) throw new Exception("Tile without label");

                    if (Grid.GetColumn(col.Label) == player.X && Grid.GetRow(col.Label) == player.Y)
                    {
                        col.Label.Background = Brushes.IndianRed;
                        col.IsDiscovered = true;
                        col.Label.Visibility = Visibility.Visible;
                    }

                    if (col.IsDiscovered && !(Grid.GetColumn(col.Label) == player.X && Grid.GetRow(col.Label) == player.Y))
                    {
                        col.Label.Visibility = Visibility.Visible;
                        col.Label.Background = Brushes.Gray;
                    }
                }
            }
        }

        public void MovePlayer(Key k)
        {
            if (player.X - 1 < 0 && k == Key.A ||
                player.X + 1 >= MapWidth && k == Key.D ||
                player.Y - 1 < 0 && k == Key.W ||
                player.Y + 1 >= MapHeight && k == Key.S)
            {
                if (!exits.Contains(tiles[(int)(player.Y)][(int)(player.X)])) return;

                switch (k)
                {
                    case Key.A:
                        if (!tiles[(int)(player.Y)][(int)(player.X)].Directions.Contains(game.Directions.West)) return;
                        break;
                    case Key.D:
                        if (!tiles[(int)(player.Y)][(int)(player.X)].Directions.Contains(game.Directions.East)) return;
                        break;
                    case Key.W:
                        if (!tiles[(int)(player.Y)][(int)(player.X)].Directions.Contains(game.Directions.North)) return;
                        break;
                    case Key.S:
                        if (!tiles[(int)(player.Y)][(int)(player.X)].Directions.Contains(game.Directions.South)) return;
                        break;
                }     

                string messageBoxText = "Do you want to exit the maze?";
                string caption = "Exit";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.No);

                return;
            }

            switch (k)
            {
                case Key.A:
                    if (!tiles[(int)(player.Y)][(int)(player.X - 1)].Directions.Contains(game.Directions.East)) return;
                    player.X -= 1;
                    break;
                case Key.D:
                    if (!tiles[(int)(player.Y)][(int)(player.X + 1)].Directions.Contains(game.Directions.West)) return;
                    player.X += 1;
                    break;
                case Key.W:
                    if (!tiles[(int)(player.Y - 1)][(int)(player.X)].Directions.Contains(game.Directions.South)) return;
                    player.Y -= 1;
                    break;
                case Key.S:
                    if (!tiles[(int)(player.Y + 1)][(int)(player.X)].Directions.Contains(game.Directions.North)) return;
                    player.Y += 1;
                    break;
            }     

            if (tiles[(int)(player.Y)][(int)(player.X)].IsScore && !tiles[(int)(player.Y)][(int)(player.X)].IsScored)
            {
                tiles[(int)(player.Y)][(int)(player.X)].IsScored = true;
                score += 1;

                (gameArea.Children[0] as Label).Content = score;
            }

            colorTileBackgrounds();
        }
    }
}
