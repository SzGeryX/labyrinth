using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace game
{
    class Game
    {
        
        Random rnd = new Random();

        private List<List<Tile>> tiles;
        private Point player;
        private List<Tile> exits;
        private int MapHeight { get => tiles.Count; }
        private int MapWidth { get => tiles[0].Count; }
        private Grid gameArea;
        private int totalNOTiles;
        private int discoveredNOTiles
        {
            get { return tiles.Select(x => x.Count(y => y.IsDiscovered)).Sum(); }
        }

        private int[] extraTime;

        private bool IsEnded;

        private DispatcherTimer? dp;
        private Stopwatch sw;
        
        private Dictionary<string, Label> labels;
        private Dictionary<string, StackPanel> scoreboards;

        private Dictionary<char, byte> charToBin = new()
        {
            { ' ', 0b0000_0000 },
            { '█', 0b0000_0001 },
            { '╬', 0b0000_0010 },
            { '═', 0b0000_0011 },
            { '╦', 0b0000_0100 },
            { '╩', 0b0000_0101 },
            { '║', 0b0000_0110 },
            { '╣', 0b0000_0111 },
            { '╠', 0b0000_1000 },
            { '╗', 0b0000_1001 },
            { '╝', 0b0000_1010 },
            { '╚', 0b0000_1011 },
            { '╔', 0b0000_1100 }
        };
        
        private Dictionary<byte, char> binToChar = new()
        {
            { 0b0000_0000, ' ' },
            { 0b0000_0001, '█' },
            { 0b0000_0010, '╬' },
            { 0b0000_0011, '═' },
            { 0b0000_0100, '╦' },
            { 0b0000_0101, '╩' },
            { 0b0000_0110, '║' },
            { 0b0000_0111, '╣' },
            { 0b0000_1000, '╠' },
            { 0b0000_1001, '╗' },
            { 0b0000_1010, '╝' },
            { 0b0000_1011, '╚' },
            { 0b0000_1100, '╔' }
        };

        private byte scoredMask = 0b0001_0000;
        private byte discoveredMask = 0b0010_0000;
        private byte iconMask = 0b0000_1111;

        public int Score { get; private set; }
        
        public bool ShowMap { get; set; }


        public Game(string name, Grid gameArea)
        {
            this.gameArea = gameArea;
            extraTime = new int[] {0, 0, 0,};
            initMap(gameArea.FindName("grdLabyrinth") as UniformGrid, name);

            labels = new()
            {
                { "finaldiscoveredlbl", gameArea.FindName("lblFinalDiscovered") as Label },
                { "finalscorelbl", gameArea.FindName("lblFinalScore") as Label },
                { "finaltimelbl", gameArea.FindName("lblFinalTime") as Label },
                { "discoveredlbl", gameArea.FindName("lblDiscovered") as Label },
                { "scorelbl", gameArea.FindName("lblScore") as Label },
                { "coordinateslbl", gameArea.FindName("lblCoordinates") as Label },
                { "timelbl", gameArea.FindName("lblTime") as Label },
                { "mapnamelbl", gameArea.FindName("lblMapName") as Label },
            };

            scoreboards = new()
            {
                { "scoreboard", gameArea.FindName("spScoreBoard") as StackPanel },
                { "finalscoreboard", gameArea.FindName("spFinalScoreBoard") as StackPanel }
            };

            Tile rndExit = exits[rnd.Next(exits.Count())];
            player = new Point(Grid.GetColumn(rndExit.Label), Grid.GetRow(rndExit.Label));
            IsEnded = false;
            totalNOTiles = tiles.Select(x => x.Count(y => y.Icon != ' ')).Sum();
            sw = new Stopwatch();

            Score = 0;
            ShowMap = false;

            
            colorTileBackgrounds();
            updateInfo();
        }

        private List<Tile> calculateExits()
        {
            List<Tile> exits = new List<Tile>();

            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    if (tiles[i][j].Directions.Count == 0) continue;

                    if (i == 0 && tiles[i][j].Directions.Contains(Directions.North)) exits.Add(tiles[i][j]);
                    if (i == MapHeight - 1 && tiles[i][j].Directions.Contains(Directions.South)) exits.Add(tiles[i][j]);
                    if (j == 0 && tiles[i][j].Directions.Contains(Directions.West)) exits.Add(tiles[i][j]);
                    if (j == MapWidth - 1 && tiles[i][j].Directions.Contains(Directions.East)) exits.Add(tiles[i][j]);
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

        public List<List<Tile>> readSaveFile(string name)
        {
            List<List<Tile>> tiles = new();

            List<byte> file = File.ReadAllBytes(name).ToList();
            
            if (file.FindAll(x => x == (byte)0xFF).Count() != 1) throw new Exception("Broken savefile!");

            int eofMap = file.FindIndex(x => x == 0xFF);


            List<byte> map = file.Take(eofMap).ToList();
            
            int height, width, m, s, ms;
            
            height = BitConverter.ToInt32(file.GetRange(eofMap + 1, 4).ToArray());
            
            width = BitConverter.ToInt32(file.GetRange(eofMap + 5, 4).ToArray());
            
            extraTime[0] = BitConverter.ToInt32(file.GetRange(eofMap + 9, 4).ToArray());
            extraTime[1] = BitConverter.ToInt32(file.GetRange(eofMap + 13, 4).ToArray());
            extraTime[2] = BitConverter.ToInt32(file.GetRange(eofMap + 17, 4).ToArray());

            for (int i = 0; i < height; i++)
            {
                List<Tile> temp = new List<Tile>();
                for (int j = 0; j < width; j++)
                {
                    byte current = map[i * width + j];
                    char icon = binToChar[(byte)(current & iconMask)];
                    Console.WriteLine(icon);
                    temp.Add(new Tile(icon));
                    
                    temp[temp.Count - 1].IsDiscovered = (current & discoveredMask) == discoveredMask;
                    temp[temp.Count - 1].IsScored = (current & scoredMask) == scoredMask;
                }
                
                tiles.Add(temp);
            }

            return tiles;
        }

        private void endGame()
        {
            sw.Stop();
            dp.Stop();
            dp.Tick -= updateTimer;
            
            IsEnded = true;
            
            labels["finaldiscoveredlbl"].Content = labels["discoveredlbl"].Content;
            labels["finaltimelbl"].Content = labels["timelbl"].Content;
            labels["finalscorelbl"].Content = labels["scorelbl"].Content;

            labels["mapnamelbl"].Visibility = Visibility.Collapsed;
            scoreboards["scoreboard"].Visibility = Visibility.Collapsed;
            scoreboards["finalscoreboard"].Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            if (dp == null) return;
            
            (gameArea.FindName("grdLabyrinth") as UniformGrid).Children.Clear();
            Console.WriteLine("dp stopped ");
            dp.Stop();
            dp.Tick -= updateTimer;
            sw.Reset();
            
            labels["mapnamelbl"].Visibility = Visibility.Visible;
            scoreboards["scoreboard"].Visibility = Visibility.Visible;
            scoreboards["finalscoreboard"].Visibility = Visibility.Collapsed;
        }

        private void initMap(UniformGrid map, string name)
        {
            tiles = name.Split('.').Last() == "SAV" ? readSaveFile(name) : readMapFile(name);
            
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
            
            exits = calculateExits();
        }

        public void colorTileBackgrounds()
        {
            foreach (List<Tile> row in tiles)
            {
                foreach (Tile col in row)
                {
                    if (col.Label == null) throw new Exception("Tile without label");

                    if (col.IsDiscovered || ShowMap)
                    {
                        col.Label.Visibility = Visibility.Visible;
                        col.Label.Background = Brushes.Gray;
                    }
                    
                    if (Grid.GetColumn(col.Label) == (int)player.X && Grid.GetRow(col.Label) == (int)player.Y)
                    {
                        col.Label.Background = Brushes.IndianRed;
                        col.IsDiscovered = true;
                        col.Label.Visibility = Visibility.Visible;
                    }

                    if (col.IsDiscovered == false && ShowMap == false)
                    {
                        col.Label.Visibility = Visibility.Hidden;
                    }

                }
            }
        }

        private bool exitMaze(Tile currentTile, Key k)
        {
            if (!exits.Contains(currentTile)) return false;

            switch (k)
            {
                case Key.A:
                    if (!currentTile.Directions.Contains(Directions.West)) return false;
                    break;
                case Key.D:
                    if (!currentTile.Directions.Contains(Directions.East)) return false;
                    break;
                case Key.W:
                    if (!currentTile.Directions.Contains(Directions.North)) return false;
                    break;
                case Key.S:
                    if (!currentTile.Directions.Contains(Directions.South)) return false;
                    break;
            }     

            string messageBoxText = "Do you want to exit the maze?";
            string caption = "Exit";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;

            return MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.No) == MessageBoxResult.Yes;
        }

        private bool boundsCheckMove(Key k)
        {
            return
                player.X - 1 < 0 && k == Key.A ||
                player.X + 1 >= MapWidth && k == Key.D ||
                player.Y - 1 < 0 && k == Key.W ||
                player.Y + 1 >= MapHeight && k == Key.S;
        }

        private void updateTimer(object sender, EventArgs e)
        {
            TimeSpan ts = sw.Elapsed;
            labels["timelbl"].Content = labels["timelbl"].Content.ToString().Split(':')[0] + $": {ts.Minutes}:{ts.Seconds}:{ts.Milliseconds}" ;
        }

        public void MovePlayer(Key k)
        {
            if (IsEnded) return;
            if (dp == null)
            {
                dp = new DispatcherTimer();
                sw.Start();

                dp.Tick += new EventHandler(updateTimer);
                dp.Interval = new TimeSpan(0, 0, 0, 0,100);
                dp.Start();
            }
            
            Tile currentTile = tiles[(int)(player.Y)][(int)(player.X)];
            
            if (boundsCheckMove(k))
            {
                if (exitMaze(currentTile, k)) endGame();
                return;
            }

            switch (k)
            {
                case Key.A:
                    if (!tiles[(int)(player.Y)][(int)(player.X - 1)].Directions.Contains(Directions.East)) return;
                    player.X -= 1;
                    break;
                case Key.D:
                    if (!tiles[(int)(player.Y)][(int)(player.X + 1)].Directions.Contains(Directions.West)) return;
                    player.X += 1;
                    break;
                case Key.W:
                    if (!tiles[(int)(player.Y - 1)][(int)(player.X)].Directions.Contains(Directions.South)) return;
                    player.Y -= 1;
                    break;
                case Key.S:
                    if (!tiles[(int)(player.Y + 1)][(int)(player.X)].Directions.Contains(Directions.North)) return;
                    player.Y += 1;
                    break;
            }     
            
            currentTile = tiles[(int)(player.Y)][(int)(player.X)];

            if (currentTile.IsScore && !currentTile.IsScored)
            {
                currentTile.IsScored = true;
                Score += 1;
            }
            
            colorTileBackgrounds();
            
            colorArrows(currentTile);
            updateInfo();
        }

        private void updateInfo()
        {
            
            labels["discoveredlbl"].Content = labels["discoveredlbl"].Content.ToString().Split(':')[0] + ": " + Math.Round(Convert.ToDouble(discoveredNOTiles) / totalNOTiles * 100, 2) + "%";
            labels["scorelbl"].Content = labels["scorelbl"].Content.ToString().Split(':')[0] + ": " + Score;
            labels["coordinateslbl"].Content = labels["coordinateslbl"].Content.ToString().Split(':')[0] + ": " + $"X: {player.X}, Y: {player.Y}";
        }

        private void colorArrows(Tile currentTile)
        {
            (gameArea.FindName("lblNorthArrow") as Label).Foreground = currentTile.Directions.Contains(Directions.North) ?  Brushes.Green : Brushes.Black;
            (gameArea.FindName("lblSouthArrow") as Label).Foreground = currentTile.Directions.Contains(Directions.South) ?  Brushes.Green : Brushes.Black;
            (gameArea.FindName("lblWestArrow") as Label).Foreground = currentTile.Directions.Contains(Directions.West) ?  Brushes.Green : Brushes.Black;
            (gameArea.FindName("lblEastArrow") as Label).Foreground = currentTile.Directions.Contains(Directions.East) ?  Brushes.Green : Brushes.Black;
        }


        public List<byte> SaveGame()
        {
            List<byte> stream = new List<byte>();

            foreach (List<Tile> i in tiles)
            {
                foreach (Tile j in i)
                {
                    byte tile = charToBin[j.Icon];

                    tile = j.IsDiscovered ? (byte)(discoveredMask | tile) : tile;
                    tile = j.IsScored ? (byte)(scoredMask | tile) : tile;
                    stream.Add(tile);
                }
            }
            
            stream.Add(0b1111_1111);
            
            stream.Add((byte)MapHeight);
            stream.Add((byte)(MapHeight>>8));
            stream.Add((byte)(MapHeight>>16));
            stream.Add((byte)(MapHeight>>24));
            
            
            stream.Add((byte)MapWidth);
            stream.Add((byte)(MapWidth>>8));
            stream.Add((byte)(MapWidth>>16));
            stream.Add((byte)(MapWidth>>24));
            

            TimeSpan ts = sw.Elapsed;
            stream.AddRange(BitConverter.GetBytes(ts.Minutes));
            stream.AddRange(BitConverter.GetBytes(ts.Seconds));
            stream.AddRange(BitConverter.GetBytes(ts.Milliseconds));

            return stream;
        }
    }
}