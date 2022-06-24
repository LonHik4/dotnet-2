using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Sudoku;

using SudokuClient;

namespace SudokuClient.ViewModels
{
    public class PlayViewModel : BaseViewModel
    {
        private readonly Client _client;

        [Reactive]
        public Point[,] Points { get; set; } = new Point[10, 10];

        [Reactive]
        public ObservableCollection<ObservableCollection<Point>> Cells { get; set; }

        public Interaction<Point, Unit> ServerResponse { get; } = new();

        public PlayViewModel (Client client)
        {
            _client = client;

            var cells = new ObservableCollection<ObservableCollection<Point>>();
            for (int i=0; i < 9; ++i)
            {
                
                var tmp = new ObservableCollection<Point>();
                for (int j =0; j < 9; ++j)
                {
                    tmp.Add(new Point() { X = i, Y = j }) ;
                    
                }
                cells.Add(tmp);
            }

            foreach (var p in _client.Field.FixedPoints)
            {
                cells[p.X][p.Y] = new Point(p);
            }

            Cells = cells;
        }

        public async Task<int> TryChange(Point point)
        {
            var fixedPoint = _client.Field.FixedPoints.FirstOrDefault(p => p.X == point.X && p.Y == point.Y);
            await _client.TurnRequest(point);
            if (fixedPoint is not null || ! await _client.TurnEvent.)
            {
                return point.Value = fixedPoint.Value;
            }

            

            return point.Value;
        }

        
    }
}
