using UnityEngine;

namespace ShanHaiSpiritTrail
{
    public static class MazeContact
    {
        // Fruit has no portal copies. Split a player teleport at the seam so
        // only the two short, visible player paths can intersect the fruit.
        public static bool CrossedFruit(Vector2 playerBefore, Vector2 playerAfter,
            Vector2 fruitBefore, Vector2 fruitAfter, int columns, float radius = .62f)
        {
            float horizontal = playerAfter.x - playerBefore.x;
            if (Mathf.Abs(horizontal) <= columns * .5f)
                return Segment(playerBefore, playerAfter, fruitBefore, fruitAfter, radius);
            Vector2 movement = Unwrap(playerAfter-playerBefore, columns);
            float edge = movement.x > 0 ? columns-.5f : -.5f;
            float fraction = Mathf.Clamp01((edge-playerBefore.x)/movement.x);
            Vector2 seam = playerBefore + movement*fraction;
            Vector2 fruitAtSeam = Vector2.Lerp(fruitBefore,fruitAfter,fraction);
            Vector2 otherSeam = seam + Vector2.right*(movement.x>0 ? -columns : columns);
            return Segment(playerBefore,seam,fruitBefore,fruitAtSeam,radius)
                || Segment(otherSeam,playerAfter,fruitAtSeam,fruitAfter,radius);
        }

        private static bool Segment(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, float radius)
        {
            Vector2 relative = a0-b0, velocity = (a1-a0)-(b1-b0);
            float t = velocity.sqrMagnitude < .000001f ? 0
                : Mathf.Clamp01(-Vector2.Dot(relative,velocity)/velocity.sqrMagnitude);
            return (relative+velocity*t).sqrMagnitude <= radius*radius;
        }

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
