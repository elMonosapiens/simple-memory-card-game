// ----------------------------------------
// Script: CPU.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Gameplay
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 19:48:24
// Updated: 2026-02-08 14:19:47
// Description: [Insert Description]
// ----------------------------------------

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ElMonosapiens.FlipEmCards.Core;
using UnityEngine;

namespace ElMonosapiens.FlipEmCards.Gameplay
{
    public class CPU : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float memoryDuration = 120f;
        [SerializeField] private int memoryCapacity = 10;

        [Tooltip("How many recent cards count as fresh.")]
        [SerializeField] private int recentCount = 5;

        [Tooltip("Percent chance to recall when small memory.")]
        [SerializeField] private int baseMemoryAccuracy = 80;

        private readonly List<Card> cardsInMemory = new();
        private Coroutine forgetCoroutine;

        private void OnEnable()
        {
            GameManager.Instance.OnGameStarted += HandleGameStart;
            TableManager.Instance.OnFreeToPlay += HandleFreeToPlay;
            TableManager.Instance.OnMatchFound += HandleMatchUpFound;
            TableManager.Instance.OnTurnEnded += HandleTurnEnded;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStarted -= HandleGameStart;
            TableManager.Instance.OnFreeToPlay -= HandleFreeToPlay;
            TableManager.Instance.OnMatchFound -= HandleMatchUpFound;
            TableManager.Instance.OnTurnEnded -= HandleTurnEnded;
        }

        private void StartMemoryTimer()
        {
            ClearMemory();
            StopMemoryTimer();
            forgetCoroutine = StartCoroutine(ForgetOldestPeriodically());
        }

        private IEnumerator ForgetOldestPeriodically()
        {
            while (true)
            {
                yield return new WaitForSeconds(memoryDuration);

                if (cardsInMemory.Count >= 2)
                {
                    Debug.Log($"<color=yellow>[CPU]</color> Forget cards: {cardsInMemory[^2].gameObject.name} ({cardsInMemory[^2].Label}), {cardsInMemory[^1].gameObject.name} ({cardsInMemory[^1].Label})");
                    cardsInMemory.RemoveAt(cardsInMemory.Count - 1);
                    cardsInMemory.RemoveAt(cardsInMemory.Count - 1);
                }
                else
                {
                    Debug.Log($"<color=yellow>[CPU]</color> Forget cards: {cardsInMemory[^1].gameObject.name} ({cardsInMemory[^1].Label})");
                    cardsInMemory.RemoveAt(cardsInMemory.Count - 1);
                }
            }
        }

        private void StopMemoryTimer()
        {
            if (forgetCoroutine != null)
            {
                StopCoroutine(forgetCoroutine);
                forgetCoroutine = null;
            }
        }

        public void ClearMemory() => cardsInMemory.Clear();

        // ====== BEHAVIOUR ======       
        private IEnumerator FlipPair(Card first, Card second)
        {
            yield return Flip(first);
            yield return Flip(second);
        }

        private IEnumerator FlipRandom()
        {
            yield return Wait();

            // Choose a random faced-down card
            if (TableManager.Instance.FacedDownCardsCount == 0) yield break;

            int idx = Random.Range(0, TableManager.Instance.FacedDownCardsCount);
            Card randomCard = TableManager.Instance.FacedDownCards[idx];

            yield return Flip(randomCard);

            if (TableManager.Instance.IsAllowedToFlipCards)
            {
                int accuracy = ComputeMemoryAccuracy();
                if (TryFindMatchFor(randomCard, recentCount, accuracy, out Card match))
                {
                    Debug.Log($"<color=yellow>[CPU]</color> Found a match in memory...");
                    yield return Flip(match);
                }
                else
                {
                    Debug.Log("<color=yellow>[CPU]</color> No match up found in memory, flipping another random card...");
                    yield return Wait();

                    // Flip another random card
                    StartCoroutine(FlipRandom());
                }
            }
        }

        private IEnumerator Flip(Card card)
        {
            yield return Wait(longer: true);
            TableManager.Instance.FlipCard(card);
        }

        private IEnumerator Wait(bool longer = false)
        {
            float min, max;

            if (longer)
            {
                min = 1f;
                max = 2f;
            }
            else
            {
                min = 0.5f;
                max = 0.8f;
            }

            yield return new WaitForSeconds(Random.Range(min, max));
        }

        // ====== MEMORY ======       
        private void Memorize(Card card)
        {
            if (card == null) return;

            // Always insert at front to be the freshest card
            if (cardsInMemory.Contains(card))
                cardsInMemory.Remove(card);

            cardsInMemory.Insert(0, card);

            // Trim memory if over capacity
            if (cardsInMemory.Count > memoryCapacity)
                cardsInMemory.RemoveAt(cardsInMemory.Count - 1);
        }

        private void RemoveFromMemory(Card card)
        {
            if (card == null) return;

            Debug.Log($"<color=yellow>[CPU]</color> Remove matched card from memory: {card.gameObject.name} ({card.Label})");
            cardsInMemory.Remove(card);
        }

        /// <summary>
        /// Attempts to find a matching pair of cards within the most recent entries of cardsInMemory.
        /// </summary>
        /// <param name="recentCount">Number of recent cards to search. Clamped to [1, cardsInMemory.Count].</param>
        /// <param name="first">Out parameter set to the first card in the matching pair if found; otherwise null.</param>
        /// <param name="second">Out parameter set to the second card in the matching pair if found; otherwise null.</param>
        /// <returns>True if a matching pair is found; false otherwise.</returns>
        private bool TryFindPairWithinRecent(int recentCount, out Card first, out Card second)
        {
            // Initialize out parameters
            first = null; second = null;

            // If there are no cards in memory, there is nothing to search
            if (cardsInMemory == null || cardsInMemory.Count == 0)
                return false;

            // Clamp the number of cards to search between 1 and the actual count
            int limit = Mathf.Clamp(recentCount, 1, cardsInMemory.Count);

            // Outer loop: iterate through each candidate card in the recent window
            for (int i = 0; i < limit; i++)
            {
                // Inner loop: compare the current card with all later cards in the window
                for (int ii = i + 1; ii < limit; ii++)
                {
                    if (cardsInMemory[i].Matches(cardsInMemory[ii]))
                    {
                        first = cardsInMemory[i];
                        second = cardsInMemory[ii];
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to find a matching card for the given flipped card
        /// within the most recent entries of cardsInMemory.
        /// The search only proceeds with probability defined by accuracyPercent.
        /// </summary>
        /// <param name="flipped">The card to find a match for.</param>
        /// <param name="recentCount">How many recent cards to search.</param>
        /// <param name="accuracyPercent">Chance (0–100) that the search will be attempted.</param>
        /// <param name="match">Out parameter set to the matching card if found; otherwise null.</param>
        /// <returns>True if a match was found; false otherwise.</returns>
        private bool TryFindMatchFor(Card flipped, int recentCount, int accuracyPercent, out Card match)
        {
            // Initialize out parameter
            match = null;

            // If no card was provided, nothing to match
            if (flipped == null) return false;

            // Accuracy check: only proceed within probability accuracyPercent            
            if (UnityEngine.Random.Range(0, 100) >= accuracyPercent)
                return false;

            // Clamp the number of cards to search between 1 and the actual count
            int limit = Mathf.Clamp(recentCount, 1, cardsInMemory.Count);

            // Iterate through the recent cards
            for (int i = 0; i < limit; i++)
            {
                // Skip comparing the flipped card with itself
                // If another card matches the flipped one, return it
                if (cardsInMemory[i] != flipped && cardsInMemory[i].Matches(flipped))
                {
                    match = cardsInMemory[i];
                    return true;
                }
            }

            return false;
        }

        private int ComputeMemoryAccuracy()
        {
            // Base accuracy plus small bonus for smaller memory
            int size = cardsInMemory.Count;
            // Smaller memory = slightly better recall
            int bonus = size == 0 ? 0 : Mathf.Clamp(10 - size, 0, 10);
            return Mathf.Clamp(baseMemoryAccuracy + bonus, 10, 100);
        }

        // ====== EVENT HANDLERS ======
        private void HandleGameStart(Turn turn) => StartMemoryTimer();

        private void HandleFreeToPlay(Turn turn)
        {
            if (turn == Turn.Player) return;

            // 1. Try to find a match up in memory
            if (TryFindPairWithinRecent(recentCount, out Card first, out Card second))
            {
                StartCoroutine(FlipPair(first, second));
                Debug.Log($"<color=yellow>[CPU]</color> Found match up in memory! ({first.Label})");
                return;
            }

            // 2. Flip a random card
            StartCoroutine(FlipRandom());
            Debug.Log("<color=yellow>[CPU]</color> No match up found in memory, flipping a random card...");
        }

        private void HandleTurnEnded(Card first, Card second)
        {
            Memorize(first);
            Memorize(second);
        }

        private void HandleMatchUpFound(Card first, Card second)
        {
            RemoveFromMemory(first);
            RemoveFromMemory(second);
        }
    }
}

