// ----------------------------------------
// Script: CardUtility.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Gameplay
// License: MIT
// Version: 1.0.0
// Created: 2026-02-07 01:21:29
// Updated: 2026-04-10 11:15:05
// Description: [Insert Description]
// ----------------------------------------

using System;
using System.Collections.Generic;
using ElMonosapiens.FlipEmCards.Core;

namespace ElMonosapiens.FlipEmCards.Gameplay
{
    public static class CardUtility
    {
        public static List<CardData> CreateCardPairs(CardData[] dataArray)
        {
            List<CardData> cardPairs = new();

            for (int i = 0; i < dataArray.Length; i++)
            {
                cardPairs.Add(dataArray[i]);
                cardPairs.Add(dataArray[i]);
            }

            return cardPairs;
        }

        /// <summary>
        /// Randomly assigns CardData entries to Card instances.
        /// Uses an unbiased Fisher–Yates shuffle to produce a uniform permutation.
        /// </summary>
        /// <param name="cards">Array of Card instances to receive data. Must not be null.</param>
        /// <param name="dataArray">Array of CardData to assign. Must not be null and length must be <= cards.Length.</param>
        public static void ShuffleCards(Card[] cards, CardData[] dataArray)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (dataArray == null) throw new ArgumentNullException(nameof(dataArray));
            if (dataArray.Length > cards.Length) throw new ArgumentException("dataArray length must be <= cards length", nameof(dataArray));

            // Create index array 0..n-1
            int n = cards.Length;
            int[] indices = new int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            // Fisher–Yates shuffle (unbiased version)
            for (int i = n - 1; i > 0; i--)
            {
                int randomNumber = UnityEngine.Random.Range(0, i + 1); // inclusive upper bound
                (indices[i], indices[randomNumber]) = (indices[randomNumber], indices[i]);
            }

            // Assign dataArray entries to cards using the shuffled indices
            for (int i = 0; i < dataArray.Length; i++)
            {
                cards[indices[i]].SetData(dataArray[i]);
            }
        }
    }
}
