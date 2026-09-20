using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    public sealed class ArcadeHud : MonoBehaviour
    {
        private Text title, score, round, remaining, message, controls, lives, power;
        private Button primaryButton, menuButton;
        private ArcadeGame activeGame;
        private bool actionsVisible;
        private GameObject veil;
        private Image veilImage;
        private RectTransform canvasRect;
        public static readonly Color Jade = new Color(.46f, .84f, .72f);
        public static readonly Color Gold = new Color(.95f, .79f, .40f);
        public static readonly Color Ink = new Color(.035f, .065f, .10f);
        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRect = (RectTransform)transform;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = 1;
            gameObject.AddComponent<GraphicRaycaster>();
            title = ResponsiveLabel(transform, "Title", "SHAN HAI  /  SPIRIT TRAIL", 26, 18, Gold,
                0, 1, 1, -25, 40, 20);
            score = ResponsiveLabel(transform, "Score", "", 24, 14, Jade,
                .04f, .36f, 1, -78, 60);
            round = ResponsiveLabel(transform, "Round", "", 24, 14, Color.white,
                .36f, .64f, 1, -78, 60);
            remaining = ResponsiveLabel(transform, "Remaining", "", 24, 14, Jade,
                .64f, .96f, 1, -78, 60);
            lives = ResponsiveLabel(transform, "Lives", "", 22, 14, Gold,
                .04f, .36f, 0, 84, 48);
            power = ResponsiveLabel(transform, "Power", "", 22, 14, Jade,
                .36f, .96f, 0, 84, 48);
            controls = ResponsiveLabel(transform, "Controls", "", 22, 14, Jade,
                0, 1, 0, 30, 52, 20);
            veil = new GameObject("StatusOverlay", typeof(RectTransform), typeof(Image));
            veil.transform.SetParent(transform, false);
            var rect = (RectTransform)veil.transform;
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            veilImage = veil.GetComponent<Image>();
            message = ResponsiveLabel(veil.transform, "Status", "", 42, 24, Gold,
                0, 1, .5f, 0, 280, 24);
            primaryButton = MainMenu.CreateButton(veil.transform,"ContinueButton","Continue",new Vector2(.5f,.5f),
                new Vector2(0,-190),new Vector2(310,64), () => {
                    if(activeGame.Phase == ArcadePhase.Paused) activeGame.TogglePause();
                    else if(activeGame.Phase == ArcadePhase.RoundClear) activeGame.NextRound();
                    else activeGame.NewGame();
                });
            menuButton = MainMenu.CreateButton(veil.transform,"MenuButton","Main Menu",new Vector2(.5f,.5f),
                new Vector2(0,-270),new Vector2(310,64), () => activeGame.ReturnToMenu());
        }

        public void Draw(ArcadeGame game)
        {
            bool compact = canvasRect.rect.width < 900;
            activeGame = game;
            title.text = game.TalismanMode ? "SHAN HAI  /  TALISMAN TRIAL" : "SHAN HAI  /  CLASSIC TRAIL";
            string separator = compact ? "\n" : "  ";
            score.text = "SCORE" + separator + game.Score.ToString("D6");
            round.text = "ROUND" + separator + game.Round;
            remaining.text = "MOTES" + separator + game.Pickups.Remaining;
            lives.text = "LIVES  " + game.Lives + "\nBEST  " + game.BestScore.ToString("D6");
            power.text = game.Ghosts.FrightRemaining > 0 ? "ELIXIR  " + game.Ghosts.FrightRemaining.ToString("F1") + "s"
                : game.FruitTime > 0 ? "CRIMSON FRUIT  " + Mathf.CeilToInt(game.FruitTime) + "s" : "COLLECT EVERY SPIRIT MOTE";
            controls.text = "ARROWS / WASD  Move" + (compact ? "\n" : "     ") + "P / ESC  Pause";
            if(game.TalismanMode)
            {
                controls.text += "     SPACE  Ward";
                power.text += "\n" + (game.Ward.Active ? "WARD ACTIVE  " + game.Ward.Remaining.ToString("F1") + "s"
                    : game.Ward.Cooldown > 0 ? "WARD RECHARGE  " + game.Ward.Cooldown.ToString("F1") + "s" : "WARD READY");
            }
            bool showActions = game.Phase == ArcadePhase.Paused || game.Phase == ArcadePhase.GameOver || game.Phase == ArcadePhase.RoundClear;
            primaryButton.gameObject.SetActive(showActions); menuButton.gameObject.SetActive(showActions);
            primaryButton.GetComponentInChildren<Text>().text = game.Phase == ArcadePhase.Paused ? "Resume"
                : game.Phase == ArcadePhase.RoundClear ? "Next Round" : "Try Again";
            if(showActions && !actionsVisible) primaryButton.Select();
            actionsVisible = showActions;
            veil.SetActive(game.Phase != ArcadePhase.Playing);
            float opacity = game.Phase == ArcadePhase.Dying ? .18f : game.Phase == ArcadePhase.Ready ? .4f : .8f;
            veilImage.color = new Color(.02f, .04f, .07f, opacity);
            message.text = game.Phase == ArcadePhase.Ready ? "ENTER THE JADE LABYRINTH\n" + Mathf.Max(1, Mathf.CeilToInt(game.ReadyTime))
                : game.Phase == ArcadePhase.Paused ? "PAUSED\n<size=24>Press P or ESC to resume</size>"
                : game.Phase == ArcadePhase.Dying ? "SPIRIT DISPERSED\n<size=24>Lives remaining: " + game.Lives + "</size>"
                : game.Phase == ArcadePhase.GameOver ? "JOURNEY ENDED\n<size=24>Press ENTER to begin again</size>"
                : "MAZE CLEARED\n<size=24>Press ENTER for the next round</size>";
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
