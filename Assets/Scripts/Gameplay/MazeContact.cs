using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public static class MazeContact
    {
        // Sweep relative motion so two actors cannot pass through one another
        // between rendered frames. Unwrap the short motion at the side tunnel.
        public static bool Crossed(Vector2 playerBefore, Vector2 playerAfter,
            Vector2 ghostBefore, Vector2 ghostAfter, int columns, float radius = .62f)
        {
            Vector2 velocity = Unwrap(playerAfter - playerBefore, columns)
                - Unwrap(ghostAfter - ghostBefore, columns);
            Vector2 separation = playerBefore - ghostBefore;
            for (int copy = -1; copy <= 1; copy++)
            {
                Vector2 relative = separation + new Vector2(copy * columns, 0);
                float t = velocity.sqrMagnitude < .000001f ? 0
                    : Mathf.Clamp01(-Vector2.Dot(relative, velocity) / velocity.sqrMagnitude);
                if ((relative + velocity * t).sqrMagnitude <= radius * radius) return true;
            }
            return false;
        }
        private static Vector2 Unwrap(Vector2 delta, int columns)
        {
            if (delta.x > columns * .5f) delta.x -= columns;
            else if (delta.x < -columns * .5f) delta.x += columns;
            return delta;
        }
    }
}
