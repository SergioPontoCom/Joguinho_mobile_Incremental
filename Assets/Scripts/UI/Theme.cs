using UnityEngine;

namespace PixelIdle.UI
{
    /// <summary>Paleta de cores central. Mude aqui para mudar o visual do jogo todo.</summary>
    public static class Theme
    {
        public static readonly Color Background  = Hex("#0A0D16");
        public static readonly Color Card        = Hex("#151A2A");
        public static readonly Color NavBar      = Hex("#0F1320");
        public static readonly Color Accent      = Hex("#3DDCFF");
        public static readonly Color AccentDim   = Hex("#16324A");
        public static readonly Color Buy         = Hex("#2ECC71");
        public static readonly Color BuyDisabled = Hex("#2B3145");
        public static readonly Color TextPrimary = Hex("#F2F5FF");
        public static readonly Color TextDim     = Hex("#8E97B3");
        public static readonly Color TextDark    = Hex("#07131A");
        public static readonly Color Positive    = Hex("#5CE08A");

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var c);
            return c;
        }
    }
}
