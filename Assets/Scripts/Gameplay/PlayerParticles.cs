using System;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    // The game advances these effects explicitly, keeping particle time aligned
    // with gameplay time through pause, countdown and death transitions.
    public sealed class PlayerParticles : MonoBehaviour
    {
        private const float DustRate = 24;
        private static readonly Color Jade = new Color(.48f, .92f, .77f, .7f);
        private static readonly Color Gold = new Color(1, .81f, .38f, .8f);
        private readonly System.Random random = new System.Random(7319);
        private ParticleSystem dust, impact, death;
        private Material particleMaterial;
        private float dustAccumulator;
        private Vector2 previousPosition;
        private bool hasPreviousPosition;

        public int DustEmitted { get; private set; }
        public int ImpactBursts { get; private set; }
        public int DeathBursts { get; private set; }
        public int ParticleCount => (dust ? dust.particleCount : 0)
            + (impact ? impact.particleCount : 0) + (death ? death.particleCount : 0);

        public void Initialize(Sprite sprite, Material material)
        {
            if (!sprite) throw new ArgumentNullException(nameof(sprite));
            if (!material) throw new ArgumentNullException(nameof(material));
            if (particleMaterial) throw new InvalidOperationException("Particle effects are already initialized.");

            // Reuse the referenced sprite shader and atlas rather than depending
            // on a shader name that could be stripped from the player build.
            particleMaterial = new Material(material) { name = "SpiritParticleMaterial" };
            particleMaterial.mainTexture = sprite.texture;
            if (particleMaterial.HasProperty("_Color")) particleMaterial.SetColor("_Color", Color.white);
            if (particleMaterial.HasProperty("_RendererColor")) particleMaterial.SetColor("_RendererColor", Color.white);
            dust = CreateSystem("CultivatorDust", sprite, 96, 9);
            impact = CreateSystem("WallImpact", sprite, 48, 12);
            death = CreateSystem("SpiritDispersal", sprite, 96, 13);
        }

        public void Tick(float seconds, Vector2 playerPosition, bool moving)
        {
            if (seconds < 0 || float.IsNaN(seconds) || float.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            if (!dust) return;

            Simulate(dust, seconds);
            Simulate(impact, seconds);
            Simulate(death, seconds);
            if (!hasPreviousPosition)
            {
                previousPosition = playerPosition;
                hasPreviousPosition = true;
            }

            if (moving && seconds > 0)
            {
                dustAccumulator += seconds * DustRate;
                int count = Mathf.Min(24, Mathf.FloorToInt(dustAccumulator));
                dustAccumulator -= Mathf.Floor(dustAccumulator);
                // A tunnel jump must not leave a line of dust across the maze.
                Vector2 origin = Vector2.Distance(previousPosition, playerPosition) > 2
                    ? playerPosition : previousPosition;
                for (int index = 0; index < count; index++)
                {
                    Vector2 position = Vector2.Lerp(origin, playerPosition, (index + 1f) / count)
                        + new Vector2(Range(-.16f, .16f), Range(-.30f, -.18f));
                    Emit(dust, position, new Vector2(Range(-.18f, .18f), Range(.05f, .3f)),
                        Range(.10f, .18f), Range(.28f, .48f), DustEmitted % 2 == 0 ? Gold : Jade);
                    DustEmitted++;
                }
            }
            else dustAccumulator = 0;
            previousPosition = playerPosition;
        }

        public void Impact(Vector2 position, GridDirection direction)
        {
            if (!impact) return;
            Vector2 normal = MazeNavigation.StepVector(direction);
            if (normal == Vector2.zero) return;
            Vector2 tangent = new Vector2(-normal.y, normal.x);
            Vector2 contact = position + normal * .43f;
            for (int index = 0; index < 14; index++)
            {
                Vector2 velocity = -normal * Range(.6f, 1.4f) + tangent * Range(-.9f, .9f);
                Emit(impact, contact + tangent * Range(-.09f, .09f), velocity,
                    Range(.10f, .19f), Range(.3f, .55f), index % 3 == 0 ? Jade : Gold);
            }
            ImpactBursts++;
        }

        public void Death(Vector2 position)
        {
            if (!death) return;
            dustAccumulator = 0;
            for (int index = 0; index < 72; index++)
            {
                float angle = index * Mathf.PI * 2 / 72 + Range(-.04f, .04f);
                Vector2 radial = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 velocity = radial * Range(.55f, 1.65f) + Vector2.up * .25f;
                Emit(death, position + radial * Range(.02f, .16f), velocity,
                    Range(.13f, .29f), 1.8f, index % 3 == 0 ? Gold : Jade);
            }
            DeathBursts++;
        }

        public void Clear()
        {
            if (dust) dust.Clear(false);
            if (impact) impact.Clear(false);
            if (death) death.Clear(false);
            dustAccumulator = 0;
            hasPreviousPosition = false;
        }

        private ParticleSystem CreateSystem(string label, Sprite sprite, int capacity, int sortingOrder)
        {
            var item = new GameObject(label);
            item.SetActive(false);
            item.transform.SetParent(transform, false);
            var system = item.AddComponent<ParticleSystem>();
            var main = system.main;
            main.playOnAwake = false;
            main.loop = true;
            main.duration = 5;
            main.startSpeed = 0;
            main.startLifetime = 1.8f;
            main.startSize = .15f;
            main.gravityModifier = 0;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Shape;
            main.maxParticles = capacity;
            main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
            var emission = system.emission;
            emission.enabled = false;
            var shape = system.shape;
            shape.enabled = false;
            var texture = system.textureSheetAnimation;
            texture.enabled = true;
            texture.mode = ParticleSystemAnimationMode.Sprites;
            texture.AddSprite(sprite);
            texture.frameOverTime = new ParticleSystem.MinMaxCurve(0);

            var fade = new Gradient();
            fade.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(.8f, .35f), new GradientAlphaKey(0, 1) });
            var color = system.colorOverLifetime;
            color.enabled = true;
            color.color = new ParticleSystem.MinMaxGradient(fade);
            var size = system.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.EaseInOut(0, 1, 1, .2f));

            var renderer = item.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.sharedMaterial = particleMaterial;
            renderer.sortingOrder = sortingOrder;
            system.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            item.SetActive(true);
            system.Simulate(0, false, false, false);
            return system;
        }

        private void Emit(ParticleSystem system, Vector2 position, Vector2 velocity, float size, float lifetime, Color color)
        {
            var parameters = new ParticleSystem.EmitParams
            {
                position = new Vector3(position.x, position.y, 0),
                velocity = new Vector3(velocity.x, velocity.y, 0),
                startSize = size,
                startLifetime = lifetime,
                startColor = color,
                rotation = Range(0, 360),
                angularVelocity = Range(-60, 60),
                randomSeed = (uint)random.Next(1, int.MaxValue),
                applyShapeToPosition = false
            };
            system.Emit(parameters, 1);
        }

        private static void Simulate(ParticleSystem system, float seconds)
        {
            if (seconds > 0) system.Simulate(seconds, false, false, false);
        }

        private float Range(float minimum, float maximum)
            => minimum + (maximum - minimum) * (float)random.NextDouble();

        private void OnDestroy()
        {
            if (!particleMaterial) return;
            if (Application.isPlaying) Destroy(particleMaterial);
            else DestroyImmediate(particleMaterial);
        }
    }
}
