// ----------------------------------------
// Script: TitleScreen.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-08 21:11:33
// Updated: 2026-02-08 21:15:45
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using ElMonosapiens.FlipEmCards.Core;
using ElMonosapiens.FlipEmCards.Gameplay;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class TitleScreen : MonoBehaviour
    {
        [SerializeField] private Button playerStartButton;
        [SerializeField] private Button cpuStartButton;

        private void Awake()
        {
            playerStartButton.onClick.AddListener(() => GameManager.Instance.StartGame(Turn.Player));
            cpuStartButton.onClick.AddListener(() => GameManager.Instance.StartGame(Turn.CPU));
        }
    }
}
