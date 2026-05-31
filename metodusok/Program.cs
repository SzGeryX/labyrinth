namespace metodusok
{
    class Program
    {
        /// <summary>
        /// Megadja, hogy hány termet tartamaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>
        static int GetRoomNumber(char[,] map)
        {
            if (map == null)
            {
                return 0;
            }

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int count = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (map[row, col] == '█')
                    {
                        count++;
                    }
                }
            }

            return count;
        }
        /// <summary>
        /// A kapott térkép széleit végignézve megállapítja, hogy hány kijárat van.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        static int GetSuitableEntrance(char[,] map)
        {
            if (map == null)
            {
                return 0;
            }

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            int count = 0;

            // Felső és alsó sor
            for (int col = 0; col < cols; col++)
            {
                if (map[0, col] == '║')
                {
                    count++;
                }

                if (map[rows - 1, col] == '║')
                {
                    count++;
                }
            }

            // Bal és jobb oszlop
            for (int row = 0; row < rows; row++)
            {
                if (map[row, 0] == '═')
                {
                    count++;
                }

                if (map[row, cols - 1] == '═')
                {
                    count++;
                }
            }

            return count;
        }
        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        static bool IsInvalidElement(char[,] map)
        {
            HashSet<char> elemek =
            [
                '.', '█', '╬', '═', '╦', '╩', '║', '╣', '╠', '╗', '╝', '╚', '╔'
            ];

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (!elemek.Contains(map[row, col]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez egyetlen szomszéd pozícióból sem lehet eljutni.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista elemeiként
        static List<string> GetUnavailableElements(char[,] map)
        {
            List<string> unavailables = new List<string>();

            int rows = map.GetLength(0);
            int cols = map.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    char current = map[row, col];

                    if (current == '.')
                        continue;

                    bool connected = false;

                    // FEL
                    if (row > 0 &&
                        OpenUp(current) &&
                        OpenDown(map[row - 1, col]))
                    {
                        connected = true;
                    }

                    // LE
                    if (row < rows - 1 &&
                        OpenDown(current) &&
                        OpenUp(map[row + 1, col]))
                    {
                        connected = true;
                    }

                    // BAL
                    if (col > 0 &&
                        OpenLeft(current) &&
                        OpenRight(map[row, col - 1]))
                    {
                        connected = true;
                    }

                    // JOBB
                    if (col < cols - 1 &&
                        OpenRight(current) &&
                        OpenLeft(map[row, col + 1]))
                    {
                        connected = true;
                    }

                    if (!connected)
                    {
                        unavailables.Add($"{row}:{col}");
                    }
                }
            }

            return unavailables;
        }

        static bool OpenUp(char c)
        {
            return c == '║' || c == '╬' || c == '█' ||
                   c == '╣' || c == '╠' || c == '╝' ||
                   c == '╚' || c == '╩';
        }

        static bool OpenDown(char c)
        {
            return c == '║' || c == '╬' || c == '█' ||
                   c == '╣' || c == '╠' || c == '╗' ||
                   c == '╔' || c == '╦';
        }

        static bool OpenLeft(char c)
        {
            return c == '═' || c == '╬' || c == '╩' ||
                   c == '╦' || c == '╣' || c == '╗' ||
                   c == '╝' || c == '█';
        }

        static bool OpenRight(char c)
        {
            return c == '═' || c == '╬' || c == '╩' ||
                   c == '╦' || c == '╠' || c == '╔' ||
                   c == '╚' || c == '█';
        }
        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            int maxRow = 0;
            int maxCol = 0;

            HashSet<string> positions = new();

            foreach (string pos in positionsList)
            {
                string[] parts = pos.Split(':');

                int row = int.Parse(parts[0]);
                int col = int.Parse(parts[1]);

                positions.Add(pos);

                maxRow = Math.Max(maxRow, row);
                maxCol = Math.Max(maxCol, col);
            }

            char[,] map = new char[maxRow + 1, maxCol + 1];

            for (int r = 0; r <= maxRow; r++)
            {
                for (int c = 0; c <= maxCol; c++)
                {
                    map[r, c] = '.';
                }
            }

            foreach (string pos in positions)
            {
                string[] parts = pos.Split(':');

                int row = int.Parse(parts[0]);
                int col = int.Parse(parts[1]);

                bool up =
                    positions.Contains($"{row - 1}:{col}");

                bool down =
                    positions.Contains($"{row + 1}:{col}");

                bool left =
                    positions.Contains($"{row}:{col - 1}");

                bool right =
                    positions.Contains($"{row}:{col + 1}");

                map[row, col] =
                    GetCharacter(up, down, left, right);
            }

            return map;
        }

        static char GetCharacter(
            bool up,
            bool down,
            bool left,
            bool right)
        {
            if (up && down && left && right)
                return '╬';

            if (up && down && left)
                return '╣';

            if (up && down && right)
                return '╠';

            if (left && right && up)
                return '╩';

            if (left && right && down)
                return '╦';

            if (left && right)
                return '═';

            if (up && down)
                return '║';

            if (down && right)
                return '╔';

            if (down && left)
                return '╗';

            if (up && right)
                return '╚';

            if (up && left)
                return '╝';

            if (up || down)
                return '║';

            if (left || right)
                return '═';

            return '█';
        }

        static void PrintMap(char[,] md)
        {
            int rows = md.GetLength(0);
            int cols = md.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Console.Write(md[row, col]);
                }

                Console.WriteLine();
            }
        }
        static void Main(string[] args)
        {
            char[,] map =
            {
                {'.','.','.','.','.','╔','═','═','═','═','═','═','╗','.','.','.','.','█','.','.','.','.','.'},
                {'═','═','═','═','╗','║','.','.','.','.','.','.','║','.','.','.','.','.','.','█','.','.','.'},
                {'.','╔','═','═','╬','╝','.','.','╔','═','═','═','╣','.','.','.','╗','.','.','║','.','.','.'},
                {'.','║','.','.','║','.','.','.','║','.','.','.','╚','═','╦','═','╝','╚','╦','╝','.','.','.'},
                {'.','╚','╦','═','╩','═','═','═','╣','.','.','.','.','.','║','.','.','.','║','.','.','.','║'},
                {'.','.','╚','═','═','.','.','.','║','.','.','█','.','.','╚','═','═','═','╝','.','.','.','.'}
            };
            Console.WriteLine(GetRoomNumber(map));
            Console.WriteLine(GetSuitableEntrance(map));
            Console.WriteLine(IsInvalidElement(map));
            GetUnavailableElements(map).ForEach(x => Console.WriteLine(x));
            List<string> positionsList =
                [
                    "1:1","1:2","1:3","1:4","1:5","1:6",
                
                    "2:1",
                    "2:6",
                
                    "3:1","3:2","3:3","3:4",
                    "3:6",
                
                    "4:4",
                    "4:6",
                
                    "5:2","5:3","5:4","5:5","5:6",
                
                    "6:2",
                    "6:5",
                
                    "7:2","7:3","7:4","7:5",
                
                    "8:5",
                
                    "9:1","9:2","9:3","9:4","9:5","9:6","9:7"
                ];

            char[,] asd = GenerateLabyrinth(positionsList);
            PrintMap(asd);
        }
    }
}
