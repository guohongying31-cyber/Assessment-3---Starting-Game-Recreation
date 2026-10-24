using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public sealed class TalismanWard
    {
        public const float Duration = 3;
        public const float Recharge = 15;
        private double remaining, cooldown;
        public float Remaining => (float)remaining;
        public float Cooldown => (float)cooldown;
        public bool BlockedContact { get; private set; }
        public bool Active => remaining > 0;
        public bool TryActivate()
        {
            if (cooldown > 0) return false;
            remaining = Duration; cooldown = Recharge;
            BlockedContact = false;
            return true;
        }
        public bool RegisterBlockedContact()
        {
            if (BlockedContact) return false;
            BlockedContact = true;
            return true;
        }
        public void Tick(float seconds)
        {
            if (seconds < 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            remaining = Math.Max(0, remaining-seconds);
            cooldown = Math.Max(0, cooldown-seconds);
            // Substeps arrive as floats. Remove only sub-microsecond residue
            // so an exact displayed deadline does not require another input.
            if (remaining < .000001) remaining = 0;
            if (cooldown < .000001) cooldown = 0;
        }
        public void Reset() { remaining = 0; cooldown = 0; BlockedContact = false; }
    }
}
