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

namespace game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game game;
        public MainWindow()
        {
            InitializeComponent();

            game = new("lab.txt", gridLabyrinth);
        }

        private void gridLabyrinth_KeyDown(object sender, KeyEventArgs e)
        {
            if (new List<Key>([Key.W, Key.A, Key.S, Key.D]).Contains(e.Key))
            {
                game.MovePlayer(e.Key);
            }
        }
    }
}