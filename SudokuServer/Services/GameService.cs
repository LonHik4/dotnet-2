using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;


using SudokuServer.Models;
using SudokuServer.Repositories;

namespace SudokuServer.Services
{
    public class GameService
    {
        private readonly IPlayersRepository _playersRepository;
        private readonly ConcurrentDictionary<string, PlayerSession> _players = new();

        public GameService(IPlayersRepository playersRepository)
        {
            _playersRepository = playersRepository;
        }

        public async Task Connect(
            LoginRequest loginRequest,
            IAsyncStreamReader<Request> requestStream,
            IServerStreamWriter<Event> responseStream)
        {
            var loginEvent = new LoginEvent();
            loginEvent.Success = !_players.TryGetValue(loginRequest.Login, out _);
            await responseStream.WriteAsync(new Event() { Login = loginEvent });
            if (!loginEvent.Success)
                return;

            var player = await _playersRepository.GetPlayer(loginRequest.Login);
            Task<bool>? addPlayerTask = null;
            if (player is null)
            {
                player = new Player() { Login = loginRequest.Login };
                addPlayerTask = _playersRepository.AddPlayer(player);
            }

            var playerSession = new PlayerSession(player, requestStream, responseStream);

            try
            {
                if (!_players.TryAdd(loginRequest.Login, playerSession))
                    return;

                playerSession.StartGame().Wait();
                if (addPlayerTask is not null)
                    await addPlayerTask;
            }
            finally
            {
                await _playersRepository.UpdatePlayer(player);
                _players.TryRemove(loginRequest.Login, out _);
            }
        }
    }
}