using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace SudokuClient.ViewModels

{
    public class LoginViewModel : BaseViewModel
    {
        [Reactive]
        public string Login { get; set; } = string.Empty;

        [Reactive]
        public string ServerAddress { get; set; } = "http://127.0.0.1";

        [Reactive]
        public int ServerPort { get; set; } = 5000;

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        public Interaction<Client, Unit> ShowScoreWindow { get; } = new();

        public LoginViewModel()
        {
            var canLogin = this
                .WhenAnyValue(
                    o => o.ServerAddress,
                    o => o.Login,
                    (serverAddress, login) =>
                        !string.IsNullOrWhiteSpace(serverAddress) &&
                        !string.IsNullOrWhiteSpace(login));

            LoginCommand = ReactiveCommand.CreateFromTask(LoginImpl, canLogin);
        }

        private async Task<Unit> LoginImpl()
        {
            try
            {
                Client client = new Client($"{ServerAddress}:{ServerPort}");

                client.SuccessLoginEvent.Subscribe(async _ => await ShowScoreWindow.Handle(client));

                client.DisconnectedEvent.Subscribe(async s => await ShowErrorInteraction.Handle(s) );

                await client.LoginRequest(Login);

                return Unit.Default;
            }
            catch
            {
                return Unit.Default;
            }
        }
    }
}
