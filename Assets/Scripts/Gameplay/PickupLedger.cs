using System;
using System.Collections.Generic;

namespace ShanHaiSpiritTrail
{
    public sealed class PickupLedger
    {
        private readonly Dictionary<MazeCell, int> remaining = new Dictionary<MazeCell, int>();
        public int InitialCount { get; }
        public int Remaining => remaining.Count;
        public int Collected => InitialCount - Remaining;
        public event Action<MazeCell, int> CollectedPickup;
        public bool Contains(MazeCell cell) => remaining.ContainsKey(cell);

        public PickupLedger(MazeNavigation navigation)
        {
            foreach (var cell in navigation.Cells)
            {
                int category = navigation.CategoryAt(cell);
                if (category == 5 || category == 6) remaining.Add(cell, category);
            }
            InitialCount = Remaining;
        }

        public int Consume(MazeCell cell)
        {
            if (!remaining.TryGetValue(cell, out int category)) return 0;
            remaining.Remove(cell);
            CollectedPickup?.Invoke(cell, category);
            return category == 6 ? 50 : 10;
        }
    }
}
