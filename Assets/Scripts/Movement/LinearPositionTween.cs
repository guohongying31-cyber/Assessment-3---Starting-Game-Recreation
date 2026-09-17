using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public sealed class LinearPositionTween
    {
        private readonly Vector3 start;
        private readonly Vector3 end;
        private readonly double duration;
        private double elapsed;

        public bool IsComplete => elapsed >= duration;

        public LinearPositionTween(Vector3 start, Vector3 end, double durationSeconds)
        {
            if (durationSeconds <= 0 || double.IsNaN(durationSeconds) || double.IsInfinity(durationSeconds))
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));

            this.start = start;
            this.end = end;
            duration = durationSeconds;
        }

        public Vector3 Advance(double deltaSeconds, out double unusedSeconds)
        {
            if (deltaSeconds < 0 || double.IsNaN(deltaSeconds) || double.IsInfinity(deltaSeconds))
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

            double step = Math.Min(deltaSeconds, duration - elapsed);
            elapsed += step;
            unusedSeconds = deltaSeconds - step;
            return Vector3.Lerp(start, end, (float)(elapsed / duration));
        }
    }
}
