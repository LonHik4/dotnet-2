using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ReactiveUI;

using SudokuClient.ViewModels;

namespace SudokuClient.Views
{
    // https://github.com/reactiveui/ReactiveUI/issues/2330#issuecomment-577968613
    public class ScoreWindowBase : BaseWindow<ScoreViewModel>
    {
    }

    public partial class ScoreWindow : ScoreWindowBase
    {
        public ScoreWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                if (ViewModel is not ScoreViewModel viewModel)
                    return;
                viewModel.ShowPlayWindow.RegisterHandler(ShowPlay);
            });
        }

        private IObservable<Unit> ShowPlay(InteractionContext<Client, Unit> ctx)
        {
            return Observable.Start(() =>
            {
                var viewModel = new PlayViewModel(ctx.Input); 
                var window = new PlayWindow { ViewModel = viewModel };
                window.Show();

                Close();

                ctx.SetOutput(Unit.Default);
            }, RxApp.MainThreadScheduler);

        }
    }
}
