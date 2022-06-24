using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Grpc.Core;

using Sudoku;

using SudokuServer.Models;
using SudokuServer.Repositories;

namespace SudokuServer.Services
{
    public class GameService
    {
        private readonly IPlayersRepository _playersRepository;
        private readonly ConcurrentDictionary<string, Player> _players = new();

        public GameService(IPlayersRepository playersRepository)
        {
            _playersRepository = playersRepository;
        }

        public async Task Connect(
            LoginRequest loginRequest,
            IAsyncStreamReader<Request> requestStream,
            IServerStreamWriter<Event> responseStream)
        {
            if (_players.TryGetValue(loginRequest.Login, out _))
            {
                await SendLoginEvent(responseStream, false);
                return;
            }
            await SendLoginEvent(responseStream, true);

            var player = await _playersRepository.GetPlayer(loginRequest.Login);
            if (player is null)
            {
                player = new Player() { Login = loginRequest.Login };
                if (!await _playersRepository.AddPlayer(player))
                {
                    await SendErrorEvent(responseStream, "Internal error. Re Login please.");
                    return;
                }
            }
            if (!_players.TryAdd(loginRequest.Login, player))
                return;
            try
            {
                if (await requestStream.MoveNext())
                {
                    if (requestStream.Current.RequestCase != Request.RequestOneofCase.Play)
                        await SendErrorEvent(responseStream, "Play Request expected. Re Login please.");
                    else
                    {
                        var gameSession = new GameSession(player, requestStream, responseStream);
                        gameSession.Start().Wait();
                    }
                }
            }
            catch (Exception exception)
            {
                await SendErrorEvent(responseStream, exception.Message);
            }
            finally
            {
                _players.TryRemove(loginRequest.Login, out _);
                await _playersRepository.UpdatePlayer(player);
            }
        }

        private static async Task SendLoginEvent(IServerStreamWriter<Event> responseStream, bool success)
        {
            var loginEvent = new LoginEvent() { Success = success };
            await responseStream.WriteAsync(new Event() { Login = loginEvent });
        }

        private static async Task SendErrorEvent(IServerStreamWriter<Event> responseStream, string message)
        {
            var errorEvent = new ErrorEvent() { Error = message };
            await responseStream.WriteAsync(new Event() { Error = errorEvent });
        }
    }
}
