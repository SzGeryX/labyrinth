using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabirintusSzerkeszto_projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] elemek =
        {
            '.',
            '█',
            '╬',
            '═',
            '╦',
            '╩',
            '║',
            '╣',
            '╠',
            '╗',
            '╝',
            '╚', 
            '╔'
        };

        char[] balraNyit =
        {
            '═',
            '╬',
            '╣',
            '╗',
            '╝',
            '╦',
            '╩'
        };

        char[] jobbraNyit =
        {
            '═',
            '╬',
            '╠',
            '╔',
            '╚',
            '╦',
            '╩'
        };

        char[] felfeleNyit =
        {
            '║',
            '╬',
            '╣',
            '╠',
            '╚',
            '╝',
            '╩'
        };

        char[] lefeleNyit =
        {
            '║',
            '╬',
            '╣',
            '╠',
            '╔',
            '╗',
            '╦'
        };

        private int palyaSzelesseg;
        private int palyaMagassag;
        private char[,] palya;
        private bool angolNyelv = false;

        private char valasztottElem = '.';
        public MainWindow()
        {
            InitializeComponent();
            ValaszthatoElemekLetrehozasa();
        }

        private void ValaszthatoElemekLetrehozasa()
        {
            foreach (var elem in elemek)
            {
                Button gomb = new Button();
                gomb.Content = elem;

                gomb.Width = 30;
                gomb.Height = 30;
                gomb.FontSize = 20;

                gomb.Click += ElemKivalasztas;

                valaszthatoElemek.Children.Add(gomb);
            }
        }

        private void ElemKivalasztas(object sender, RoutedEventArgs e)
        {
            Button gomb = sender as Button;

            valasztottElem = Convert.ToChar(gomb.Content);
        }

        private void PalyaLetrehozasa(int szelesseg, int magassag)
        {
            palyaMagassag = magassag;
            palyaSzelesseg = szelesseg;

            palya = new char[magassag, szelesseg];

            for (int y = 0; y < magassag; y++)
            {
                for (int x = 0; x < szelesseg; x++)
                {
                    palya[y, x] = '.';
                }
            }
        }

        private void PalyaKirajzolasa()
        {
            PalyaGrid.Children.Clear();

            PalyaGrid.Rows = palyaMagassag;
            PalyaGrid.Columns = palyaSzelesseg;

            for (int y = 0;y < palyaMagassag; y++)
            {
                for (int x = 0;x < palyaSzelesseg; x++)
                {
                    Button gomb = new Button();

                    gomb.Content = palya[y, x];
                    gomb.Width = 30;
                    gomb.Height = 30;
                    gomb.FontSize = 20;
                    gomb.Margin = new Thickness(0);
                    gomb.Padding = new Thickness(0);
                    gomb.BorderThickness = new Thickness(0);

                    gomb.Click += PalyaMezoKattintas;
                    gomb.Tag = new Point(x, y);

                    PalyaGrid.Children.Add(gomb);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(szelessegBox.Text) || string.IsNullOrWhiteSpace(magassagBox.Text)) {
                MessageBox.Show(Szoveg("Add meg a pálya szélességét és magasságát!", "Please enter the map width and height!"));

                return;
            }

            if (!int.TryParse(szelessegBox.Text, out int szelesseg) || !int.TryParse(magassagBox.Text, out int magassag))
            {
                MessageBox.Show(Szoveg("A szélesség és magasság csak szám lehet!",
           "Width and height must be numbers!"));
                return;
            }

            PalyaLetrehozasa(szelesseg, magassag);
            PalyaKirajzolasa();
        }

        private void PalyaMezoKattintas(object sender, RoutedEventArgs e)
        {
            Button gomb = sender as Button;

            Point pozicio = (Point)gomb.Tag;

            int x = (int)pozicio.X;
            int y = (int)pozicio.Y;

            palya[y, x] = valasztottElem;

            gomb.Content = valasztottElem;
        }

        private void Mentes(object sender, RoutedEventArgs e)
        {

            if (!VanKincsesSzoba())
            {
                MessageBox.Show(Szoveg("Nincs kincses szoba!", "No treasure room found!"));
                return;
            }

            if (!VanKijarat())
            {
                MessageBox.Show(Szoveg("Nincs kijárat!", "No exit found!"));
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Text File (*.txt)|*.txt";

            if (sfd.ShowDialog() != true)
            {
                return;
            }

            List<string> sorok = new List<string>();

            for (int y = 0; y < palyaMagassag; y++)
            {

                string sor = "";
                for (int x = 0; x < palyaSzelesseg; x++)
                {
                    sor += palya[y, x];
                }

                sorok.Add(sor);
            }

            File.WriteAllLines(sfd.FileName, sorok);
        }

        private bool VanKincsesSzoba()
        {
            for (int y = 0; y < palyaMagassag; y++)
            {

                for (int x = 0; x < palyaSzelesseg; x++)
                {
                    if (palya[y, x] == '█')
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private bool VanKijarat()
        {
            
            for (int y = 0;y < palyaMagassag; y++)
            {
                if (balraNyit.Contains(palya[y, 0]))
                {
                    return true;
                }

                if (jobbraNyit.Contains(palya[y, palyaSzelesseg - 1]))
                {
                    return true;
                }
            }

            for (int x =0; x < palyaSzelesseg; x++)
            {
                if (felfeleNyit.Contains(palya[0, x]))
                {
                    return true;
                }

                if (lefeleNyit.Contains(palya[palyaMagassag - 1, x]))
                {
                    return true;
                }
            }

            return false;

        }

        private void PalyaBetoltese(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Text File (*.txt)|*.txt";

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            string[] sorok = File.ReadAllLines(ofd.FileName);

            palyaMagassag = sorok.Length;
            palyaSzelesseg = sorok[0].Length;

            palya = new char[palyaMagassag, palyaSzelesseg];

            for (int y = 0; y < palyaMagassag; y++)
            {
                for (int x = 0; x < palyaSzelesseg; x++)
                {
                    palya[y, x] = sorok[y][x];
                }
            }

            PalyaKirajzolasa();
        }

        private void NyelvValtas(object sender, RoutedEventArgs e)
        {
            angolNyelv = !angolNyelv;

            if (angolNyelv)
            {
                ujPalyaBtn.Content = "Create Map";
                mentesBtn.Content = "Save";
                palyaBetoltesBtn.Content = "Load";
                NyelvGomb.Content = "Magyar";

                szelessegLb.Content = "Width:";
                magassagLb.Content = "Height:";
            }
            else
            {
                ujPalyaBtn.Content = "Új pálya";
                mentesBtn.Content = "Mentés";
                palyaBetoltesBtn.Content = "Betöltés";
                NyelvGomb.Content = "English";

                szelessegLb.Content = "Szélesség:";
                magassagLb.Content = "Magasság:";
            }
        }

        private string Szoveg(string magyar, string angol)
        {
            return angolNyelv ? angol : magyar;
        }
    }
}