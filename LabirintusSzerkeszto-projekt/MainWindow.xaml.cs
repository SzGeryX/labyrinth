using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

        private int palyaSzelesseg;
        private int palyaMagassag;
        private char[,] palya;

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

                    gomb.Click += PalyaMezoKattintas;
                    gomb.Tag = new Point(x, y);

                    PalyaGrid.Children.Add(gomb);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int szelesseg = int.Parse(szelessegBox.Text);
            int magassag = int.Parse(magassagBox.Text);

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if(sfd.ShowDialog() != true)
            {
                return;
            }

            sfd.Filter = "Text File (*.txt)|*.txt";

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
    }
}