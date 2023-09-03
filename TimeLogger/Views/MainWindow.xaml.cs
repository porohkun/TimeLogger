using System;
using System.Windows;

namespace TimeLogger.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EventCommand_OnAction(object? sender, EventArgs e)
        {
            Show();
            Activate();
        }
    }
}
