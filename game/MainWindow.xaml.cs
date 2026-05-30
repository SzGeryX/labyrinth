using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game? game;
        private bool language = true;

        private LanguagePack hungarianLanguagePack = JsonSerializer.Deserialize<LanguagePack>(File.ReadAllText("magyar.json"));
        private LanguagePack englishLanguagePack = JsonSerializer.Deserialize<LanguagePack>(File.ReadAllText("english.json"));
        
        public MainWindow()
        {
            InitializeComponent();
            
            setLanguage();
            
            Console.WriteLine("started");
        }

        private void setLanguage()
        {
            LanguagePack currentLanguage = language ? englishLanguagePack : hungarianLanguagePack;
            
            btnOpen.Content = currentLanguage.btnOpenText; 
            btnSave.Content = currentLanguage.btnSaveText; 
            btnChangeLanguage.Content = currentLanguage.btnChangeLanguageText; 
            btnToggleMap.Content = game == null ? currentLanguage.btnToggleMapShowText : game.IsMapShown ? currentLanguage.btnToggleMapHideText : currentLanguage.btnToggleMapShowText; 
            lblScore.Content = currentLanguage.lblScoreText; 
            lblCoordinates.Content = currentLanguage.lblCoordinatesText; 
            lblDiscovered.Content = currentLanguage.lblDiscoveredText; 
            lblTime.Content = currentLanguage.lblTimeText; 
        }

        private void gridLabyrinth_KeyDown(object sender, KeyEventArgs e)
        {
            if (game == null) return;   
            
            if (new List<Key>([Key.W, Key.A, Key.S, Key.D]).Contains(e.Key))
            {
                game.MovePlayer(e.Key);
            }
        }

        private void BtnOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt|Save files (*.SAV*)|*.SAV*";
            ofd.RestoreDirectory = true;
            
            if (ofd.ShowDialog() != true) return; 
            
            lblMapName.Content = ofd.SafeFileName;

            game?.Dispose();
            game = new Game(ofd.FileName, grdGameArea);
        }

        private void BtnChangeLanguage_OnClick(object sender, RoutedEventArgs e)
        {
            language = !language;
            
            setLanguage();

        }

        private void BtnToggleMap_OnClick(object sender, RoutedEventArgs e)
        {
            if (game == null) return;
            
            game.IsMapShown = !game.IsMapShown;
            
            setLanguage();
            
            game.ColorTileBackgrounds();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Save files (*.SAV)|*.SAV";
            sfd.DefaultExt = ".SAV";
            
            if (sfd.ShowDialog() != true) return;
            
            File.WriteAllBytes(sfd.FileName, game.SaveGame().ToArray());
        }
    }
}