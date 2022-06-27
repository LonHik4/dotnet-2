using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using ReactiveUI;

namespace SudokuClient.ViewModels
{
    public class ScoreViewModel : BaseViewModel
    {
        public string Login => _client.Player.Login;

        public int Score => _client.Player.Score;

        private Client _client;

        public ReactiveCommand<Unit, Unit> PlayCommand { get; }

        public Interaction<Client, Unit> ShowPlayWindow { get; } = new();

        public Interaction<Unit, Unit> ShowLoginWindow { get; } = new();

        public ScoreViewModel(Client client)
        {
            _client = client;

            PlayCommand = ReactiveCommand.CreateFromTask(PlayImpl);
        }

        private async Task<Unit> PlayImpl()
        {
            try
            {
                _disposables.Add(_client.PLayStartEvent.Subscribe(async _ =>
                {
                    await ShowPlayWindow.Handle(_client);
                    DisposeSubscriptions();
                }));

                _disposables.Add(_client.DisconnectedEvent.Subscribe(async s =>
                {
                    await ShowErrorInteraction.Handle(s);
                    await ShowLoginWindow.Handle(Unit.Default);
                    DisposeSubscriptions();
                }));

                await _client.PlayRequest();

                return Unit.Default;
            }
            catch
            {
                return Unit.Default;
            }
        }

        private void DisposeSubscriptions()
        {
            foreach (var d in _disposables)
                d.Dispose();

            _disposables.Clear();
        }

        private readonly List<IDisposable> _disposables = new();
    }
}
