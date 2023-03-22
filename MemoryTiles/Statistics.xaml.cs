using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Xml.Linq;
using System.Xml;

namespace MemoryTiles
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        public Statistics(string playerName)
        {
            InitializeComponent();

            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + playerName + "']");

            usernameBox.Content = $"Username: {playerName}";

            if (userNode != null)
            {
                XmlNode gamesPlayedNode = userNode.SelectSingleNode("gamesplayed");
                if (gamesPlayedNode != null)
                {
                    gamesPlayedBox.Content = $"Games played: {gamesPlayedNode.InnerText.ToString()}";
                }
                else
                {
                    gamesPlayedBox.Content = "Games played: 0";
                }
                XmlNode gamesWonNode = userNode.SelectSingleNode("gameswon");
                if(gamesWonNode != null)
                {
                    gamesWonBox.Content = $"Games won: {gamesWonNode.InnerText.ToString()}";
                }
                else
                {
                    gamesWonBox.Content = "Games won: 0";
                }
            }

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

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
