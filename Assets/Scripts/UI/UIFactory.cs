using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PixelIdle.UI
{
    /// <summary>Funções auxiliares para criar elementos de UI por código.</summary>
    public static class UIFactory
    {
        private static Font font;
        private static Sprite roundedSprite;

        public static Font DefaultFont
        {
            get
            {
                if (font == null)
                {
#if UNITY_2022_2_OR_NEWER
                    font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#else
                    font = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif
                }
                return font;
            }
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            return rt;
        }

        public static void Stretch(RectTransform rt, float left = 0, float bottom = 0, float right = 0, float top = 0)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static void Anchor(RectTransform rt, float minX, float minY, float maxX, float maxY,
                                  float left = 0, float bottom = 0, float right = 0, float top = 0)
        {
            rt.anchorMin = new Vector2(minX, minY);
            rt.anchorMax = new Vector2(maxX, maxY);
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static Image Panel(Transform parent, string name, Color color, bool rounded = true)
        {
            var rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            if (rounded)
            {
                img.sprite = RoundedSprite();
                img.type = Image.Type.Sliced;
            }
            return img;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
                                 TextAnchor anchor = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            var rt = Rect("Text", parent);
            var t = rt.gameObject.AddComponent<Text>();
            t.font = DefaultFont;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = anchor;
            t.fontStyle = style;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        public static void BestFit(Text t, int min, int max)
        {
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = min;
            t.resizeTextMaxSize = max;
        }

        public static Button MakeButton(Transform parent, string name, Color normal, Color disabled,
                                        UnityAction onClick, out Text label, int fontSize, Color textColor)
        {
            var img = Panel(parent, name, Color.white);
            var button = img.gameObject.AddComponent<Button>();
            button.targetGraphic = img;

            var colors = button.colors;
            colors.normalColor = normal;
            colors.highlightedColor = Color.Lerp(normal, Color.white, 0.12f);
            colors.pressedColor = Color.Lerp(normal, Color.black, 0.25f);
            colors.selectedColor = normal;
            colors.disabledColor = disabled;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            label = Label(img.transform, "", fontSize, textColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch(label.rectTransform, 10, 6, 10, 6);

            if (onClick != null) button.onClick.AddListener(onClick);
            return button;
        }

        /// <summary>Gera um sprite com cantos arredondados (9-slice) em tempo de execução.</summary>
        private static Sprite RoundedSprite()
        {
            if (roundedSprite != null) return roundedSprite;

            const int size = 64;
            const int radius = 20;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = x + 0.5f, py = y + 0.5f;
                    float dx = Mathf.Max(Mathf.Max(radius - px, 0f), px - (size - radius));
                    float dy = Mathf.Max(Mathf.Max(radius - py, 0f), py - (size - radius));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(radius - dist + 0.5f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();

            roundedSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f),
                                          100f, 0, SpriteMeshType.FullRect,
                                          new Vector4(radius, radius, radius, radius));
            return roundedSprite;
        }
    }
}
