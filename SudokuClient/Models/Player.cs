using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuClient.Models
{
    public sealed class Player
    {
        public string Login { get; set; } = string.Empty;
        public int Score;
    }
}
