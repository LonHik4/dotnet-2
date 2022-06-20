using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SudokuClient.ViewModels
{
    public class ScoreViewModel: BaseViewModel
    {
        public string Login => _client.Player.Login;

        public int Score => _client.Player.Score;

        private Client _client;

        public ReactiveCommand<Unit, Unit> PlayCommand { get; }

        public Interaction<Client, Unit> ShowPlayWindow { get; } = new();

        public ScoreViewModel(Client client)
        {
            _client = client;

            PlayCommand = ReactiveCommand.CreateFromTask(PlayImpl);
        }

        private async Task<Unit> PlayImpl()
        {
            try
            {
                _client.PLayStartEvent.Subscribe(async _ => await ShowPlayWindow.Handle(_client));

                _client.DisconnectedEvent.Subscribe(async s => await ShowErrorInteraction.Handle(s));

                await _client.PlayRequest();

                return Unit.Default;
            }
            catch
            {
                return Unit.Default;
            }
        }



    }
}
