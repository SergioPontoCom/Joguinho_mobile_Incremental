using System;
using System.Collections.Generic;

namespace PixelIdle.Core
{
    /// <summary>
    /// Geração passiva baseada em timer: acumula o tempo e, a cada tick (0,1 s),
    /// deposita os pixels produzidos na carteira.
    /// </summary>
    public class PassiveGenerationSystem
    {
        private const float TickInterval = 0.1f;

        private readonly PixelWallet wallet;
        private readonly IReadOnlyList<Product> products;
        private readonly double baseRate;
        private float accumulator;

        public double PixelsPerSecond { get; private set; }
        public event Action RateChanged;

        public PassiveGenerationSystem(PixelWallet wallet, IReadOnlyList<Product> products, double baseRate)
        {
            this.wallet = wallet;
            this.products = products;
            this.baseRate = baseRate;
            Recalculate();
        }

        /// <summary>Recalcula a produção total (base + produtos). Chame após qualquer compra.</summary>
        public void Recalculate()
        {
            double rate = baseRate;
            foreach (var p in products) rate += p.TotalProduction;
            PixelsPerSecond = rate;
            RateChanged?.Invoke();
        }

        /// <summary>Deve ser chamado todo frame com Time.deltaTime.</summary>
        public void Tick(float deltaTime)
        {
            accumulator += deltaTime;
            int ticks = (int)(accumulator / TickInterval);
            if (ticks <= 0) return;

            accumulator -= ticks * TickInterval;
            wallet.Add(PixelsPerSecond * TickInterval * ticks);
        }
    }
}
