using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    // A screen-anchored seal with travelling lights. Every graphic is decorative
    // and leaves pointer input available to the menu below it.
    public sealed class MenuBorderAnimation : MonoBehaviour
    {
        private RectTransform border;
        private readonly RectTransform[] lights = new RectTransform[8];
        private readonly Image[] lightImages = new Image[8];

        private void Awake()
        {
            var root = new GameObject("AnimatedSealBorder", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            border = (RectTransform)root.transform;
            border.anchorMin = Vector2.zero;
            border.anchorMax = Vector2.one;
            border.offsetMin = new Vector2(26, 26);
            border.offsetMax = new Vector2(-26, -26);
            CreateEdge("Top", new Vector2(0, 1), Vector2.one, new Vector2(0, 2));
            CreateEdge("Bottom", Vector2.zero, new Vector2(1, 0), new Vector2(0, 2));
            CreateEdge("Left", Vector2.zero, new Vector2(0, 1), new Vector2(2, 0));
            CreateEdge("Right", new Vector2(1, 0), Vector2.one, new Vector2(2, 0));
            for (int i = 0; i < lights.Length; i++)
            {
                var light = CreateImage("TravellingLight_" + (i + 1));
                lights[i] = light.rectTransform;
                lights[i].anchorMin = lights[i].anchorMax = Vector2.zero;
                lights[i].sizeDelta = new Vector2(10, 10);
                lights[i].localRotation = Quaternion.Euler(0, 0, 45);
                lightImages[i] = light;
            }
        }

        private Image CreateImage(string name)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(Image));
            item.transform.SetParent(border, false);
            var image = item.GetComponent<Image>();
            image.raycastTarget = false;
            return image;
        }

        private void CreateEdge(string name, Vector2 minimum, Vector2 maximum, Vector2 size)
        {
            var image = CreateImage(name);
            image.color = new Color(ArcadeHud.Jade.r, ArcadeHud.Jade.g, ArcadeHud.Jade.b, .35f);
            image.rectTransform.anchorMin = minimum;
            image.rectTransform.anchorMax = maximum;
            image.rectTransform.anchoredPosition = Vector2.zero;
            image.rectTransform.sizeDelta = size;
        }

        private void Update()
        {
            float width = border.rect.width;
            float height = border.rect.height;
            float perimeter = 2 * (width + height);
            if (perimeter <= 0) return;
            for (int i = 0; i < lights.Length; i++)
            {
                float distance = Mathf.Repeat(Time.unscaledTime * 155 + perimeter * i / lights.Length, perimeter);
                Vector2 position;
                if (distance < height) position = new Vector2(0, distance);
                else if ((distance -= height) < width) position = new Vector2(distance, height);
                else if ((distance -= width) < height) position = new Vector2(width, height - distance);
                else position = new Vector2(width - (distance - height), 0);
                lights[i].anchoredPosition = position;
                lightImages[i].color = Color.Lerp(ArcadeHud.Jade, ArcadeHud.Gold,
                    .5f + .5f * Mathf.Sin(Time.unscaledTime * 2 + i));
            }
        }
    }
}
