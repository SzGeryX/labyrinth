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

namespace game
{
    class Game
    {
        private List<List<Tile>> tiles;
        private Point player;
        public int MapHeight { get => tiles.Count; }
        public int MapWidth { get => tiles[0].Count; }


        public Game(string name, UniformGrid map)
        {
            tiles = readMapFile(name);
            player = new Point(0, 1);

            initMap(map);

            colorTileBackgrounds();
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
            switch (k)
            {
                case Key.A:
                    if (player.X - 1 < 0) return;
                    if (!tiles[(int)(player.Y)][(int)(player.X - 1)].Directions.Contains(game.Directions.East)) return;
                    player.X -= 1;
                    break;
                case Key.D:
                    if (player.X + 1 >= MapWidth) return;
                    if (!tiles[(int)(player.Y)][(int)(player.X + 1)].Directions.Contains(game.Directions.West)) return;
                    player.X += 1;
                    break;
                case Key.W:
                    if (player.Y - 1 < 0) return;
                    if (!tiles[(int)(player.Y - 1)][(int)(player.X)].Directions.Contains(game.Directions.South)) return;
                    player.Y -= 1;
                    break;
                case Key.S:
                    if (player.Y + 1 >= MapHeight) return;
                    if (!tiles[(int)(player.Y + 1)][(int)(player.X)].Directions.Contains(game.Directions.North)) return;
                    player.Y += 1;
                    break;
            }     

            colorTileBackgrounds();
        }
    }
}
