// ----------------------------------------
// Script: TitleScreen.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-08 21:11:33
// Updated: 2026-02-09 02:12:17
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ElMonosapiens.FlipEmCards.Core;
using ElMonosapiens.FlipEmCards.Gameplay;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class TitleScreen : MonoBehaviour
    {
        [SerializeField] private Button playerStartButton;
        [SerializeField] private Button cpuStartButton;
        [SerializeField] private Button openRepoButton;
        [SerializeField] private TextMeshProUGUI developerNameText;
        [SerializeField] private TextMeshProUGUI versionText;

        private void Awake()
        {
            playerStartButton.onClick.AddListener(() => GameManager.Instance.StartGame(Turn.Player));
            cpuStartButton.onClick.AddListener(() => GameManager.Instance.StartGame(Turn.CPU));
            openRepoButton.onClick.AddListener(OpenRepoOnWeb);
        }

        private void Start()
        {
            developerNameText.text = $"Made by {Application.companyName}";
            versionText.text = $"v{Application.version}";
        }

        private void OpenRepoOnWeb()
        {
            Application.OpenURL("https://github.com/elMonosapiens/SimpleMemoryCardGame");
        }
    }
}
