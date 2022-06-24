using System.Threading.Tasks;

using Grpc.Core;

using Sudoku;

using SudokuServer.Services;

namespace SudokuServer
{
    public class LoginService : SudokuService.SudokuServiceBase
    {
        private readonly GameService _gameService;
        public LoginService(GameService gameService)
        {
            _gameService = gameService;
        }

        public override async Task Connect(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                if (requestStream.Current.RequestCase == Request.RequestOneofCase.Login)
                {
                    await _gameService.Connect(requestStream.Current.Login, requestStream, responseStream);
                }
            }
        }
    }
}
