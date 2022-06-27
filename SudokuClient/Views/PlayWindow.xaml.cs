using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

using ReactiveUI;

using SudokuClient.ViewModels;

namespace SudokuClient.Views
{
    // https://github.com/reactiveui/ReactiveUI/issues/2330#issuecomment-577968613
    public class PlayWindowBase : BaseWindow<PlayViewModel>
    {
    }

    public partial class PlayWindow : PlayWindowBase
    {
        public PlayWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                if (ViewModel is not PlayViewModel viewModel)
                    return;
                viewModel.ShowWinInteraction.RegisterHandler(ShowWin);
                viewModel.ShowLoginWindow.RegisterHandler(ShowLogin);
            });
        }

        private IObservable<Unit> ShowWin(InteractionContext<Unit, Unit> ctx)
        {
            return Observable.Start(() =>
            {
                var window = new LoginWindow { ViewModel = new LoginViewModel() };
                window.Show();
                Close();
                MessageBox.Show("You won!", "Victory", MessageBoxButton.OK, MessageBoxImage.Information);

                ctx.SetOutput(Unit.Default);
            }, RxApp.MainThreadScheduler);

        }

        private async void textChangedEventHandler(object sender, TextChangedEventArgs args) // sender is TextBox.DataContext
        {
            var textBox = sender as TextBox;
            var d = args.Source as TextBox;

            var test = d.GetBindingExpression(TextBox.TextProperty).DataItem as Sudoku.Point;


            if (textBox.Text != String.Empty)
            {
                int valueFromTextBox = int.Parse(textBox.Text);

                if (valueFromTextBox < 0 || valueFromTextBox > 9)
                {
                    textBox.Text = "";
                    return;
                }
            }

            textBox.TextChanged -= textChangedEventHandler;
            textBox.Text = (await ViewModel.TryChange(test)).ToString();
            textBox.TextChanged += textChangedEventHandler;

            Console.WriteLine("change");
        }

        private IObservable<Unit> ShowLogin(InteractionContext<Unit, Unit> ctx)
        {
            return Observable.Start(() =>
            {
                var window = new LoginWindow { ViewModel = new LoginViewModel() };
                window.Show();

                Close();

                ctx.SetOutput(Unit.Default);
            }, RxApp.MainThreadScheduler);
        }
    }
}
