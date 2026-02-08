// ----------------------------------------
// Script: GameManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Core
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:41:37
// Updated: 2026-02-08 19:35:19
// Description: [Insert Description]
// ----------------------------------------

using System;
using UnityEngine;
using ElMonosapiens.FlipEmCards.UI;
using ElMonosapiens.FlipEmCards.Gameplay;
using System.Collections;

namespace ElMonosapiens.FlipEmCards.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameUIManager uiManager;
        [SerializeField] private GameObject menuContainer;
        [SerializeField] private GameObject gameContainer;

        [Header("Config")]
        [SerializeField] private float turnChangeDelay = 2f;

        public int PlayerPoints { get; private set; } = 0;
        public int CpuPoints { get; private set; } = 0;
        public Turn CurrentTurn { get; private set; }

        public event Action<Turn> OnGameStarted;
        public event Action<Turn> OnTurnChanged;
        public event Action<GameResult> OnGameResult;

        private void Awake() => MakethSingleton();
        private void Start() => ShowTitleScreen();

        private void MakethSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public void StartGame(Turn turn)
        {
            ResetScore();
            ShowGameScreen();

            OnGameStarted?.Invoke(turn);
        }

        public void RequestTurnChange()
        {
            Turn nextTurn = CurrentTurn == Turn.Player
                ? Turn.CPU
                : Turn.Player;

            if (nextTurn is Turn.Player)
                Debug.Log("<color=white>[GameManager]</color> Player turn...");
            else
                Debug.Log("<color=white>[GameManager]</color> CPU turn...");

            StartCoroutine(ChangeTurn(nextTurn));
        }

        private IEnumerator ChangeTurn(Turn nextTurn)
        {
            CurrentTurn = nextTurn;
            uiManager.AnnounceNextTurn(nextTurn);

            yield return new WaitForSeconds(turnChangeDelay);

            uiManager.HideAnnouncementPanel();

            OnTurnChanged?.Invoke(nextTurn);
        }

        public void EndGame()
        {
            ShowTitleScreen();
        }

        private void ShowGameScreen()
        {
            menuContainer.SetActive(false);
            gameContainer.SetActive(true);
        }

        private void ShowTitleScreen()
        {
            gameContainer.SetActive(false);
            menuContainer.SetActive(true);
        }

        public void UpdateScore()
        {
            if (CurrentTurn is Turn.Player) PlayerPoints++;
            else CpuPoints++;

            uiManager.UpdateScore(PlayerPoints, CpuPoints);
        }

        private void ResetScore()
        {
            PlayerPoints = CpuPoints = 0;
            uiManager.UpdateScore(PlayerPoints, CpuPoints);
        }

        public void DisplayResult(GameResult result)
        {
            OnGameResult?.Invoke(result);
            Invoke(nameof(EndGame), 3f);
        }
    }
}
