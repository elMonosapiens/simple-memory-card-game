// ----------------------------------------
// Script: TableManager.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Gameplay
// License: MIT
// Version: 1.0.1
// Created: 2026-02-07 00:31:17
// Updated: 2026-04-10 12:27:24
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
        public int TotalCardsCount => cardArray.Length;
        private bool IsAllCardsFaceUp => facedDownCards.Count == 0;

        public IReadOnlyList<Card> FacedDownCards => facedDownCards;
        private readonly List<Card> facedDownCards = new();

        private Card firstCard;
        private Card secondCard;

        public event Action<Card, Card> OnMatchFound;

        // ====== UNITY LIFECYCLE ======
        private void Awake() => MakethSingleton();

        private void OnEnable()
        {
            GameManager.Instance.OnGameStarted += HandleGameStarted;
            GameManager.Instance.OnTurnEnded += HandleTurnEnded;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
            GameManager.Instance.OnTurnEnded -= HandleTurnEnded;
        }

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

        private void ClearTurnValues()
        {
            secondCard = null;
            firstCard = null;
            FlipCardsCount = 0;
        }

        // ====== CARD ======
        public void RequestCardFlip(Card card)
        {
            if (!CanFlipCard(card)) return;

            card.Flip(onComplete: () =>
            {
                // Store the card after the flip completion (has delay), that way, the CheckMatchUp will be in sync
                StoreFlippedCard(card);
            });

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
                CheckMatchUp();
            }
        }

        private void CheckMatchUp()
        {
            if (firstCard.Matches(secondCard))
            {
                DisableCards();
                SetCardsMatchedOwnerLabel();
                EmitMatchFound(firstCard, secondCard);

                ClearTurnValues();

                if (IsAllCardsFaceUp)
                    GameManager.Instance.ComputeResult();
                else
                    GameManager.Instance.ContinuePlaying();
            }
            else // If no match up, end turn
            {
                GameManager.Instance.EndTurn(firstCard, secondCard);
            }
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

        // ====== EVENT HANDLERS ======
        private void HandleGameStarted()
        {
            SetupDeck();
            ClearTurnValues();
        }

        private void HandleTurnEnded(Card firstCard, Card secondCard)
        {
            firstCard.Flip();
            secondCard.Flip(onComplete: HandleCardsFaceDownComplete);
        }

        private void HandleCardsFaceDownComplete()
        {
            // Request turn change first before clearing current FLipCardsCount
            // This avoids the player being able to flip another card right after the turn end
            GameManager.Instance.RequestTurnChange();

            ListFacedDownCards();
            ClearTurnValues();
        }

        // ====== EMITTERS ======        
        private void EmitMatchFound(Card first, Card second) => OnMatchFound?.Invoke(first, second);

    }
}
