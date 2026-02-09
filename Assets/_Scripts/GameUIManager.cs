// ----------------------------------------
// Script: GameUIManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:42:08
// Updated: 2026-02-08 22:39:02
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using ElMonosapiens.FlipEmCards.Gameplay;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private AnnouncementPanel announcementPanel;
        [SerializeField] private ScoreDisplay scoreDisplay;

        public void UpdateScore(int playerPoints, int cpuPoints)
        {
            string score = $"YOU: {playerPoints} | CPU: {cpuPoints}";
            scoreDisplay.UpdateScore(score);
        }

        public void AnnounceNextTurn(Turn nextTurn) =>
            announcementPanel.ShowTurn(nextTurn);

        public void AnnounceGameResult(GameResult result) =>
            announcementPanel.ShowResult(result);

        public void HideAnnouncementPanel() =>
            announcementPanel.Hide();
    }
}
