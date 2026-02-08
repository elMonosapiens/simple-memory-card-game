// ----------------------------------------
// Script: GameUIManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:42:08
// Updated: 2026-02-08 19:38:15
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ElMonosapiens.FlipEmCards.Core;
using ElMonosapiens.FlipEmCards.Gameplay;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private AnnouncementPanel announcementPanel;
        [SerializeField] private ScoreDisplay scoreDisplay;

        public void AnnounceNextTurn(Turn nextTurn)
        {
            announcementPanel.ShowTurn(nextTurn);
        }

        public void UpdateScore(int playerPoints, int cpuPoints)
        {
            string score = $"YOU: {playerPoints} | CPU: {cpuPoints}";
            scoreDisplay.UpdateScore(score);
        }

        public void AnnounceGameResult(GameResult result)
        {
            announcementPanel.ShowResult(result);
        }

        public void HideAnnouncementPanel()
        {
            announcementPanel.Hide();
        }
    }
}
