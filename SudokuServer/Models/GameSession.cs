using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;

using Sudoku;

using SudokuServer.Services;

namespace SudokuServer.Models
{
    public class GameSession
    {
        private readonly Player _player;
        private readonly Field _field;
        private readonly IAsyncStreamReader<Request> _requestStream;
        private readonly IServerStreamWriter<Event> _responseStream;

        public GameSession(Player player, IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Event> responseStream)
        {
            _player = player;
            _requestStream = requestStream;
            _responseStream = responseStream;
            var fieldGenerator = new FieldGeneratorService();
            var field = fieldGenerator.GenerateField();
            while (field is not null && player.Solved.Contains(field.Id))
            {
                field = fieldGenerator.GenerateField();
            }
            if (field is null)
                throw new System.Exception("There are no fields left for this player");
            _field = field;
        }

        public async Task Start()
        {
            await SendPlayEvent();

            while (await _requestStream.MoveNext())
            {
                if (_requestStream.Current.RequestCase == Request.RequestOneofCase.Turn)
                {
                    var point = _requestStream.Current.Turn.Point;
                    if (!_field.ChangePoint(point))
                    {
                        await SendTurnEvent(false);
                    }
                    else
                    {
                        await SendTurnEvent(true);
                        if (_field.Count == _field.Size)
                        {
                            _player.Solved.Add(_field.Id);
                            await SendWinEvent();
                            return;
                        }
                    }
                }
            }
        }

        private async Task SendPlayEvent()
        {
            var playEvent = new PlayEvent();
            playEvent.Points.Add(_field.FixedPoints.ToArray());
            await _responseStream.WriteAsync(new Event() { Play = playEvent });
        }
        private async Task SendTurnEvent(bool success)
        {
            var turnEvent = new TurnEvent() { Success = success };
            await _responseStream.WriteAsync(new Event() { Turn = turnEvent });
        }
        private async Task SendWinEvent()
        {
            await _responseStream.WriteAsync(new Event() { Win = new WinEvent() });
        }
    }
}
