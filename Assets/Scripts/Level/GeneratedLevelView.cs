using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LevelGenerator))]
    public sealed class GeneratedLevelView : MonoBehaviour
    {
        [SerializeField] private Transform showcase;
        [SerializeField] private Camera levelCamera;
        [SerializeField, Min(0.1f)] private float panelGap = 1f;
        [SerializeField, Min(0.1f)] private float cameraPadding = 0.8f;

        private enum Panel { Left, Right }

        private sealed class DisplayPose
        {
            public Transform Target;
            public Vector3 Position;
            public Bounds Envelope;
            public Panel Side;
        }

        private readonly List<DisplayPose> poses = new List<DisplayPose>();
        private readonly Vector3[] corners = new Vector3[4];
        private LevelGenerator generator;
        private Bounds leftEnvelope;
        private Bounds rightEnvelope;
        private RectTransform mazeTitle;
        private RectTransform mazeInventory;
        private RectTransform leftDivider;
        private RectTransform rightDivider;
        private Text inventoryText;
        private float unitsPerPixel;
        private float lastAspect = -1f;
        private bool prepared;

        private void Awake() => generator = GetComponent<LevelGenerator>();

        private void OnEnable()
        {
            if (!generator) generator = GetComponent<LevelGenerator>();
            generator.LevelGenerated += OnLevelGenerated;
            if (generator.GeneratedLayout != null) RefreshLayout();
        }

        private void Start()
        {
            if (generator.GeneratedLayout != null) RefreshLayout();
        }

        private void OnDisable()
        {
            if (generator) generator.LevelGenerated -= OnLevelGenerated;
        }

        private void OnLevelGenerated(LevelMapLayout layout) => RefreshLayout();

        private void LateUpdate()
        {
            if (levelCamera && generator.GeneratedLayout != null &&
                Mathf.Abs(levelCamera.aspect - lastAspect) > 0.0001f)
                RefreshLayout();
        }

        public void RefreshLayout()
        {
            LevelMapLayout layout = generator ? generator.GeneratedLayout : null;
            if (layout == null) return;
            if (!prepared) PrepareDisplay();

            float aspect = levelCamera.aspect;
            if (aspect <= 0f || float.IsNaN(aspect) || float.IsInfinity(aspect)) return;
            bool portrait = aspect < 1f;
            var map = new Bounds(
                new Vector3((layout.Columns - 1) * 0.5f, -(layout.Rows - 1) * 0.5f, 0),
                new Vector3(layout.Columns, layout.Rows, 0));

            mazeTitle.position = new Vector3(map.center.x, map.max.y + 0.62f, -1);
            mazeInventory.position = new Vector3(map.center.x, map.min.y - 0.62f, -1);
            inventoryText.text = $"SPIRIT MOTES {layout.CountCategory(5)}   |   WARD ELIXIRS {layout.CountCategory(6)}";

            Vector3 leftOffset;
            Vector3 rightOffset;
            if (portrait)
            {
                float totalWidth = leftEnvelope.size.x + panelGap + rightEnvelope.size.x;
                float leftEdge = map.center.x - totalWidth * 0.5f;
                float top = RectBounds(mazeInventory).min.y - panelGap;
                leftOffset = new Vector3(leftEdge - leftEnvelope.min.x, top - leftEnvelope.max.y, 0);
                rightOffset = new Vector3(leftEdge + leftEnvelope.size.x + panelGap - rightEnvelope.min.x,
                    top - rightEnvelope.max.y, 0);
            }
            else
            {
                leftOffset = new Vector3(map.min.x - panelGap - leftEnvelope.max.x,
                    map.center.y - leftEnvelope.center.y, 0);
                rightOffset = new Vector3(map.max.x + panelGap - rightEnvelope.min.x,
                    map.center.y - rightEnvelope.center.y, 0);
            }

            Bounds visible = map;
            visible.Encapsulate(RectBounds(mazeTitle));
            visible.Encapsulate(RectBounds(mazeInventory));
            foreach (DisplayPose pose in poses)
            {
                Vector3 offset = pose.Side == Panel.Left ? leftOffset : rightOffset;
                pose.Target.position = pose.Position + offset;
                var movedEnvelope = new Bounds(pose.Envelope.center + offset, pose.Envelope.size);
                visible.Encapsulate(movedEnvelope);
            }

            leftDivider.gameObject.SetActive(!portrait);
            rightDivider.gameObject.SetActive(!portrait);
            if (!portrait)
            {
                float height = Mathf.Max(map.size.y, Mathf.Max(leftEnvelope.size.y, rightEnvelope.size.y));
                ArrangeDivider(leftDivider, map.min.x - panelGap * 0.5f, map.center.y, height);
                ArrangeDivider(rightDivider, map.max.x + panelGap * 0.5f, map.center.y, height);
                visible.Encapsulate(RectBounds(leftDivider));
                visible.Encapsulate(RectBounds(rightDivider));
            }

            levelCamera.orthographic = true;
            levelCamera.transform.position = new Vector3(visible.center.x, visible.center.y,
                levelCamera.transform.position.z);
            levelCamera.orthographicSize = Mathf.Max(visible.extents.y,
                visible.extents.x / aspect) + cameraPadding;
            lastAspect = aspect;
        }

        private void PrepareDisplay()
        {
            if (!showcase || !levelCamera)
                throw new InvalidOperationException("The generated level view needs its showcase and camera references.");

            var labelRoot = showcase.Find("ShowcaseLabels") as RectTransform;
            if (!labelRoot || !labelRoot.TryGetComponent(out Canvas canvas) ||
                !labelRoot.TryGetComponent(out CanvasScaler scaler))
                throw new InvalidOperationException("The authored showcase label canvas is missing.");

            Vector2 reference = scaler.referenceResolution;
            unitsPerPixel = levelCamera.orthographicSize * 2f / reference.y;
            Vector3 referenceCenter = levelCamera.transform.position;
            // Preserve the authored label coordinates as world units before the
            // camera changes. Each panel then moves its sprites and labels together.
            scaler.enabled = false;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = levelCamera;
            labelRoot.anchorMin = labelRoot.anchorMax = labelRoot.pivot = new Vector2(0.5f, 0.5f);
            labelRoot.sizeDelta = reference;
            labelRoot.rotation = Quaternion.identity;
            labelRoot.localScale = Vector3.one * unitsPerPixel;
            labelRoot.position = new Vector3(referenceCenter.x, referenceCenter.y, -1);
            Canvas.ForceUpdateCanvases();

            mazeTitle = RequiredLabel(labelRoot, "MazeTitle");
            mazeInventory = RequiredLabel(labelRoot, "MazeInventory");
            leftDivider = RequiredLabel(labelRoot, "LeftDivider");
            rightDivider = RequiredLabel(labelRoot, "RightDivider");
            inventoryText = mazeInventory.GetComponent<Text>();

            foreach (Transform group in showcase)
            {
                if (group == labelRoot) continue;
                foreach (Transform item in group)
                {
                    SpriteRenderer[] renderers = item.GetComponentsInChildren<SpriteRenderer>();
                    if (renderers.Length == 0) continue;
                    Bounds envelope = renderers[0].bounds;
                    foreach (SpriteRenderer renderer in renderers) envelope.Encapsulate(renderer.bounds);
                    Panel side = group.name == "Characters" && item.name != "PacStudent" ? Panel.Right : Panel.Left;
                    AddPose(item, envelope, side);
                }
            }
            foreach (RectTransform label in labelRoot)
            {
                if (label == mazeTitle || label == mazeInventory || label == leftDivider || label == rightDivider) continue;
                AddPose(label, RectBounds(label), IsBestiaryLabel(label.name) ? Panel.Right : Panel.Left);
            }

            leftEnvelope = PanelBounds(Panel.Left);
            rightEnvelope = PanelBounds(Panel.Right);
            prepared = true;
        }

        private void AddPose(Transform target, Bounds envelope, Panel side)
        {
            poses.Add(new DisplayPose { Target = target, Position = target.position, Envelope = envelope, Side = side });
        }

        private Bounds PanelBounds(Panel side)
        {
            bool found = false;
            var result = new Bounds();
            foreach (DisplayPose pose in poses)
            {
                if (pose.Side != side) continue;
                if (!found) { result = pose.Envelope; found = true; }
                else result.Encapsulate(pose.Envelope);
            }
            if (!found) throw new InvalidOperationException("A showcase display panel has no content.");
            return result;
        }

        private Bounds RectBounds(RectTransform rect)
        {
            rect.GetWorldCorners(corners);
            var result = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Length; i++) result.Encapsulate(corners[i]);
            return result;
        }

        private void ArrangeDivider(RectTransform divider, float x, float y, float height)
        {
            divider.position = new Vector3(x, y, -1);
            divider.sizeDelta = new Vector2(1f, height / unitsPerPixel);
        }

        private static RectTransform RequiredLabel(Transform root, string name)
        {
            var label = root.Find(name) as RectTransform;
            if (!label) throw new InvalidOperationException("Missing showcase label: " + name);
            return label;
        }

        private static bool IsBestiaryLabel(string name)
        {
            switch (name)
            {
                case "BestiaryTitle":
                case "BestiarySubtitle":
                case "NineTailTitle":
                case "FlameCraneTitle":
                case "WhiteHoundTitle":
                case "SerpentTortoiseTitle":
                case "SpiritForms":
                case "StateNames":
                case "PreviewHint":
                    return true;
                default:
                    return false;
            }
        }
    }
}
