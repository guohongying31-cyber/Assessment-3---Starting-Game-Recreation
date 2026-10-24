using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    // The simulation owns this clock. Countdown time counts towards the first
    // spawn; pausing the simulation freezes both waiting and travelling fruit.
    public sealed class CherryController : MonoBehaviour
    {
        private const double SpawnDelay = 5;
        private const float TravelSpeed = 3;
        private GameObject fruitPrefab, fruitView, maskView;
        private Camera levelCamera;
        private Texture2D maskTexture;
        private Sprite maskSprite;
        private Vector2 centre, halfSize, travelDirection, lastOutward;
        private double waitRemaining = SpawnDelay, travelled;
        private float spriteMargin;
        private bool initialized, hasPreviousStart;

        public bool Active => fruitView != null;
        public Vector2 Position { get; private set; }
        public Vector2 PreviousPosition { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 EndPosition { get; private set; }
        public float SpawnRemaining => Active ? 0 : (float)waitRemaining;
        public int SpawnCount { get; private set; }
        public float TravelProgress => Mathf.Clamp01((float)travelled / Mathf.Max(.001f, Vector2.Distance(StartPosition, EndPosition)));

        public void Initialize(GameObject prefab, int rows, int columns, Camera camera)
        {
            if (initialized) throw new InvalidOperationException("CherryController is already initialized.");
            if (!prefab) throw new ArgumentNullException(nameof(prefab));
            if (!camera) throw new ArgumentNullException(nameof(camera));
            if (rows < 1 || columns < 1) throw new ArgumentOutOfRangeException(nameof(rows));
            fruitPrefab = prefab;
            levelCamera = camera;
            centre = new Vector2((columns - 1) * .5f, -(rows - 1) * .5f);
            halfSize = new Vector2(columns * .5f, rows * .5f);
            CreateMask(columns, rows);
            initialized = true;
            ResetClock();
        }

        private void CreateMask(int columns, int rows)
        {
            maskTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            maskTexture.name = "CherryBoundsMaskTexture";
            maskTexture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            maskTexture.Apply();
            maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, 2, 2), Vector2.one * .5f, 2);
            maskSprite.name = "CherryBoundsMaskSprite";
            maskView = new GameObject("CherryLevelBoundsMask", typeof(SpriteMask));
            maskView.transform.SetParent(transform, false);
            maskView.transform.position = centre;
            maskView.transform.localScale = new Vector3(columns, rows, 1);
            var mask = maskView.GetComponent<SpriteMask>();
            mask.sprite = maskSprite;
            mask.alphaCutoff = .01f;
            mask.isCustomRangeActive = true;
            mask.backSortingLayerID = mask.frontSortingLayerID = 0;
            mask.frontSortingOrder = 51;
            // Unity clamps the back order to the current front order.
            mask.backSortingOrder = 49;
        }

        public void Tick(float seconds)
        {
            if (!initialized) throw new InvalidOperationException("Initialize the cherry before ticking.");
            if (seconds < 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            PreviousPosition = Position;
            double remaining = seconds;
            while (remaining > 0)
            {
                if (!Active)
                {
                    double step = Math.Min(remaining, waitRemaining);
                    remaining -= step;
                    waitRemaining -= step;
                    if (waitRemaining > 0) break;
                    Spawn();
                }
                if (remaining <= 0) break;
                // Re-evaluate the far edge after a window resize without changing
                // the straight trajectory or its distance travelled per second.
                UpdateEndPosition();
                double distance = Vector2.Distance(StartPosition, EndPosition);
                double secondsToExit = Math.Max(0, distance - travelled) / TravelSpeed;
                double movementStep = Math.Min(remaining, secondsToExit);
                travelled += movementStep * TravelSpeed;
                remaining -= movementStep;
                Position = StartPosition + travelDirection * (float)travelled;
                fruitView.transform.position = Position;
                if (movementStep >= secondsToExit) Despawn();
            }
        }

        private void Spawn()
        {
            fruitView = Instantiate(fruitPrefab, transform);
            fruitView.name = "Moving_CrimsonFruit";
            fruitView.transform.position = centre;
            var renderers = fruitView.GetComponentsInChildren<SpriteRenderer>();
            Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds(centre, Vector3.one);
            foreach (var renderer in renderers)
            {
                renderer.sortingLayerID = 0;
                renderer.sortingOrder = 50;
                renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                bounds.Encapsulate(renderer.bounds);
            }
            spriteMargin = Mathf.Max(.6f, bounds.extents.magnitude) + .1f;
            Vector2 outward = Vector2.right;
            for (int attempt = 0; attempt < 16; attempt++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                outward = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                if (!hasPreviousStart || Vector2.Dot(outward, lastOutward) < .98f) break;
                if (attempt == 15) outward = new Vector2(-lastOutward.y, lastOutward.x);
            }
            lastOutward = outward;
            hasPreviousStart = true;
            travelDirection = -outward;
            StartPosition = centre + outward * ExitDistance(centre, outward,
                centre - halfSize - Vector2.one * spriteMargin, centre + halfSize + Vector2.one * spriteMargin);
            Position = PreviousPosition = StartPosition;
            fruitView.transform.position = Position;
            travelled = 0;
            waitRemaining = 0;
            SpawnCount++;
            UpdateEndPosition();
        }

        private void UpdateEndPosition()
        {
            float depth = Mathf.Abs(levelCamera.transform.position.z);
            Vector2 cameraMinimum = levelCamera.ViewportToWorldPoint(new Vector3(0, 0, depth));
            Vector2 cameraMaximum = levelCamera.ViewportToWorldPoint(new Vector3(1, 1, depth));
            Vector2 minimum = Vector2.Min(centre - halfSize, cameraMinimum) - Vector2.one * spriteMargin;
            Vector2 maximum = Vector2.Max(centre + halfSize, cameraMaximum) + Vector2.one * spriteMargin;
            EndPosition = centre + travelDirection * (ExitDistance(centre, travelDirection, minimum, maximum) + .01f);
        }

        private static float ExitDistance(Vector2 origin, Vector2 direction, Vector2 minimum, Vector2 maximum)
        {
            float x = Mathf.Abs(direction.x) < .000001f ? float.PositiveInfinity
                : ((direction.x > 0 ? maximum.x : minimum.x) - origin.x) / direction.x;
            float y = Mathf.Abs(direction.y) < .000001f ? float.PositiveInfinity
                : ((direction.y > 0 ? maximum.y : minimum.y) - origin.y) / direction.y;
            return Mathf.Min(x, y);
        }

        public bool Collect()
        {
            if (!Active) return false;
            Despawn();
            return true;
        }

        private void Despawn()
        {
            if (fruitView)
            {
                fruitView.SetActive(false);
                Destroy(fruitView);
                fruitView = null;
            }
            waitRemaining = SpawnDelay;
        }

        public void ResetClock()
        {
            Despawn();
            Position = PreviousPosition = centre;
            StartPosition = EndPosition = centre;
            travelled = 0;
            SpawnCount = 0;
        }

        private void OnDestroy()
        {
            if (fruitView) Destroy(fruitView);
            if (maskView) Destroy(maskView);
            if (maskSprite) Destroy(maskSprite);
            if (maskTexture) Destroy(maskTexture);
        }
    }
}
