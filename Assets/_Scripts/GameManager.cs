// ----------------------------------------
// Script: GameManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Core
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:41:37
// Updated: 2026-02-08 23:51:31
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
        [SerializeField] private GameObject titleScreenGO;
        [SerializeField] private GameObject gameScreenGO;
        [SerializeField] private GameObject blockInputPanel;

        [Header("Config")]
        [SerializeField] private float turnChangeDelay = 2f;

        public int PlayerPoints { get; private set; } = 0;
        public int CpuPoints { get; private set; } = 0;
        public Turn CurrentTurn { get; private set; }

        public event Action OnGameStarted;
        public event Action OnGameEnded;
        public event Action<Turn> OnTurnChanged;
        public event Action<Turn> OnExtraTurn;
        public event Action<Card, Card> OnTurnEnded;

        // ====== UNITY LIFECYCLE ======
        private void Awake() => MakethSingleton();

        private void Start()
        {
            ShowTitleScreen();
            TableManager.Instance.OnMatchFound += HandleMatchFound;
        }

        private void OnDestroy() =>
            TableManager.Instance.OnMatchFound -= HandleMatchFound;

        // ====== INIT ======
        private void MakethSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        // ====== GAMEPLAY ======
        public void StartGame(Turn turn)
        {
            ResetScore();
            ShowGameScreen();
            StartCoroutine(ChangeTurn(turn));

            OnGameStarted?.Invoke();
        }

        public void EndTurn(Card firstFlip, Card secondFlip)
        {
            OnTurnEnded?.Invoke(firstFlip, secondFlip);
        }

        public void RequestTurnChange()
        {
            Turn nextTurn = CurrentTurn == Turn.Player
                ? Turn.CPU
                : Turn.Player;

            StartCoroutine(ChangeTurn(nextTurn));
        }

        private IEnumerator ChangeTurn(Turn nextTurn)
        {
            if (nextTurn is Turn.Player)
            {
                Debug.Log("<color=white>[GameManager]</color> Player turn...");
                blockInputPanel.SetActive(false);
            }
            else
            {
                Debug.Log("<color=white>[GameManager]</color> CPU turn...");
                blockInputPanel.SetActive(true);
            }

            CurrentTurn = nextTurn;
            uiManager.AnnounceNextTurn(nextTurn);

            yield return new WaitForSeconds(turnChangeDelay);

            uiManager.HideAnnouncementPanel();

            OnTurnChanged?.Invoke(nextTurn);
        }

        public void ContinuePlaying()
        {
            OnExtraTurn?.Invoke(CurrentTurn);
        }

        public void ComputeResult()
        {
            GameResult result;

            if (PlayerPoints > CpuPoints) result = GameResult.PlayerWin;
            else if (PlayerPoints < CpuPoints) result = GameResult.CpuWin;
            else result = GameResult.Draw;

            DisplayResult(result);
        }

        public void EndGame()
        {
            uiManager.HideAnnouncementPanel();
            ShowTitleScreen();
            OnGameEnded?.Invoke();
        }

        private void ShowGameScreen()
        {
            titleScreenGO.SetActive(false);
            gameScreenGO.SetActive(true);
        }

        private void ShowTitleScreen()
        {
            blockInputPanel.SetActive(false);
            gameScreenGO.SetActive(false);
            titleScreenGO.SetActive(true);
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
            uiManager.AnnounceGameResult(result);
            Invoke(nameof(EndGame), 3f);
        }

        private void HandleMatchFound(Card firstCard, Card secondCard)
        {
            UpdateScore();
        }
    }
}
