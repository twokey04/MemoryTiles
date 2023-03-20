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

namespace MemoryTiles
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            aboutLabel.Content = "Nume: Cojocaru Bogdan-Andrei\n\n\nGrupa: 10LF311\n\n\nSpecializarea: Informatica Aplicata";
            aboutLabel.FontSize = 24;
            aboutLabel.VerticalAlignment = VerticalAlignment.Center;
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
    }
}
