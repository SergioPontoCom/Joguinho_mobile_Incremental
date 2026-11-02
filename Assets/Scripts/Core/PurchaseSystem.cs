using System;
using System.Collections.Generic;

namespace PixelIdle.Core
{
    /// <summary>Sistema de compras: valida, desconta o custo e atualiza a produção.</summary>
    public class PurchaseSystem
    {
        private readonly PixelWallet wallet;
        private readonly PassiveGenerationSystem generator;

        public IReadOnlyList<Product> Products { get; }
        public event Action<Product> Purchased;

        public PurchaseSystem(PixelWallet wallet, IReadOnlyList<Product> products, PassiveGenerationSystem generator)
        {
            this.wallet = wallet;
            this.generator = generator;
            Products = products;
        }

        public bool CanBuy(Product product) => wallet.CanAfford(product.Cost);

        public bool TryBuy(Product product)
        {
            if (!wallet.TrySpend(product.Cost)) return false;

            product.Owned++;
            generator.Recalculate();   // produção sobe imediatamente
            Purchased?.Invoke(product);
            return true;
        }
    }
}
