using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public sealed class LevelMapLayout
    {
        private readonly int[,] source;

        public int SourceRows => source.GetLength(0);
        public int SourceColumns => source.GetLength(1);
        public int Rows => SourceRows * 2 - 1;
        public int Columns => SourceColumns * 2;

        public LevelMapLayout(int[,] quadrant)
        {
            if (quadrant == null || quadrant.GetLength(0) == 0 || quadrant.GetLength(1) == 0)
                throw new ArgumentException("The level map must contain at least one row and column.", nameof(quadrant));

            source = (int[,])quadrant.Clone();
            for (int row = 0; row < SourceRows; row++)
            for (int column = 0; column < SourceColumns; column++)
                if (source[row, column] < 0 || source[row, column] > 8)
                    throw new ArgumentException($"Unknown tile category at row {row}, column {column}.", nameof(quadrant));
        }

        public int SourceCategory(int row, int column) => source[row, column];

        public int CategoryAt(int row, int column)
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException("Cell is outside the full level.");
            return source[Math.Min(row, Rows - 1 - row), Math.Min(column, Columns - 1 - column)];
        }

        public int CountCategory(int category)
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                if (CategoryAt(row, column) == category) count++;
            return count;
        }

        public int QuadrantRowCount(bool reflectY) => SourceRows - (reflectY ? 1 : 0);

        public Vector3 QuadrantOrigin(bool reflectX, bool reflectY)
            => new Vector3(reflectX ? Columns - 1 : 0, reflectY ? -(Rows - 1) : 0, 0);

        public static Vector3 Reflection(bool reflectX, bool reflectY)
            => new Vector3(reflectX ? -1 : 1, reflectY ? -1 : 1, 1);

        public static Vector3 CellPosition(int row, int column) => new Vector3(column, -row, 0);
    }
}
