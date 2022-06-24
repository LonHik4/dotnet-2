using System.Reactive;

using ReactiveUI;


namespace SudokuClient.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        public Interaction<string, Unit> ShowErrorInteraction { get; } = new();
    }
}
