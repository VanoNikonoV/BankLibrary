using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bank.View
{
    /// <summary>
    /// Логика взаимодействия для NewClientWindow.xaml
    /// </summary>
    public partial class NewClientWindow : Window
    {
        public NewClientWindow()
        {
            InitializeComponent();
        }
        
        private bool isFocused = false;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e) { isFocused = true;}

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (isFocused)
            {
                isFocused = false;
                (sender as TextBox).SelectAll();
            }
        }

        /// <summary>
        /// Перемещение окна по рабочему столу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) { this.DragMove(); }
        }
    }
}
