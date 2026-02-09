// ----------------------------------------
// Script: GameUIManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:42:08
// Updated: 2026-02-09 00:51:21
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using ElMonosapiens.FlipEmCards.Gameplay;
using ElMonosapiens.FlipEmCards.Core;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private AnnouncementPanel announcementPanel;
        [SerializeField] private ScoreDisplay scoreDisplay;
        [SerializeField] private Button quitMatchButton;

        private void Awake()
        {
            quitMatchButton.onClick.AddListener(GameManager.Instance.EndGame);
        }

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
