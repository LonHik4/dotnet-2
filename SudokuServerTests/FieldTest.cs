using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Sudoku;

using SudokuServer;
using SudokuServer.Models;

using Xunit;

namespace SudokuServerTests
{
    public class FieldTest
    {
        private const string _filePath = "test_field.json";

        private Field GetTestField()
        {
            string jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Field>(jsonString);
        }

        [Fact]
        public void InsertInvalidPointTest()
        {
            var field = GetTestField();
            var invalidPoints = new List<Point>();
            invalidPoints.Add(new Point() { X = 1, Y = 0, Value = 50 });
            invalidPoints.Add(new Point() { X = 1, Y = 0, Value = -30 });
            invalidPoints.Add(new Point() { X = -10, Y = 0, Value = 3 });
            invalidPoints.Add(new Point() { X = 1, Y = 30, Value = 3 });

            foreach (var invalidPoint in invalidPoints)
            {
                Assert.False(field.ChangePoint(invalidPoint));
            }
        }

        [Fact]
        public void ChangeFixedPointTest()
        {
            var field = GetTestField();
            var invalidPoint = new Point() { X = 0, Y = 0, Value = 6 };

            var result = field.ChangePoint(invalidPoint);

            Assert.False(result);
        }

        [Fact]
        public void InsertValidPointTest()
        {
            var field = GetTestField();
            var invalidPoint = new Point() { X = 1, Y = 0, Value = 3 };

            var result = field.ChangePoint(invalidPoint);

            Assert.True(result);
        }

        [Fact]
        public void ChangePointCountTest()
        {
            var field = GetTestField();
            var validPoints = new List<Point>();
            validPoints.Add(new Point() { X = 1, Y = 1, Value = 3 });
            validPoints.Add(new Point() { X = 1, Y = 2, Value = 4 });

            var countBeforeInsert = field.Count;
            foreach (var p in validPoints)
                field.ChangePoint(p);
            var countAfterInsert = countBeforeInsert + validPoints.Count;

            Assert.Equal(validPoints.Count, countAfterInsert - countBeforeInsert);
        }
    }
}
