using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ReactiveUI;

using SudokuClient.ViewModels;

namespace SudokuClient.Views
{
    public class BaseWindow<TViewModel> : ReactiveWindow<TViewModel> where TViewModel : class 
    {
        protected BaseWindow()
        {
            this.WhenActivated(d =>
            {
                if (ViewModel is not BaseViewModel viewModel)
                    return;
                viewModel.ShowErrorInteraction.RegisterHandler(ShowError);
            });
        }

        private Task ShowError(InteractionContext<string, Unit> ctx)
        {
            MessageBox.Show(ctx.Input, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            ctx.SetOutput(Unit.Default);

            return Task.CompletedTask;
        }
    }
}
