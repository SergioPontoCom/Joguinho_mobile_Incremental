using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelIdle.UI
{
    /// <summary>Controla qual tela está visível e o destaque da aba ativa.</summary>
    public class NavigationController
    {
        private class Tab
        {
            public Image Background;
            public Text Label;
            public GameObject Screen;
        }

        private readonly List<Tab> tabs = new List<Tab>();

        public void Add(Button button, Text label, GameObject screen)
        {
            int index = tabs.Count;
            tabs.Add(new Tab { Background = button.image, Label = label, Screen = screen });
            button.onClick.AddListener(() => Select(index));
        }

        public void Select(int index)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                bool active = i == index;
                tabs[i].Screen.SetActive(active);
                tabs[i].Background.color = active ? Theme.AccentDim : Color.clear;
                tabs[i].Label.color = active ? Theme.Accent : Theme.TextDim;
            }
        }
    }
}
