// ----------------------------------------
// Script: ScoreDisplay.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: UI
// License: MIT
// Version: 1.0.0
// Created: 2026-02-08 15:15:16
// Updated: 2026-02-08 19:38:15
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using TMPro;

namespace ElMonosapiens.FlipEmCards.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        public void UpdateScore(string score)
        {
            scoreText.text = score;
        }
    }
}
