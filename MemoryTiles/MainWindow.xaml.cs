using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace MemoryTiles
{
    public partial class MainWindow : Window
    {
        private List<string> imagePaths = new List<string>()
            {
                "images/airplane.jpg",
                "images/astronaut.jpg",
                "images/ball.jpg",
                "images/beach.jpg",
                "images/butterfly.jpg",
                "images/car.jpg",
                "images/cat.jpg",
                "images/chess.jpg",
                "images/dirtbike.jpg",
                "images/dog.jpg",
                "images/drip.jpg",
                "images/duck.jpg",
                "images/fish.jpg",
                "images/frog.jpg",
                "images/guest.jpg",
                "images/guitar.jpg",
                "images/horses.jpg",
                "images/kick.jpg",
                "images/launch.jpg",
                "images/palmtree.jpg",
                "images/pinkflower.jpg",
                "images/redflower.jpg",
                "images/skater.jpg",
                "images/snowflake.jpg"
            };
        public MainWindow()
        {
            InitializeComponent();
            InitializeUserList();
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
        private void InitializeUserList()
        {
            buttonDeleteUser.IsEnabled = false;
            buttonPlay.IsEnabled = false;

            XmlDocument doc = new XmlDocument();
            doc.Load("../../users/users.xml");

            XmlNodeList docUsers = doc.GetElementsByTagName("name");

            foreach (XmlNode userNode in docUsers)
            {
                string username = userNode.InnerText;
                userList.Items.Add(username);
            }
        }

        private void buttonNewUser_Click(object sender, RoutedEventArgs e)
        {
            SignUp window = new SignUp();
            window.Show();
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(userList.SelectedItem != null)
            {
                buttonDeleteUser.IsEnabled = true;
                buttonPlay.IsEnabled = true;

                XmlDocument doc = new XmlDocument();
                doc.Load("../../users/users.xml");

                XmlNode userNode = doc.SelectSingleNode("/users/user[name='" + userList.SelectedItem.ToString() + "']");
                if (userNode != null)
                {
                    XmlNode pictureNode = userNode.SelectSingleNode("profilepic");
                    profilePicture.Source = new BitmapImage(new Uri(pictureNode.InnerText, UriKind.Relative));
                }
            }
        }

        private void buttonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            XDocument xmlDoc = XDocument.Load("../../users/users.xml");

            XElement nodeToDelete = xmlDoc.Descendants("user")
                                          .Where(x => (string)x.Element("name") == userList.SelectedItem.ToString())
                                          .FirstOrDefault();

            if (nodeToDelete != null)
            {
                nodeToDelete.Remove();
            }

            xmlDoc.Save("../../users/users.xml");
            userList.Items.Clear();
            profilePicture.Source = null;
            InitializeUserList();
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            Window playWindow = new Play(userList.SelectedItem.ToString(), null, null);
            playWindow.Show();
            Close();
        }
    }
}
