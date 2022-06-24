using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sudoku;

namespace SudokuClient.Models
{
    public class Field
    {
        public HashSet<Point> FixedPoints { get; set; } = new HashSet<Point>();

        public Field(IEnumerable<Point> points)
        {
            FixedPoints = points.ToHashSet<Point>();
        }
    }
}
