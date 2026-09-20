using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    public sealed class ArcadeHud : MonoBehaviour
    {
        private Text score, round, remaining, message, controls;
        private GameObject veil;
        public static readonly Color Jade = new Color(.46f, .84f, .72f);
        public static readonly Color Gold = new Color(.95f, .79f, .40f);
        public static readonly Color Ink = new Color(.035f, .065f, .10f);
        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = 1;
            gameObject.AddComponent<GraphicRaycaster>();
            Label(transform, "Title", "SHAN HAI  /  SPIRIT TRAIL", 26, Gold,
                new Vector2(.5f, 1), new Vector2(0, -25), new Vector2(800, 40));
            score = Label(transform, "Score", "", 24, Jade, new Vector2(.25f, 1), new Vector2(0, -66), new Vector2(340, 36));
            round = Label(transform, "Round", "", 24, Color.white, new Vector2(.5f, 1), new Vector2(0, -66), new Vector2(300, 36));
            remaining = Label(transform, "Remaining", "", 24, Jade, new Vector2(.75f, 1), new Vector2(0, -66), new Vector2(340, 36));
            controls = Label(transform, "Controls", "ARROWS / WASD  Move     P / ESC  Pause", 22, Jade,
                new Vector2(.5f, 0), new Vector2(0, 30), new Vector2(1200, 42));
            veil = new GameObject("StatusOverlay", typeof(RectTransform), typeof(Image));
            veil.transform.SetParent(transform, false);
            var rect = (RectTransform)veil.transform;
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            veil.GetComponent<Image>().color = new Color(.02f, .04f, .07f, .80f);
            message = Label(veil.transform, "Status", "", 42, Gold, new Vector2(.5f, .5f), Vector2.zero, new Vector2(1000, 220));
        }

        public void Draw(ArcadeGame game)
        {
            score.text = "SCORE  " + game.Score.ToString("D6");
            round.text = "ROUND  " + game.Round;
            remaining.text = "MOTES  " + game.Pickups.Remaining;
            veil.SetActive(game.Phase != ArcadePhase.Playing);
            message.text = game.Phase == ArcadePhase.Ready ? "ENTER THE JADE LABYRINTH\n" + Mathf.Max(1, Mathf.CeilToInt(game.ReadyTime))
                : game.Phase == ArcadePhase.Paused ? "PAUSED\n<size=24>Press P or ESC to resume</size>"
                : "MAZE CLEARED\n<size=24>Press ENTER for the next round</size>";
        }

        public static Text Label(Transform parent, string name, string text, int size, Color color,
            Vector2 anchor, Vector2 position, Vector2 dimensions)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(Text));
            item.transform.SetParent(parent, false);
            var label = item.GetComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text; label.fontSize = size; label.color = color;
            label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position; rect.sizeDelta = dimensions;
            return label;
        }
    }
}
