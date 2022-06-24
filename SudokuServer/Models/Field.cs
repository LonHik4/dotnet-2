using System.Collections.Generic;
using System.Linq;

using Sudoku;

namespace SudokuServer.Models
{
    public class Field
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public int Count { get => FixedPoints.Count + _newPoints.Count; }
        public HashSet<Point> FixedPoints { get; set; } = new HashSet<Point>();

        [System.Text.Json.Serialization.JsonIgnore]
        private HashSet<Point> _newPoints = new HashSet<Point>();

        public int this[int x, int y]
        {
            get
            {
                var allPoints = new HashSet<Point>(FixedPoints);
                allPoints.UnionWith(_newPoints);
                var point = allPoints.FirstOrDefault(p => p.X == x && p.Y == y);
                if (point is null)
                    return 0;
                return point.Value;
            }
            set
            {
                var point = _newPoints.FirstOrDefault(p => p.X == x && p.Y == y);
                if (point is null)
                    _newPoints.Add(new Point() { X = x, Y = y, Value = value });
                else
                    point.Value = value;
            }
        }

        public bool ChangePoint(Point point)
        {
            var fixedPoint = FixedPoints.FirstOrDefault(p => p.X == point.X && p.Y == point.Y);
            if (fixedPoint is not null || !Check(point))
                return false;

            this[point.X, point.Y] = point.Value;
            return true;
        }

        private bool Check(Point point)
        {
            if (point.X < 0 || point.X > 9 || point.Y < 0 || point.Y > 9 || point.Value < 0 || point.Value > 9)
                return false;

            for (int i = 0; i < 9; ++i)
            {
                if (i == point.X)
                    continue;
                if (this[i, point.Y] == point.Value)
                    return false;
            }
            for (int i = 0; i < 9; ++i)
            {
                if (i == point.Y)
                    continue;
                if (this[point.X, i] == point.Value)
                    return false;
            }

            int I = point.X / 3;
            int J = point.Y / 3;

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (this[i + 3 * I, j + 3 * J] == point.Value)
                        return false;
                }
            }
            return true;
        }
    }
}
