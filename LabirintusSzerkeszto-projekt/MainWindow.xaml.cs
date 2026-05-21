using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                

                valaszthatoElemek.Children.Add(gomb);
            }
        }
    }
}