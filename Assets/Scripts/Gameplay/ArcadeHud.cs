using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    public sealed class ArcadeHud : MonoBehaviour
    {
        private Text title, score, timer, remaining, message, controls, power, ward;
        private Button resumeButton, menuButton, exitButton;
        private readonly Image[] lifeImages = new Image[3];
        private ArcadeGame activeGame;
        private bool actionsVisible;
        private GameObject veil;
        private Image veilImage, wardMeterFill;
        private GameObject wardMeter;
        public static readonly Color Jade = new Color(.46f, .84f, .72f);
        public static readonly Color Gold = new Color(.95f, .79f, .40f);
        public static readonly Color Ink = new Color(.035f, .065f, .10f);
        private void Awake()
        {
            gameObject.name = "HUD";
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = 1;
            gameObject.AddComponent<GraphicRaycaster>();
            title = ResponsiveLabel(transform, "LevelName", "Level 1  /  SHAN HAI SPIRIT TRAIL", 26, 18, Gold,
                .04f, .82f, 1, -29, 40, 20);
            exitButton = MainMenu.CreateButton(transform, "ExitButton", "Exit", Vector2.one,
                new Vector2(-96, -29), new Vector2(128, 42), () => activeGame.ReturnToMenu());
            score = ResponsiveLabel(transform, "Score", "SCORE  000000", 24, 16, Jade,
                .04f, .35f, 1, -80, 46);
            timer = ResponsiveLabel(transform, "GameTimer", "TIME  00:00:00", 24, 16, Color.white,
                .35f, .65f, 1, -80, 46);
            remaining = ResponsiveLabel(transform, "Remaining", "", 24, 16, Jade,
                .65f, .96f, 1, -80, 46);
            CreateLifeIndicators();
            power = ResponsiveLabel(transform, "GhostScaredTimer", "", 22, 16, Jade,
                .36f, .64f, 0, 83, 52);
            power.gameObject.SetActive(false);
            ward = ResponsiveLabel(transform, "WardStatus", "", 22, 16, Gold,
                .64f, .96f, 0, 87, 54);
            CreateWardMeter();
            controls = ResponsiveLabel(transform, "Controls", "", 18, 14, Jade,
                0, 1, 0, 29, 40, 20);
            veil = new GameObject("StatusOverlay", typeof(RectTransform), typeof(Image));
            veil.transform.SetParent(transform, false);
            var rect = (RectTransform)veil.transform;
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            veilImage = veil.GetComponent<Image>();
            message = ResponsiveLabel(veil.transform, "Status", "", 42, 24, Gold,
                0, 1, .5f, 0, 280, 24);
            resumeButton = MainMenu.CreateButton(veil.transform,"ResumeButton","Resume",new Vector2(.5f,.5f),
                new Vector2(0,-190),new Vector2(310,64), () => activeGame.TogglePause());
            menuButton = MainMenu.CreateButton(veil.transform,"MenuButton","Exit",new Vector2(.5f,.5f),
                new Vector2(0,-270),new Vector2(310,64), () => activeGame.ReturnToMenu());
        }

        private void CreateLifeIndicators()
        {
            var root = new GameObject("LifeIndicators", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            var rect = (RectTransform)root.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.19f, 0);
            rect.anchoredPosition = new Vector2(0, 83);
            rect.sizeDelta = new Vector2(300, 52);
            Label(root.transform, "LivesLabel", "LIVES", 22, Gold, new Vector2(.5f, .5f),
                new Vector2(-90, 0), new Vector2(88, 48));
            for (int i = 0; i < lifeImages.Length; i++)
            {
                var item = new GameObject("Life_" + (i + 1), typeof(RectTransform), typeof(Image));
                item.transform.SetParent(root.transform, false);
                var image = item.GetComponent<Image>();
                image.preserveAspect = true;
                image.raycastTarget = false;
                image.rectTransform.anchorMin = image.rectTransform.anchorMax = new Vector2(.5f, .5f);
                image.rectTransform.anchoredPosition = new Vector2(-20 + i * 52, 0);
                image.rectTransform.sizeDelta = new Vector2(44, 48);
                lifeImages[i] = image;
            }
        }

        public void SetLifeSprite(Sprite sprite)
        {
            foreach (var image in lifeImages) image.sprite = sprite;
        }

        private void CreateWardMeter()
        {
            wardMeter = new GameObject("WardMeter", typeof(RectTransform), typeof(Image));
            wardMeter.transform.SetParent(transform, false);
            var background = wardMeter.GetComponent<Image>();
            background.color = new Color(.10f, .20f, .22f, 1);
            background.raycastTarget = false;
            var rect = background.rectTransform;
            rect.anchorMin = new Vector2(.67f, 0);
            rect.anchorMax = new Vector2(.93f, 0);
            rect.anchoredPosition = new Vector2(0, 55);
            rect.sizeDelta = new Vector2(0, 7);
            var fill = new GameObject("WardMeterFill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(wardMeter.transform, false);
            wardMeterFill = fill.GetComponent<Image>();
            wardMeterFill.raycastTarget = false;
            wardMeterFill.rectTransform.anchorMin = Vector2.zero;
            wardMeterFill.rectTransform.anchorMax = Vector2.one;
            wardMeterFill.rectTransform.offsetMin = wardMeterFill.rectTransform.offsetMax = Vector2.zero;
        }

        private void DrawWard(ArcadeGame game)
        {
            ward.gameObject.SetActive(game.TalismanMode);
            wardMeter.SetActive(game.TalismanMode);
            if (!game.TalismanMode) return;
            bool active = game.Ward.Active;
            bool recharging = !active && game.Ward.Cooldown > 0;
            float proportion = active ? game.Ward.Remaining / TalismanWard.Duration
                : 1 - game.Ward.Cooldown / TalismanWard.Recharge;
            // Stretch the plain Image instead of relying on a runtime texture.
            wardMeterFill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(proportion), 1);
            wardMeterFill.color = active ? Gold : recharging ? Color.Lerp(Ink, Jade, .6f) : Jade;
            ward.color = active ? Gold : Jade;
            ward.text = active ? (game.Ward.BlockedContact ? "CONTACT BLOCKED" : "WARD ACTIVE")
                + "\n" + game.Ward.Remaining.ToString("F1") + "s protection"
                : recharging ? "WARD RECHARGING\n" + game.Ward.Cooldown.ToString("F1") + "s to ready"
                : "WARD READY\nSPACE: 3s protection";
        }

        public void Draw(ArcadeGame game)
        {
            activeGame = game;
            title.text = game.TalismanMode ? "Level 2  /  TALISMAN TRIAL" : "Level 1  /  SHAN HAI SPIRIT TRAIL";
            score.text = "SCORE  " + game.Score.ToString("D6");
            timer.text = "TIME  " + LevelRecord.FormatTime(game.ElapsedSeconds);
            remaining.text = "MOTES  " + game.Pickups.Remaining;
            for (int i = 0; i < lifeImages.Length; i++) lifeImages[i].gameObject.SetActive(i < game.Lives);
            power.gameObject.SetActive(game.Ghosts.FrightRemaining > 0);
            power.text = "SCARED  " + Mathf.CeilToInt(game.Ghosts.FrightRemaining) + "s";
            controls.text = "ARROWS / WASD  Move     P / ESC  Pause" + (game.TalismanMode ? "     SPACE  Ward" : "");
            DrawWard(game);
            bool showActions = game.Phase == ArcadePhase.Paused;
            bool ended = game.Phase == ArcadePhase.GameOver;
            exitButton.interactable = !ended && game.Phase != ArcadePhase.Dying;
            resumeButton.gameObject.SetActive(showActions); menuButton.gameObject.SetActive(showActions);
            resumeButton.interactable = menuButton.interactable = showActions;
            if(showActions && !actionsVisible) resumeButton.Select();
            actionsVisible = showActions;
            veil.SetActive(game.Phase != ArcadePhase.Playing);
            float opacity = game.Phase == ArcadePhase.Dying ? .18f : game.Phase == ArcadePhase.Ready ? .4f : .8f;
            veilImage.color = new Color(.02f, .04f, .07f, opacity);
            message.text = game.Phase == ArcadePhase.Ready ? "ENTER THE JADE LABYRINTH\n" +
                (game.ReadyTime > 1 ? Mathf.Clamp(Mathf.CeilToInt(game.ReadyTime) - 1, 1, 3).ToString() : "GO!")
                : game.Phase == ArcadePhase.Paused ? "PAUSED\n<size=24>Press P or ESC to resume</size>"
                : game.Phase == ArcadePhase.Dying ? "SPIRIT DISPERSED"
                : "Game Over\n<size=24>" + (game.Pickups.Remaining == 0 ? "All spirit motes collected" : "No lives remaining")
                    + "</size>\n\n<size=26>SCORE  " + game.Score.ToString("D6") + "     TIME  "
                    + LevelRecord.FormatTime(game.ElapsedSeconds) + "</size>\n<size=20>Returning to the start screen in "
                    + Mathf.Max(1, Mathf.CeilToInt(game.ResultRemaining)) + "...</size>";
        }

        private static Text ResponsiveLabel(Transform parent, string name, string text, int size,
            int minimumSize, Color color, float left, float right, float verticalAnchor,
            float verticalOffset, float height, float inset = 12)
        {
            var label = Label(parent, name, text, size, color, Vector2.zero, Vector2.zero, Vector2.zero);
            var rect = label.rectTransform;
            rect.anchorMin = new Vector2(left, verticalAnchor);
            rect.anchorMax = new Vector2(right, verticalAnchor);
            rect.anchoredPosition = new Vector2(0, verticalOffset);
            rect.sizeDelta = new Vector2(-2 * inset, height);
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = minimumSize;
            label.resizeTextMaxSize = size;
            return label;
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
