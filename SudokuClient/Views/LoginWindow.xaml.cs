using System;
using System.Reactive;
using System.Reactive.Linq;

using ReactiveUI;

using SudokuClient.ViewModels;

namespace SudokuClient.Views
{
    // https://github.com/reactiveui/ReactiveUI/issues/2330#issuecomment-577968613
    public class LoginWindowBase : BaseWindow<LoginViewModel>
    {
    }

    public partial class LoginWindow : LoginWindowBase
    {
        public LoginWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                if (ViewModel is not LoginViewModel viewModel)
                    return;
                viewModel.ShowScoreWindow.RegisterHandler(ShowScore);
            });
        }

        private IObservable<Unit> ShowScore(InteractionContext<Client, Unit> ctx)
        {
            return Observable.Start(() =>
            {
                var viewModel = new ScoreViewModel(ctx.Input);
                var window = new ScoreWindow { ViewModel = viewModel };
                window.Show();

                Close();

                ctx.SetOutput(Unit.Default);
            }, RxApp.MainThreadScheduler);

        }
    }
}
