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

namespace MemoryTiles
{
    /// <summary>
    /// Interaction logic for CustomGame.xaml
    /// </summary>
    public partial class CustomGame : Window
    {
        public CustomGame()
        {
            InitializeComponent();
            SpawnInCenterOfScreen();
        }

        public int GetRows()
        {
            try
            {
                return Convert.ToInt32(rowsBox.Text);
            }
            catch
            {
                return 6;
            }
        }

        public int GetColumns()
        {
            try
            {
                return Convert.ToInt32(columnsBox.Text);
            }
            catch
            {
                return 6;
            }
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

        private void playCustom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Int32 rows = Convert.ToInt32(rowsBox.Text);
                Int32 columns = Convert.ToInt32(columnsBox.Text);

                if ((rows * columns) % 2 != 0)
                {
                    warningBox.Content = "The number of rows multiplied by\nthe number of columns must be a multiple of 2";
                }
                else
                {
                    Close();
                    return;
                }
            }
            catch (FormatException)
            {
                warningBox.Content = "Please enter valid integer values for rows and columns";
                return;
            }
            catch (OverflowException)
            {
                warningBox.Content = "The entered values are too large to be converted to Int64";
                return;
            }
        }
    }
}
