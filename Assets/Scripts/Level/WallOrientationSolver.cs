using System;
using System.Collections.Generic;

namespace ShanHaiSpiritTrail
{
    public static class WallOrientationSolver
    {
        private const int SearchNodeLimit = 100000;

        // Directions are north, east, south, west. Outer arms are wider than inner arms.
        // The exit seal uses the inner arm width, with an intentional colour transition.
        private static readonly int[,] BasePorts =
        {
            {0, 0, 0, 0},
            {0, 1, 1, 0},
            {0, 1, 0, 1},
            {0, 2, 2, 0},
            {0, 2, 0, 2},
            {0, 0, 0, 0},
            {0, 0, 0, 0},
            {0, 1, 2, 1},
            {0, 2, 0, 2}
        };

        private static readonly int[] RowStep = {-1, 0, 1, 0};
        private static readonly int[] ColumnStep = {0, 1, 0, -1};

        public static int[,] Solve(LevelMapLayout layout)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            if (layout.SourceCategory(0, 0) != 1)
                throw new ArgumentException("The top-left source cell must be an outer corner (category 1).", nameof(layout));

            var domains = new int[layout.SourceRows, layout.SourceColumns];
            for (int row = 0; row < layout.SourceRows; row++)
            for (int column = 0; column < layout.SourceColumns; column++)
            {
                int category = layout.SourceCategory(row, column);
                int candidates = IsCornerOrJunction(category) ? 15 : IsWall(category) ? 3 : 1;
                if (row == 0 && column == 0) candidates = 1;

                // The last source row is shared by both halves of the full map.
                // Its artwork must remain the same after a vertical reflection.
                if (row == layout.SourceRows - 1)
                    for (int turn = 0; turn < 4; turn++)
                        if (Port(category, turn, 0) != Port(category, turn, 2))
                            candidates &= ~(1 << turn);

                if (candidates == 0)
                    throw new InvalidOperationException($"Tile at row {row}, column {column} cannot be symmetric on the shared center row.");
                domains[row, column] = candidates;
            }

            int visitedNodes = 0;
            string conflict = "No compatible wall orientations exist.";
            int[,] solution = Search(layout, domains, ref visitedNodes, ref conflict);
            if (solution == null)
                throw new InvalidOperationException("The level map has incompatible wall connections. " + conflict);

            var angles = new int[layout.SourceRows, layout.SourceColumns];
            for (int row = 0; row < layout.SourceRows; row++)
            for (int column = 0; column < layout.SourceColumns; column++)
                for (int turn = 0; turn < 4; turn++)
                    if ((solution[row, column] & (1 << turn)) != 0)
                        angles[row, column] = turn * 90;
            return angles;
        }

        private static int[,] Search(LevelMapLayout layout, int[,] domains, ref int visitedNodes, ref string conflict)
        {
            var pending = new Stack<int[,]>();
            pending.Push(domains);
            while (pending.Count > 0)
            {
                if (++visitedNodes > SearchNodeLimit)
                    throw new InvalidOperationException("Wall orientation search exceeded its 100000-node budget. Simplify ambiguous wall clusters before retrying.");
                domains = pending.Pop();
                if (!Propagate(layout, domains, ref conflict)) continue;

                int selectedRow = -1;
                int selectedColumn = -1;
                int smallestDomain = 5;
                for (int row = 0; row < layout.SourceRows; row++)
                for (int column = 0; column < layout.SourceColumns; column++)
                {
                    int count = CandidateCount(domains[row, column]);
                    if (count > 1 && count < smallestDomain)
                    {
                        selectedRow = row;
                        selectedColumn = column;
                        smallestDomain = count;
                    }
                }
                if (selectedRow < 0) return domains;

                // Choose by domain size, row, column and then ascending angle. Reverse
                // insertion makes the lowest angle the next branch popped from the stack.
                for (int turn = 3; turn >= 0; turn--)
                {
                    if ((domains[selectedRow, selectedColumn] & (1 << turn)) == 0) continue;
                    var trial = (int[,])domains.Clone();
                    trial[selectedRow, selectedColumn] = 1 << turn;
                    pending.Push(trial);
                }
            }
            return null;
        }

        private static bool Propagate(LevelMapLayout layout, int[,] domains, ref string conflict)
        {
            bool changed;
            do
            {
                changed = false;
                for (int row = 0; row < layout.SourceRows; row++)
                for (int column = 0; column < layout.SourceColumns; column++)
                {
                    if (!IsWall(layout.SourceCategory(row, column))) continue;
                    int remaining = domains[row, column];
                    for (int turn = 0; turn < 4; turn++)
                        if ((remaining & (1 << turn)) != 0 && !FitsNeighbors(layout, domains, row, column, turn))
                            remaining &= ~(1 << turn);

                    if (remaining == 0)
                    {
                        conflict = $"No rotation fits the neighbors of row {row}, column {column} (category {layout.SourceCategory(row, column)}).";
                        return false;
                    }
                    if (remaining == domains[row, column]) continue;
                    domains[row, column] = remaining;
                    changed = true;
                }
            } while (changed);
            return true;
        }

        private static bool FitsNeighbors(LevelMapLayout layout, int[,] domains, int row, int column, int turn)
        {
            int category = layout.SourceCategory(row, column);
            for (int direction = 0; direction < 4; direction++)
            {
                int neighborRow = row + RowStep[direction];
                int neighborColumn = column + ColumnStep[direction];
                // Outer boundaries may have tunnel arms. Across the right mirror seam,
                // each arm meets its own reflected copy. The shared bottom row was
                // already restricted to vertical symmetry, so its upper neighbor suffices.
                if (neighborRow < 0 || neighborRow >= layout.SourceRows ||
                    neighborColumn < 0 || neighborColumn >= layout.SourceColumns) continue;

                int facingPort = Port(category, turn, direction);
                int neighborCategory = layout.SourceCategory(neighborRow, neighborColumn);
                bool compatible = false;
                for (int neighborTurn = 0; neighborTurn < 4; neighborTurn++)
                    if ((domains[neighborRow, neighborColumn] & (1 << neighborTurn)) != 0 &&
                        facingPort == Port(neighborCategory, neighborTurn, (direction + 2) % 4))
                    {
                        compatible = true;
                        break;
                    }
                if (!compatible) return false;
            }
            return true;
        }

        private static int Port(int category, int turn, int direction) => BasePorts[category, (direction + turn) % 4];
        private static bool IsCornerOrJunction(int category) => category == 1 || category == 3 || category == 7;
        private static bool IsWall(int category) => category >= 1 && category <= 4 || category == 7 || category == 8;

        private static int CandidateCount(int domain)
        {
            int count = 0;
            for (int turn = 0; turn < 4; turn++)
                if ((domain & (1 << turn)) != 0) count++;
            return count;
        }
    }
}
