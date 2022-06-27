using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Sudoku;

namespace SudokuClient.ViewModels
{
    public class PlayViewModel : BaseViewModel
    {
        private readonly Client _client;

        [Reactive]
        public Point[,] Points { get; set; } = new Point[10, 10];

        public List<List<Point>> Cells { get; set; }

        public Interaction<Point, Unit> ServerResponse { get; } = new();

        public Interaction<Unit, Unit> ShowWinInteraction { get; } = new();

        public Interaction<Unit, Unit> ShowLoginWindow { get; } = new();

        public PlayViewModel(Client client)
        {
            _client = client;

            var cells = new List<List<Point>>();
            for (int i = 0; i < 9; ++i)
            {

                var tmp = new List<Point>();
                for (int j = 0; j < 9; ++j)
                {
                    tmp.Add(new Point() { X = i, Y = j });

                }
                cells.Add(tmp);
            }

            foreach (var p in _client.Field.FixedPoints)
            {
                cells[p.X][p.Y] = new Point(p);
            }

            Cells = cells;

            _disposables.Add(_client.WinEvent.Subscribe(async _ =>
            {
                await ShowWinInteraction.Handle(Unit.Default);
                await ShowLoginWindow.Handle(Unit.Default);
                DisposeSubscriptions();
            }));

            _disposables.Add(_client.DisconnectedEvent.Subscribe(async s =>
            {
                await ShowErrorInteraction.Handle(s);
                await ShowLoginWindow.Handle(Unit.Default);
                DisposeSubscriptions();
            }));
        }

        public async Task<int> TryChange(Point point)
        {
            var fixedPoint = _client.Field.FixedPoints.FirstOrDefault(p => p.X == point.X && p.Y == point.Y);

            if (fixedPoint is not null)
            {
                return point.Value = fixedPoint.Value;
            }
            var dd = _client._turnSubject.FirstAsync().PublishLast();
            dd.Connect();
            await _client.TurnRequest(point);
            if (!await dd)
            {
                return point.Value = 0;
            }

            return point.Value;
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
