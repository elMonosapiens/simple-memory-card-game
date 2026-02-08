// ----------------------------------------
// Script: TableManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Gameplay
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 00:31:17
// Updated: 2026-02-08 19:08:51
// Description: [Insert Description]
// ----------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using ElMonosapiens.FlipEmCards.Core;

namespace ElMonosapiens.FlipEmCards.Gameplay
{
    public class TableManager : MonoBehaviour
    {
        private const int FLIP_THRESHOLD = 2;

        public static TableManager Instance { get; private set; }

        [Header("Cards")]
        [SerializeField] private Card[] cardArray;
        [SerializeField] private CardData[] cardDataArray;

        public bool IsAllowedToFlipCards => FlipCardsCount < FLIP_THRESHOLD;
        public int FlipCardsCount { get; private set; }
        public int FacedDownCardsCount => facedDownCards.Count;

        public IReadOnlyList<Card> FacedDownCards => facedDownCards;
        private readonly List<Card> facedDownCards = new();

        private Card firstCard;
        private Card secondCard;

        private int playerPoints = 0;
        private int cpuPoints = 0;

        public event Action<Turn> OnFreeToPlay;
        public event Action<Card, Card> OnMatchFound;
        public event Action<Card, Card> OnTurnEnded;

        // ====== UNITY LIFECYCLE ======
        private void Awake() => MakethSingleton();
        private void OnEnable() => GameManager.Instance.OnGameStarted += HandleGameStarted;
        private void OnDisable() => GameManager.Instance.OnGameStarted -= HandleGameStarted;

        // ====== INIT =====
        private void MakethSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        private void SetupDeck()
        {
            // 1. Create pair of the card data
            var deck = CardUtility.CreateCardPairs(cardDataArray);

            // 2. Shuffle and apply
            CardUtility.ShuffleCards(cardArray, deck.ToArray());

            foreach (var card in cardArray)
            {
                card.SetInteractable(true);
                card.Initialize(this);
            }

            // 3. Initialize facedDownCards
            facedDownCards.Clear();
            facedDownCards.AddRange(cardArray);
        }

        private void ListFacedDownCards()
        {
            facedDownCards.Clear();

            foreach (var card in cardArray)
            {
                if (!card.IsFaceUp)
                    facedDownCards.Add(card);
            }
        }

        // ====== TURN =======
        private void StartTurn(Turn turn)
        {
            // TODO: Announce turn start            
        }

        private void ResetTurn()
        {
            secondCard = null;
            firstCard = null;
            FlipCardsCount = 0;
        }

        private void EndTurn()
        {
            ListFacedDownCards();
            GameManager.Instance.RequestTurnChange();
        }

        private void EndGame() { }

        // ====== CARD ======
        public void FlipCard(Card card)
        {
            if (!CanFlipCard(card)) return;

            card.Flip();
            StoreFlippedCard(card);
            ListFacedDownCards();

            FlipCardsCount++;

            Debug.Log($"<color=cyan>[TableManager]</color> Card flipped: {card.gameObject.name} ({card.Label})");
        }

        public bool CanFlipCard(Card card)
        {
            if (!IsAllowedToFlipCards)
            {
                Debug.LogWarning("<color=cyan>[TableManager]</color> Cannot flip: waiting for match check.");
                return false;
            }

            if (card.IsFaceUp)
            {
                Debug.LogWarning("<color=cyan>[TableManager]</color> Cannot flip: card already facing up.");
                return false;
            }

            return true;
        }

        private void StoreFlippedCard(Card card)
        {
            if (firstCard == null) firstCard = card;
            else if (secondCard == null)
            {
                secondCard = card;
                Invoke(nameof(CheckMatchUp), 2f);
            }
        }

        private void CheckMatchUp()
        {
            if (firstCard.Matches(secondCard))
            {
                DisableCards();
                SetCardsMatchedOwnerLabel();

                GameManager.Instance.UpdateScore();

                EmitMatchFound(firstCard, secondCard);

                if (FacedDownCardsCount == 0)
                    ComputeGameResult();
                else
                    EmitFreeToPlay();
            }
            else
            {
                EmitTurnEnded(firstCard, secondCard); ;
                FaceCardsDown();
            }

            ResetTurn();
        }

        private void DisableCards()
        {
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);
        }

        private void SetCardsMatchedOwnerLabel()
        {
            Turn currentTurn = GameManager.Instance.CurrentTurn;
            firstCard.SetMatchOwnerText(currentTurn);
            secondCard.SetMatchOwnerText(currentTurn);
        }

        private void FaceCardsDown()
        {
            firstCard.Flip();
            secondCard.Flip();

            Invoke(nameof(EndTurn), 1.5f);
        }

        private void ComputeGameResult()
        {
            GameResult result;

            if (playerPoints > cpuPoints) result = GameResult.PlayerWin;
            else if (playerPoints < cpuPoints) result = GameResult.CpuWin;
            else result = GameResult.Draw;

            GameManager.Instance.DisplayResult(result);
        }

        // ====== EVENT HANDLERS ======
        private void HandleGameStarted(Turn turn)
        {
            SetupDeck();
            StartTurn(turn);
        }

        // ====== EMITTERS ======
        private void EmitFreeToPlay() => OnFreeToPlay?.Invoke(GameManager.Instance.CurrentTurn);
        private void EmitTurnEnded(Card first, Card second) => OnTurnEnded?.Invoke(first, second);
        private void EmitMatchFound(Card first, Card second) => OnMatchFound?.Invoke(first, second);

    }
}
