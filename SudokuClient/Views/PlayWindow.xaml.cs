using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ReactiveUI;

using Sudoku;

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
            //this.WhenActivated(d =>
            //{
            //    if (ViewModel is not PlayViewModel viewModel)
            //        return;
            //    viewModel.ServerResponse.RegisterHandler(ChangePoint);
            //});
        }

        //private IObservable<Unit> ChangePoint(InteractionContext<Point, Unit> ctx)
        //{
        //    return Observable.Start(() =>
        //    {
        //        var viewModel = new PlayViewModel(ctx.Input);
        //        var window = new PlayWindow { ViewModel = viewModel };
        //        window.Show();

        //        Close();

        //        ctx.SetOutput(Unit.Default);
        //    }, RxApp.MainThreadScheduler);

        //}

        private async void textChangedEventHandler(object sender, TextChangedEventArgs args) // sender is TextBox.DataContext
        {
            var textBox = sender as TextBox;
            var d = args.Source as TextBox;

            var test = d.GetBindingExpression(TextBox.TextProperty).DataItem as Point;
            //ViewModel.TryChange(test, "5);

            //if (!ViewModel.TryChange(test))
            //{
            //    var point = textBox.DataContext as Point;
            //    test.Value =  point.Value;
            //}

            textBox.Text =  (await ViewModel.TryChange(test)).ToString();

            Console.WriteLine($"{args.OriginalSource.GetType()}");
        }
    }
}
