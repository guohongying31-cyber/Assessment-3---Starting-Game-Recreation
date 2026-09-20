using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public sealed class TalismanWard
    {
        public const float Duration = 3;
        public const float Recharge = 15;
        public float Remaining { get; private set; }
        public float Cooldown { get; private set; }
        public bool Active => Remaining > 0;
        public bool TryActivate()
        {
            if (Cooldown > 0) return false;
            Remaining = Duration; Cooldown = Recharge;
            return true;
        }
        public void Tick(float seconds)
        {
            if (seconds < 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            Remaining = Mathf.Max(0, Remaining-seconds);
            Cooldown = Mathf.Max(0, Cooldown-seconds);
        }
        public void Reset() { Remaining = 0; Cooldown = 0; }
    }
}
