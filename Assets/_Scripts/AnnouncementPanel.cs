// ----------------------------------------
// Script: AnnouncementPanel.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-08 15:15:38
// Updated: 2026-02-08 22:38:06
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using TMPro;
using ElMonosapiens.FlipEmCards.Gameplay;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class AnnouncementPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject transparentPanel;

        public void ShowTurn(Turn turn)
        {
            transparentPanel.SetActive(true);

            if (turn is Turn.Player) DisplayMessage("YOUR TURN");
            else DisplayMessage("CPU TURN");
        }

        public void ShowResult(GameResult result)
        {
            transparentPanel.SetActive(true);

            switch (result)
            {
                case GameResult.PlayerWin: DisplayMessage("YOU WIN!"); break;
                case GameResult.CpuWin: DisplayMessage("YOU LOSE..."); break;
                case GameResult.Draw: DisplayMessage("IT'S A DRAW"); break;
            }
        }

        public void Hide()
        {
            transparentPanel.SetActive(false);
            messageText.text = "";
        }

        private void DisplayMessage(string message)
        {
            messageText.text = message;
        }
    }
}
