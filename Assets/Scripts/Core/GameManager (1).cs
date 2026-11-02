using System.Collections.Generic;
using UnityEngine;
using PixelIdle.UI;

namespace PixelIdle.Core
{
    /// <summary>
    /// Ponto de entrada. Cria os sistemas, liga o timer da geração passiva
    /// e instancia a interface. Coloque este script em um GameObject vazio da cena.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private double basePixelsPerSecond = 1;
        [SerializeField] private double pixelsPerClick = 1;

        [Header("Áudio")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;

        public PixelWallet Wallet { get; private set; }
        public PassiveGenerationSystem Generator { get; private set; }
        public PurchaseSystem Purchases { get; private set; }
        public double PixelsPerClick => pixelsPerClick;
        public MusicPlayer Music { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;

            var products = new List<Product>
            {
                new Product("Pixel Semente",     "Um pixel solitário que gera outros pixels.",  10,   1.15, 1),
                new Product("Cluster de Pixels", "Grupo de pixels que cresce em conjunto.",     100,  1.15, 8),
                new Product("Fábrica de Pixels", "Produção industrial de pixels em série.",     1100, 1.15, 50),
            };

            Wallet = new PixelWallet();
            Generator = new PassiveGenerationSystem(Wallet, products, basePixelsPerSecond);
            Purchases = new PurchaseSystem(Wallet, products, Generator);

            Music = MusicPlayer.Create(backgroundMusic, musicVolume);
        }

        private void Start()
        {
            new GameObject("GameUI").AddComponent<GameUI>().Init(this);
        }

        private void Update()
        {
            Generator.Tick(Time.deltaTime);
        }

        public void Click()
        {
            Wallet.Add(pixelsPerClick);
        }
    }
}
