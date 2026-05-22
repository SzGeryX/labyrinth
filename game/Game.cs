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

namespace game
{
    class Game
    {
        public List<List<Tile>> Tiles;
        public int MapHeight { get => Tiles.Count; }
        public int MapWidth { get => Tiles[0].Count; }
        public Game(string name, UniformGrid map) 
        { 
            Tiles = readMapFile(name);
            initMap(map);

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
                    Label temp = new();

                    temp.Content = Tiles[i][j].Icon;

                    temp.Padding = new Thickness(0);
                    temp.Margin = new Thickness(0);

                    temp.FontFamily = new FontFamily("Consolas");

                    temp.FontSize = 150;

                    map.Children.Add(temp);

                    Grid.SetColumn(temp, j);
                    Grid.SetRow(temp, i);
                }
            }
        }
    }
}
