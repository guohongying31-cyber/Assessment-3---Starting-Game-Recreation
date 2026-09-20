using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField] private ArcadeAudio audioPlayer;
        [SerializeField] private Camera menuCamera;
        [SerializeField] private GameObject[] rosterPrefabs = new GameObject[5];
        private Canvas canvas;
        private RectTransform canvasRect;
        private Button classicButton, trialButton, quitButton;
        private readonly RectTransform[] rosterAnchors = new RectTransform[5];
        private readonly Transform[] rosterActors = new Transform[5];
        private readonly float[] rosterHeights = new float[5];
        private bool loading;

        private void Start()
        {
            BuildInterface();
            CreateRoster();
            audioPlayer.Music("Menu");
            EventSystem.current.SetSelectedGameObject(classicButton.gameObject);
            Canvas.ForceUpdateCanvases();
            Resize();
        }

        private void Update()
        {
            if (loading || !classicButton) return;
            if (Input.GetKeyDown(KeyCode.Alpha1)) OpenLevel("ClassicLevel");
            else if (Input.GetKeyDown(KeyCode.Alpha2)) OpenLevel("TalismanLevel");
            if (EventSystem.current && !EventSystem.current.currentSelectedGameObject)
                EventSystem.current.SetSelectedGameObject(classicButton.gameObject);
            Resize();
        }

        private void BuildInterface()
        {
            var root = new GameObject("MenuCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRect = (RectTransform)root.transform;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 1080);
            scaler.matchWidthOrHeight = 1;

            StretchLabel("Eyebrow", "THE JADE LABYRINTH", -54, 36, 18, ArcadeHud.Gold);
            StretchLabel("Title", "SHAN HAI\nSPIRIT TRAIL", -141, 126, 52, ArcadeHud.Gold);
            StretchLabel("Introduction", "Gather spirit motes. Outwit the four guardians.", -239, 50, 20, ArcadeHud.Jade);

            string[] names = { "CULTIVATOR", "NINE-TAILED\nFOX", "FLAME\nCRANE", "WHITE\nHOUND", "SERPENT\nTORTOISE" };
            for (int index = 0; index < rosterAnchors.Length; index++)
            {
                float centre = .1f + index * .2f;
                var anchor = new GameObject("RosterAnchor_" + index, typeof(RectTransform));
                anchor.transform.SetParent(canvasRect, false);
                var rect = (RectTransform)anchor.transform;
                rect.anchorMin = rect.anchorMax = new Vector2(centre, 1);
                rect.anchoredPosition = new Vector2(0, -336);
                rect.sizeDelta = Vector2.zero;
                rosterAnchors[index] = rect;
                var label = ArcadeHud.Label(canvasRect, "RosterName_" + index, names[index], 16,
                    index == 0 ? ArcadeHud.Gold : ArcadeHud.Jade, new Vector2(centre, 1),
                    new Vector2(0, -406), new Vector2(100, 48));
                label.rectTransform.anchorMin = new Vector2(index * .2f, 1);
                label.rectTransform.anchorMax = new Vector2((index + 1) * .2f, 1);
                label.rectTransform.sizeDelta = new Vector2(-16, 48);
                FitText(label, 12, 16);
            }

            classicButton = CreateButton(canvasRect, "ClassicTrail", "CLASSIC TRAIL", new Vector2(.5f, 1),
                new Vector2(0, -504), new Vector2(680, 78), () => OpenLevel("ClassicLevel"));
            StretchLabel("ClassicDescription", "Collect every mote. Avoid the guardians.\nBEST  " +
                PlayerPrefs.GetInt("SpiritTrail.ClassicBest", 0).ToString("D6"), -571, 52, 18, ArcadeHud.Jade);

            trialButton = CreateButton(canvasRect, "TalismanTrial", "TALISMAN TRIAL", new Vector2(.5f, 1),
                new Vector2(0, -654), new Vector2(680, 78), () => OpenLevel("TalismanLevel"));
            StretchLabel("TrialDescription", "SPACE: 3-second shield / 15-second recharge\nBEST  " +
                PlayerPrefs.GetInt("SpiritTrail.TrialBest", 0).ToString("D6"), -721, 56, 18, ArcadeHud.Jade);

            StretchLabel("Controls", "ARROWS / WASD  Move     P / ESC  Pause\nSPACE  Activate the ward in Talisman Trial",
                -812, 66, 18, Color.white);
            quitButton = CreateButton(canvasRect, "Quit", "QUIT", new Vector2(.5f, 1),
                new Vector2(0, -918), new Vector2(220, 56), Quit);
            quitButton.interactable = !Application.isEditor;
            StretchLabel("Navigation", "ENTER selects     1 Classic     2 Trial", -993, 42, 16, ArcadeHud.Jade);

            var classicNavigation = classicButton.navigation;
            classicNavigation.mode = Navigation.Mode.Explicit;
            classicNavigation.selectOnDown = trialButton;
            classicNavigation.selectOnUp = quitButton.interactable ? quitButton : trialButton;
            classicButton.navigation = classicNavigation;
            var trialNavigation = trialButton.navigation;
            trialNavigation.mode = Navigation.Mode.Explicit;
            trialNavigation.selectOnUp = classicButton;
            trialNavigation.selectOnDown = quitButton.interactable ? quitButton : classicButton;
            trialButton.navigation = trialNavigation;
            var quitNavigation = quitButton.navigation;
            quitNavigation.mode = Navigation.Mode.Explicit;
            quitNavigation.selectOnUp = trialButton;
            quitNavigation.selectOnDown = classicButton;
            quitButton.navigation = quitNavigation;
        }

        private void CreateRoster()
        {
            var root = new GameObject("MenuCharacterRoster");
            for (int index = 0; index < rosterActors.Length; index++)
            {
                var actor = Instantiate(rosterPrefabs[index], root.transform);
                actor.name = "Showcase_" + rosterPrefabs[index].name;
                var presentation = actor.AddComponent<ActorSprite>();
                presentation.Present(index == 0 ? GridDirection.Right : GridDirection.Left, true);
                rosterActors[index] = actor.transform;
                var renderers = actor.GetComponentsInChildren<SpriteRenderer>();
                Bounds bounds = renderers[0].bounds;
                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                rosterHeights[index] = Mathf.Max(.01f, bounds.size.y);
            }
        }

        private void Resize()
        {
            float width = Mathf.Min(680, canvasRect.rect.width - 64);
            ((RectTransform)classicButton.transform).sizeDelta = new Vector2(width, 78);
            ((RectTransform)trialButton.transform).sizeDelta = new Vector2(width, 78);
            float worldPerPixel = menuCamera.orthographicSize * 2 / Mathf.Max(1, menuCamera.pixelHeight);
            for (int index = 0; index < rosterActors.Length; index++)
            {
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : menuCamera,
                    rosterAnchors[index].position);
                Vector3 position = menuCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -menuCamera.transform.position.z));
                position.z = 0;
                rosterActors[index].position = position;
                float size = 90 * canvas.scaleFactor * worldPerPixel / rosterHeights[index];
                rosterActors[index].localScale = Vector3.one * size;
            }
        }

        private Text StretchLabel(string name, string text, float offset, float height, int size, Color color)
        {
            var label = ArcadeHud.Label(canvasRect, name, text, size, color, new Vector2(.5f, 1),
                new Vector2(0, offset), new Vector2(0, height));
            label.rectTransform.anchorMin = new Vector2(0, 1);
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.sizeDelta = new Vector2(-48, height);
            FitText(label, Mathf.Min(14, size), size);
            return label;
        }

        private static void FitText(Text label, int minimum, int maximum)
        {
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = minimum;
            label.resizeTextMaxSize = maximum;
        }

        private void OpenLevel(string scene)
        {
            if (loading) return;
            loading = true;
            SceneManager.LoadScene(scene);
        }

        private static void Quit()
        {
            if (!Application.isEditor) Application.Quit();
        }

        public static Button CreateButton(Transform parent, string name, string text, Vector2 anchor,
            Vector2 position, Vector2 size, UnityAction action)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(Outline));
            item.transform.SetParent(parent, false);
            var rect = (RectTransform)item.transform;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            item.GetComponent<Image>().color = Color.white;
            var outline = item.GetComponent<Outline>();
            outline.effectColor = new Color(ArcadeHud.Jade.r, ArcadeHud.Jade.g, ArcadeHud.Jade.b, .65f);
            outline.effectDistance = new Vector2(1, -1);
            var button = item.GetComponent<Button>();
            button.targetGraphic = item.GetComponent<Image>();
            var colors = button.colors;
            colors.normalColor = new Color(.065f, .14f, .17f);
            colors.highlightedColor = new Color(.12f, .28f, .26f);
            colors.selectedColor = new Color(.14f, .34f, .30f);
            colors.pressedColor = new Color(.23f, .44f, .34f);
            colors.disabledColor = new Color(.05f, .075f, .085f);
            colors.fadeDuration = .12f;
            button.colors = colors;
            button.onClick.AddListener(action);
            var label = ArcadeHud.Label(item.transform, "Label", text, 26, ArcadeHud.Gold,
                new Vector2(.5f, .5f), Vector2.zero, size);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(16, 4);
            label.rectTransform.offsetMax = new Vector2(-16, -4);
            FitText(label, 14, 26);
            return button;
        }
    }
}
