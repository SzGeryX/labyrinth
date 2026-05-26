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
            // ?
            // pld: string poz = "4:12"; 
            return unavailables;
        }
        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            return null;
        }


        static void Main(string[] args)
        {
            char[,] map =
            {
                {'.','.','.','.','.','╔','═','═','═','═','═','═','╗','.','.','.','.','.','.','.','.','.','.'},
                {'═','═','═','═','╗','║','.','.','.','.','.','.','║','.','.','.','.','.','.','█','.','.','.'},
                {'.','╔','═','═','╬','╝','.','.','╔','═','═','═','╣','.','.','.','╗','.','.','║','.','.','.'},
                {'.','║','.','.','║','.','.','.','║','.','.','.','╚','═','╦','═','╝','╚','╦','╝','.','.','.'},
                {'.','╚','╦','═','╩','═','═','═','╣','.','.','.','.','.','║','.','.','.','║','.','.','.','.'},
                {'.','.','╚','═','═','.','.','.','║','.','.','.','.','.','╚','═','═','═','╝','.','.','.','.'}
            };
            Console.WriteLine("Hello World!");
            Console.WriteLine(GetRoomNumber(map));
            Console.WriteLine(GetSuitableEntrance(map));
            Console.WriteLine(IsInvalidElement(map));
        }
    }
}
