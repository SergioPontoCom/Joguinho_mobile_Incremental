using UnityEngine;
using UnityEngine.UI;
using PixelIdle.Core;

namespace PixelIdle.UI
{
    /// <summary>Card visual de um produto: nome, descrição, ganho, quantidade e botão de compra.</summary>
    public class ProductCardView
    {
        private readonly Product product;
        private readonly PurchaseSystem purchases;
        private readonly Text ownedText;
        private readonly Text buyText;
        private readonly Button buyButton;

        public ProductCardView(Transform parent, Product product, PurchaseSystem purchases)
        {
            this.product = product;
            this.purchases = purchases;

            var card = UIFactory.Panel(parent, "Card_" + product.Name, Theme.Card);
            card.gameObject.AddComponent<LayoutElement>().preferredHeight = 270;
            var t = card.transform;

            // Coluna esquerda: nome, descrição, ganho
            var name = UIFactory.Label(t, product.Name, 44, Theme.TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            UIFactory.Anchor(name.rectTransform, 0, 0.62f, 0.60f, 1, 32, 0, 0, 16);

            var desc = UIFactory.Label(t, product.Description, 28, Theme.TextDim, TextAnchor.UpperLeft);
            UIFactory.Anchor(desc.rectTransform, 0, 0.28f, 0.60f, 0.62f, 32, 0, 0, 0);

            var gain = UIFactory.Label(t, "+" + NumberFormatter.FormatRate(product.PixelsPerSecond) + " Pixels/s por compra",
                                       28, Theme.Positive, TextAnchor.MiddleLeft, FontStyle.Bold);
            UIFactory.Anchor(gain.rectTransform, 0, 0, 0.60f, 0.28f, 32, 14, 0, 0);

            // Coluna direita: quantidade e botão
            ownedText = UIFactory.Label(t, "", 32, Theme.Accent, TextAnchor.MiddleRight, FontStyle.Bold);
            UIFactory.Anchor(ownedText.rectTransform, 0.60f, 0.55f, 1, 1, 0, 0, 28, 14);

            buyButton = UIFactory.MakeButton(t, "BuyButton", Theme.Buy, Theme.BuyDisabled,
                                             OnBuyClicked, out buyText, 34, Theme.TextDark);
            UIFactory.Anchor(buyButton.GetComponent<RectTransform>(), 0.62f, 0, 1, 0.52f, 0, 24, 24, 0);
        }

        public void Refresh(double pixels)
        {
            double cost = product.Cost;
            bool canBuy = pixels >= cost;

            buyButton.interactable = canBuy;
            buyText.color = canBuy ? Theme.TextDark : Theme.TextDim;
            buyText.text = "Comprar\n" + NumberFormatter.Format(cost) + " px";

            ownedText.text = "Qtd: " + product.Owned + "\n+" + NumberFormatter.FormatRate(product.TotalProduction) + " /s";
        }

        private void OnBuyClicked() => purchases.TryBuy(product);
    }
}
