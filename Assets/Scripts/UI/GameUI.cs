using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PixelIdle.Core;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace PixelIdle.UI
{
    /// <summary>Monta toda a interface por código e a mantém sincronizada com os sistemas.</summary>
    public class GameUI : MonoBehaviour
    {
        private const float NavHeight = 170f;

        private static readonly string[] TabNames = { "Início", "Melhorias", "Conquistas", "Estatísticas", "Configurações" };

        private GameManager game;
        private Text pixelsText;
        private Text rateText;
        private readonly List<ProductCardView> cards = new List<ProductCardView>();
        private NavigationController nav;

        public void Init(GameManager gameManager)
        {
            game = gameManager;

            EnsureEventSystem();
            Build();

            game.Wallet.Changed += RefreshAll;
            game.Generator.RateChanged += RefreshAll;
            RefreshAll();
        }

        private void OnDestroy()
        {
            if (game == null) return;
            game.Wallet.Changed -= RefreshAll;
            game.Generator.RateChanged -= RefreshAll;
        }

        // ---------------------------------------------------------------- Atualização

        private void RefreshAll()
        {
            pixelsText.text = NumberFormatter.FormatPixels(game.Wallet.Pixels);
            rateText.text = NumberFormatter.FormatRate(game.Generator.PixelsPerSecond) + " Pixels/s";
            foreach (var card in cards) card.Refresh(game.Wallet.Pixels);
        }

        // ---------------------------------------------------------------- Construção

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;

            var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            go.AddComponent<InputSystemUIInputModule>();
#else
            go.AddComponent<StandaloneInputModule>();
#endif
        }

        private void Build()
        {
            // Canvas escalável (referência 1080x1920, retrato)
            var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform, false);
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var bg = UIFactory.Panel(canvasGO.transform, "Background", Theme.Background, false);
            UIFactory.Stretch(bg.rectTransform);

            var safe = UIFactory.Rect("SafeArea", canvasGO.transform);
            UIFactory.Stretch(safe);
            safe.gameObject.AddComponent<SafeAreaFitter>();

            // Área de conteúdo (acima da barra de navegação)
            var content = UIFactory.Rect("Content", safe);
            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.one;
            content.offsetMin = new Vector2(0, NavHeight);
            content.offsetMax = Vector2.zero;

            // Barra de navegação inferior
            var navBar = UIFactory.Panel(safe, "NavBar", Theme.NavBar, false).rectTransform;
            navBar.anchorMin = new Vector2(0, 0);
            navBar.anchorMax = new Vector2(1, 0);
            navBar.pivot = new Vector2(0.5f, 0);
            navBar.sizeDelta = new Vector2(0, NavHeight);
            navBar.anchoredPosition = Vector2.zero;

            var row = navBar.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(12, 12, 14, 14);
            row.spacing = 10;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = row.childForceExpandHeight = true;

            // Telas + abas
            nav = new NavigationController();
            for (int i = 0; i < TabNames.Length; i++)
            {
                RectTransform screen = i == 0 ? BuildHome(content) : BuildPlaceholder(content, TabNames[i]);

                var button = UIFactory.MakeButton(navBar, "Tab_" + TabNames[i], Color.white, Color.white,
                                                  null, out Text label, 30, Theme.TextDim);
                button.transition = Selectable.Transition.None;
                UIFactory.BestFit(label, 18, 32);

                label.text = TabNames[i];
                nav.Add(button, label, screen.gameObject);
            }
            nav.Select(0);
        }

        private RectTransform BuildHome(RectTransform parent)
        {
            var screen = UIFactory.Rect("Screen_Inicio", parent);
            UIFactory.Stretch(screen);

            var v = screen.gameObject.AddComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(40, 40, 40, 30);
            v.spacing = 28;
            v.childControlWidth = v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;

            // --- Painel do contador
            var header = UIFactory.Panel(screen, "Header", Theme.Card);
            header.gameObject.AddComponent<LayoutElement>().preferredHeight = 380;
            var hv = header.gameObject.AddComponent<VerticalLayoutGroup>();
            hv.padding = new RectOffset(20, 20, 20, 20);
            hv.spacing = 6;
            hv.childAlignment = TextAnchor.MiddleCenter;
            hv.childControlWidth = hv.childControlHeight = true;
            hv.childForceExpandWidth = true;
            hv.childForceExpandHeight = false;

            UIFactory.Label(header.transform, "PIXELS", 40, Theme.Accent, TextAnchor.MiddleCenter, FontStyle.Bold);

            pixelsText = UIFactory.Label(header.transform, "0", 130, Theme.TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            UIFactory.BestFit(pixelsText, 50, 140);
            pixelsText.gameObject.AddComponent<LayoutElement>().preferredHeight = 160;

            rateText = UIFactory.Label(header.transform, "0 Pixels/s", 46, Theme.Positive, TextAnchor.MiddleCenter, FontStyle.Bold);

            // --- Botão de clique manual
            var click = UIFactory.MakeButton(screen, "ClickButton", Theme.Accent, Theme.BuyDisabled,
                                             () => game.Click(), out Text clickLabel, 54, Theme.TextDark);
            clickLabel.text = "+ GERAR PIXEL";
            click.gameObject.AddComponent<LayoutElement>().preferredHeight = 170;

            // --- Título da seção
            var section = UIFactory.Label(screen, "Produtos", 40, Theme.TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            section.gameObject.AddComponent<LayoutElement>().preferredHeight = 56;

            // --- Lista rolável de produtos
            var scrollRT = UIFactory.Rect("ProductScroll", screen);
            var scrollImg = scrollRT.gameObject.AddComponent<Image>();
            scrollImg.color = Color.clear;                       // necessário para receber o arrasto
            scrollRT.gameObject.AddComponent<RectMask2D>();
            var scrollLE = scrollRT.gameObject.AddComponent<LayoutElement>();
            scrollLE.flexibleHeight = 1;
            scrollLE.minHeight = 300;

            var list = UIFactory.Rect("List", scrollRT);
            list.anchorMin = new Vector2(0, 1);
            list.anchorMax = new Vector2(1, 1);
            list.pivot = new Vector2(0.5f, 1);
            list.offsetMin = new Vector2(0, 0);
            list.offsetMax = new Vector2(0, 0);

            var lv = list.gameObject.AddComponent<VerticalLayoutGroup>();
            lv.spacing = 24;
            lv.padding = new RectOffset(0, 0, 0, 10);
            lv.childControlWidth = lv.childControlHeight = true;
            lv.childForceExpandWidth = true;
            lv.childForceExpandHeight = false;
            list.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = scrollRT.gameObject.AddComponent<ScrollRect>();
            scroll.content = list;
            scroll.viewport = scrollRT;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 40;

            foreach (var product in game.Purchases.Products)
                cards.Add(new ProductCardView(list, product, game.Purchases));

            return screen;
        }

        private RectTransform BuildPlaceholder(RectTransform parent, string title)
        {
            var screen = UIFactory.Rect("Screen_" + title, parent);
            UIFactory.Stretch(screen);

            var v = screen.gameObject.AddComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(60, 60, 60, 60);
            v.spacing = 24;
            v.childAlignment = TextAnchor.MiddleCenter;
            v.childControlWidth = v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;

            UIFactory.Label(screen, title, 80, Theme.TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            UIFactory.Label(screen, "Conteúdo em desenvolvimento...", 40, Theme.TextDim);

            return screen;
        }
    }
}
