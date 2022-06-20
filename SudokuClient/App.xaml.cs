using System;
using System.Reactive;
using System.Windows;

using SudokuClient.ViewModels;

using ReactiveUI;
using SudokuClient.Views;

namespace SudokuClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new LoginWindow { ViewModel = new LoginViewModel() };
            Current.MainWindow = mainWindow;

            mainWindow.Show();
        }
    }
}
