using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShanHaiSpiritTrail
{
    public static class GhostNumberLabel
    {
        public static Canvas Create(Transform ghost, int number)
        {
            if (number < 1 || number > 4) throw new ArgumentOutOfRangeException(nameof(number));
            string name = "Ghost" + number + "Canvas";
            var existing = ghost.Find(name);
            if (existing) return existing.GetComponent<Canvas>();
            var item = new GameObject(name, typeof(RectTransform), typeof(Canvas));
            item.transform.SetParent(ghost, false);
            var canvas = item.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 40;
            var rect = (RectTransform)item.transform;
            rect.sizeDelta = new Vector2(80, 80);
            rect.localPosition = new Vector3(0, .76f, -.1f);
            rect.localScale = Vector3.one * .008f;
            var label = ArcadeHud.Label(item.transform, "GhostNumber", number.ToString(), 64, ArcadeHud.Gold,
                new Vector2(.5f, .5f), Vector2.zero, new Vector2(80, 80));
            label.fontStyle = FontStyle.Bold;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            var outline = label.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(.02f, .04f, .06f, 1);
            outline.effectDistance = new Vector2(2, -2);
            return canvas;
        }
    }
}
