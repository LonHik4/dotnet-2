using System;
using System.Reactive;
using System.Reactive.Linq;

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
                viewModel.ShowLoginWindow.RegisterHandler(ShowLogin);
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
