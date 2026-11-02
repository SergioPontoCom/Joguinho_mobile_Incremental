using System;

namespace PixelIdle.Core
{
    /// <summary>Sistema de pixels: guarda o saldo e avisa quando ele muda.</summary>
    public class PixelWallet
    {
        public double Pixels { get; private set; }
        public double TotalEarned { get; private set; }

        public event Action Changed;

        public void Add(double amount)
        {
            if (amount <= 0) return;
            Pixels += amount;
            TotalEarned += amount;
            Changed?.Invoke();
        }

        public bool CanAfford(double cost) => Pixels >= cost;

        public bool TrySpend(double cost)
        {
            if (!CanAfford(cost)) return false;
            Pixels -= cost;
            Changed?.Invoke();
            return true;
        }
    }
}
