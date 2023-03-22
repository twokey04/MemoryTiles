using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MemoryTiles
{
    /// <summary>
    /// Interaction logic for Play.xaml
    /// </summary>
    public partial class Play : Window
    {
        int rows, columns;

        string[] tiles;

        System.Windows.Controls.Button previousButton = null;
        System.Windows.Controls.Button lastRoundButton = new System.Windows.Controls.Button();
        int pairsRevealed = 0;

        System.Windows.Controls.MenuItem standardMenuItem;
        System.Windows.Controls.MenuItem customMenuItem;

        Grid levelGrid;
        System.Windows.Controls.Label currentLevel;
        int currentLevelIndex;

        Grid userGrid;
        string playerName;
        Image playerimage;
        System.Windows.Controls.Label playerLabel;
        List<string> tmpImages;

        List<System.Windows.Controls.Button> buttonList= new List<System.Windows.Controls.Button>();

        private List<string> imagePaths = new List<string>()
            {
                "tiles/mrkrabs.png",
                "tiles/planckton_remote.png",
                "tiles/sandy.png",
                "tiles/spongebob_balloon.png",
                "tiles/spongebob_book.png",
                "tiles/spongebob_brain.png",
                "tiles/spongebob_meme.png",
                "tiles/spongebob_scream.png",
                "tiles/squidward.png",
                "tiles/squidward_sexy.png",
                "tiles/triplet.png",               
                "tiles/friends.png",               
                "tiles/saliva.png",               
                "tiles/run.png",               
                "tiles/fire.png",
                "tiles/patrick_dumb.png",
                "tiles/patrick_baby.png",
                "tiles/sp_patrick.png"
            };

        public Play(string playerName, List<string> buttonsContent, string[] configuration, int rows = 6, int columns = 6, int level = 1, int guessed = 0)
        {
            this.playerName = playerName;
            this.rows = rows;
            this.columns = columns;
            this.pairsRevealed = guessed;
            this.currentLevelIndex = level;


            InitializeComponent();

            gameGrid = GenerateMenu();
            gameGrid.Children.Add(GenerateButtons());
            windowGrid.Children.Add(gameGrid);
            windowGrid.Children.Add(GenerateLevelGrid());
            windowGrid.Children.Add(GenerateUserGrid());
            Content = windowGrid;

            tmpImages = new List<string>();
            if (buttonsContent != null)
                for (int i = 0; i < buttonsContent.Count(); i++)
                {
                    if (buttonsContent[i] == "?")
                    {
                        buttonList[i].Content = buttonsContent[i];
                    }
                    else
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(buttonsContent[i], UriKind.Relative));
                        buttonList[i].Content = image;
                        buttonList[i].IsEnabled = false;
                        imagePaths.Remove(buttonsContent[i]);
                        tmpImages.Add(buttonsContent[i]);
                    }

                }
            if (configuration == null)
                tiles = Enumerable.Range(0, rows * columns / 2)
                        .SelectMany(i => new[] { imagePaths[i], imagePaths[i] })
                        .OrderBy(i => Guid.NewGuid())
                        .ToArray();
            else
                tiles = configuration;

            UpdatePlayedGames(playerName);

            SpawnInCenterOfScreen();
        }

        

        private void SpawnInCenterOfScreen()
        {
            Screen screen = Screen.PrimaryScreen;
            System.Drawing.Rectangle workingArea = screen.WorkingArea;

            double left = workingArea.Left + (workingArea.Width - Width) / 2;
            double top = workingArea.Top + (workingArea.Height - Height) / 2;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = left;
            Top = top;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            if (button == lastRoundButton)
                return;

            int index = Grid.GetRow(button) * columns + Grid.GetColumn(button);
            string tile = tiles[index];

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(tile, UriKind.Relative));

            button.Content = image;
            button.IsEnabled = false;

            if (previousButton == null)
            {
                previousButton = new System.Windows.Controls.Button();
                previousButton = button;
                lastRoundButton = null;
            }
            if (previousButton != null && button != previousButton)
            {
                lastRoundButton = button;
                button.IsEnabled = false;
                previousButton.IsEnabled = false;
                int previousIndex = Grid.GetRow(previousButton) * columns + Grid.GetColumn(previousButton);
                string previousTile = tiles[previousIndex];
                if (tile == previousTile)
                {
                    pairsRevealed++;
                    if (pairsRevealed == rows * columns / 2)
                    {
                        currentLevelIndex += 1;
                        lastRoundButton = null;
                        await Task.Delay(TimeSpan.FromSeconds(0.5));

                        currentLevel.Content = $"Level: {currentLevelIndex}";
                        foreach (System.Windows.Controls.Button btn in buttonList)
                        {
                            btn.Content = "?";
                            btn.IsEnabled = true;
                        }
                        tiles = Enumerable.Range(0, rows * columns / 2)
                            .SelectMany(i => new[] { imagePaths[i], imagePaths[i] })
                            .OrderBy(i => Guid.NewGuid())
                            .ToArray();
                        pairsRevealed= 0;
                    }
                }
                else
                {

                    await Task.Delay(TimeSpan.FromSeconds(0.15));

                    previousButton.Content = "?";
                    previousButton.IsEnabled = true;
                    button.Content = "?";
                    button.IsEnabled = true;
                }
                previousButton = null;        
            }
            if (currentLevelIndex > 3)
            {
                currentLevelIndex = 3;
                currentLevel.Content = $"Level: {currentLevelIndex}";
                foreach (System.Windows.Controls.Button btn in buttonList)
                {
                    btn.IsEnabled = false;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load("../../users/users.xml");

                XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + playerName + "']");
                if (userNode != null)
                {
                    XmlNode savedgameNode = userNode.SelectSingleNode("savedgame");
                    if (savedgameNode != null)
                    {
                        userNode.RemoveChild(savedgameNode);
                    }
                }
                doc.Save("../../users/users.xml");
                UpdateWonGames(playerName);
                System.Windows.MessageBox.Show("You win!");
                return;
            }
        }

        private void newGameClicked(object sender, RoutedEventArgs e)
        {
            Window newGame;
            if (standardMenuItem.IsChecked && !customMenuItem.IsChecked)
            {
                newGame = new Play(playerName, null, null);                
            }
            else if (customMenuItem.IsChecked && !standardMenuItem.IsChecked)
            {
                CustomGame tmp = new CustomGame();
                tmp.ShowDialog();
                newGame = new Play(playerName, null, null, tmp.GetRows(), tmp.GetColumns());
            }
            else
            {
                newGame = new Play(playerName, null, null);               
            }
            Hide();
            newGame.Show();
            Close();
        }

        private void openGameClicked(object sender, RoutedEventArgs e)
        {
            List<string> buttonContents = new List<string>();
            int rows = 0, cols = 0, level = 1, guessed = 0;
            string[] configurationSaved = new string[imagePaths.Count];

            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + playerName + "']");
            if (userNode != null)
            {
                XmlNode matrixNode = userNode.SelectSingleNode("savedgame/matrix");
                if (matrixNode != null)
                {
                    rows = int.Parse(matrixNode.Attributes["rows"].Value);
                    cols = int.Parse(matrixNode.Attributes["cols"].Value);
                    level = int.Parse(matrixNode.Attributes["level"].Value);
                    guessed = int.Parse(matrixNode.Attributes["guessed"].Value);

                    foreach (XmlNode rowNode in matrixNode.ChildNodes)
                    {
                        foreach (XmlNode buttonNode in rowNode.ChildNodes)
                        {
                            buttonContents.Add(buttonNode.InnerText);
                        }
                    }

                }
                XmlNode configurationNode = userNode.SelectSingleNode("savedgame/configuration");
                if (configurationNode != null)
                {
                    int count = 0;
                    foreach (XmlNode tile in configurationNode.ChildNodes)
                    {
                        configurationSaved[count] = tile.InnerText;
                        count++;
                    }
                    
                }
                Play playWindow = new Play(playerName, buttonContents, configurationSaved, rows, cols, level, guessed);
                playWindow.Show();
                Close();
            }

        }

        private void saveGameClicked(object sender, RoutedEventArgs e)
        {
            if (previousButton == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("../../users/users.xml");

                XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + playerName + "']");
                if (userNode != null)
                {
                    XmlNode savedgameNode = userNode.SelectSingleNode("savedgame");
                    if (savedgameNode != null)
                    {
                        userNode.RemoveChild(savedgameNode);
                    }

                    savedgameNode = doc.CreateElement("savedgame");

                    XmlNode matrixNode = doc.CreateElement("matrix");
                    matrixNode.Attributes.Append(doc.CreateAttribute("rows")).Value = rows.ToString();
                    matrixNode.Attributes.Append(doc.CreateAttribute("cols")).Value = columns.ToString();
                    matrixNode.Attributes.Append(doc.CreateAttribute("level")).Value = currentLevelIndex.ToString();
                    matrixNode.Attributes.Append(doc.CreateAttribute("guessed")).Value = pairsRevealed.ToString();
                    for (int i = 0; i < rows; i++)
                    {
                        XmlNode rowNode = doc.CreateElement("row");
                        for (int j = 0; j < columns; j++)
                        {
                            XmlNode buttonNode = doc.CreateElement("button");
                            buttonNode.InnerText = buttonList[i * columns + j].Content.ToString() == "System.Windows.Controls.Image" ? tiles[i * columns + j] : "?";
                            rowNode.AppendChild(buttonNode);
                        }
                        matrixNode.AppendChild(rowNode);
                    }
                    savedgameNode.AppendChild(matrixNode);

                    XmlNode configNode = doc.CreateElement("configuration");
                    savedgameNode.AppendChild(configNode);

                    foreach (string s in tiles)
                    {
                        XmlNode stringNode = doc.CreateElement("tile");
                        stringNode.InnerText = s;
                        configNode.AppendChild(stringNode);
                    }

                    userNode.AppendChild(savedgameNode);

                    doc.Save("../../users/users.xml");
                }
            }
            else
                System.Windows.MessageBox.Show("Game cannot be saved yet");
        }

        private void statisticsClicked(object sender, RoutedEventArgs e)
        {
            Statistics statisticsWindow = new Statistics(playerName);
            statisticsWindow.ShowDialog();
        }

        private void exitClicked(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow();
            Hide();
            mainWindow.Show();
            Close();
        }

        private void aboutClicked(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }

        private Grid GenerateMenu()
        {
            Grid grid = new Grid();

            System.Windows.Controls.Menu menu = new System.Windows.Controls.Menu();

            // File menu item
            System.Windows.Controls.MenuItem fileMenuItem = new System.Windows.Controls.MenuItem();
            fileMenuItem.Header = "_File";

            // New game sub-menu item
            System.Windows.Controls.MenuItem newGameMenuItem = new System.Windows.Controls.MenuItem();
            newGameMenuItem.Header = "_New game";
            newGameMenuItem.Click += newGameClicked;

            // Open game sub-menu item
            System.Windows.Controls.MenuItem openGameMenuItem = new System.Windows.Controls.MenuItem();
            openGameMenuItem.Header = "_Open game";
            openGameMenuItem.Click += openGameClicked;

            // Save game sub-menu item
            System.Windows.Controls.MenuItem saveGameMenuItem = new System.Windows.Controls.MenuItem();
            saveGameMenuItem.Header = "_Save game";
            saveGameMenuItem.Click += saveGameClicked;

            // Statistics sub-menu item
            System.Windows.Controls.MenuItem statisticsMenuItem = new System.Windows.Controls.MenuItem();
            statisticsMenuItem.Header = "_Statistics";
            statisticsMenuItem.Click += statisticsClicked;

            // Separator sub-menu item
            Separator separatorMenuItem = new Separator();

            // Exit sub-menu item
            System.Windows.Controls.MenuItem exitMenuItem = new System.Windows.Controls.MenuItem();
            exitMenuItem.Header = "_Exit";
            exitMenuItem.Click += exitClicked;

            // Add sub-menu items to File menu item
            fileMenuItem.Items.Add(newGameMenuItem);
            fileMenuItem.Items.Add(openGameMenuItem);
            fileMenuItem.Items.Add(saveGameMenuItem);
            fileMenuItem.Items.Add(statisticsMenuItem);
            fileMenuItem.Items.Add(separatorMenuItem);
            fileMenuItem.Items.Add(exitMenuItem);

            // Options menu item
            System.Windows.Controls.MenuItem optionsMenuItem = new System.Windows.Controls.MenuItem();
            optionsMenuItem.Header = "_Options";

            // Standard sub-menu item
            standardMenuItem = new System.Windows.Controls.MenuItem();
            standardMenuItem.Header = "_Standard";
            standardMenuItem.IsCheckable = true;

            // Custom sub-menu item
            customMenuItem = new System.Windows.Controls.MenuItem();
            customMenuItem.Header = "_Custom";
            customMenuItem.IsCheckable = true;

            // Add sub-menu items to Options menu item
            optionsMenuItem.Items.Add(standardMenuItem);
            optionsMenuItem.Items.Add(customMenuItem);

            // Help menu item
            System.Windows.Controls.MenuItem helpMenuItem = new System.Windows.Controls.MenuItem();
            helpMenuItem.Header = "_Help";

            // About sub-menu item
            System.Windows.Controls.MenuItem aboutMenuItem = new System.Windows.Controls.MenuItem();
            aboutMenuItem.Header = "_About";
            aboutMenuItem.Click += aboutClicked;

            // Add sub-menu items to Help menu item
            helpMenuItem.Items.Add(aboutMenuItem);

            // Add menu items to menu
            menu.Items.Add(fileMenuItem);
            menu.Items.Add(optionsMenuItem);
            menu.Items.Add(helpMenuItem);

            // Add menu to grid
            grid.Children.Add(menu);

            // Return the grid
            return grid;
        }

        private Grid GenerateButtons()
        {
            Grid grid = new Grid();
            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
            for (int j = 0; j < columns; j++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                    button.Content = "?";
                    button.Margin = new Thickness(5);
                    button.Click += Button_Click;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    grid.Children.Add(button);
                    buttonList.Add(button);
                }
            }

            grid.Margin = new Thickness(0, 30, 200, 0);
            return grid;
        }

        private Grid GenerateLevelGrid()
        {
            currentLevel = new System.Windows.Controls.Label();
            currentLevel.Content = $"Level: {currentLevelIndex}";
            currentLevel.FontSize = 18;
            levelGrid = new Grid();
            levelGrid.Margin = new Thickness(650, 30, 0, 0);
            levelGrid.Children.Add(currentLevel);
            return levelGrid;
        }

        private Grid GenerateUserGrid()
        {
            userGrid = new Grid();
            userGrid.Margin = new Thickness(650, 120, 10, 0);
            playerLabel = new System.Windows.Controls.Label();
            playerLabel.Content = playerName;
            playerLabel.FontSize = 16;
            userGrid.Children.Add(playerLabel);

            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            XmlNode userNode = doc.SelectSingleNode($"/users/user[name='{playerName}']");

            string profilePicPath = "";

            if (userNode != null)
            {
                XmlNode profilePicNode = userNode.SelectSingleNode("profilepic");
                if (profilePicNode != null)
                {
                    profilePicPath = profilePicNode.InnerText;
                }
            }
            playerimage = new Image();
            playerimage.Source = new BitmapImage(new Uri(profilePicPath, UriKind.Relative));

            userGrid.Children.Add(playerimage);

            return userGrid;
        }

        private void UpdatePlayedGames(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            string tmp = "0";

            XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + name + "']");

            if (userNode != null)
            {
                XmlNode gamesPlayedNode = userNode.SelectSingleNode("gamesplayed");
                if (gamesPlayedNode != null)
                {
                    tmp = gamesPlayedNode.InnerText.ToString();
                    userNode.RemoveChild(gamesPlayedNode);
                }

                gamesPlayedNode = doc.CreateElement("gamesplayed");
                gamesPlayedNode.InnerText = (Convert.ToInt64(tmp.ToString()) + 1).ToString();

                userNode.AppendChild(gamesPlayedNode);

                doc.Save("../../users/users.xml");
            }
        }

        private void UpdateWonGames(string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            string tmp = "0";

            XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + name + "']");

            if (userNode != null)
            {
                XmlNode gamesWonNode = userNode.SelectSingleNode("gameswon");
                if (gamesWonNode != null)
                {
                    tmp = gamesWonNode.InnerText.ToString();
                    userNode.RemoveChild(gamesWonNode);
                }

                gamesWonNode = doc.CreateElement("gameswon");
                gamesWonNode.InnerText = (Convert.ToInt64(tmp.ToString()) + 1).ToString();

                userNode.AppendChild(gamesWonNode);

                doc.Save("../../users/users.xml");
            }
        }

    }
}
