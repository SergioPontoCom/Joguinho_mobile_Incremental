using System;

namespace PixelIdle.Core
{
    /// <summary>Dados e estado de um produto comprável.</summary>
    public class Product
    {
        public string Name { get; }
        public string Description { get; }
        public double BaseCost { get; }
        public double CostGrowth { get; }
        public double PixelsPerSecond { get; }   // produção por unidade comprada
        public int Owned { get; set; }

        public Product(string name, string description, double baseCost, double costGrowth, double pixelsPerSecond)
        {
            Name = name;
            Description = description;
            BaseCost = baseCost;
            CostGrowth = costGrowth;
            PixelsPerSecond = pixelsPerSecond;
        }

        /// <summary>Preço atual: aumenta a cada unidade comprada.</summary>
        public double Cost => Math.Ceiling(BaseCost * Math.Pow(CostGrowth, Owned));

        public double TotalProduction => Owned * PixelsPerSecond;
    }
}
