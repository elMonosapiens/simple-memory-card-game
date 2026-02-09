// ----------------------------------------
// Script: Card.cs
// Author: elMonosapiens
// Project: ElMonosapiens.FlipEmCards
// Module: Gameplay
// License: MIT
// Version: 1.0.0
// Created: 2026-02-06 21:53:32
// Updated: 2026-02-08 19:58:19
// Description: [Insert Description]
// ----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ElMonosapiens.FlipEmCards.Core;
using System;

namespace ElMonosapiens.FlipEmCards.Gameplay
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class Card : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Sprite backSprite;
        [SerializeField] private TextMeshProUGUI matchOwnerText;

        [Header("Config")]
        [SerializeField] private float scaledUpValue = 1.2f;
        [SerializeField] private float scaleUpDuration = 0.5f;
        [SerializeField] private float spriteChangeDuration = 0.1f;
        [SerializeField] private float scaleDownDuration = 0.25f;
        [SerializeField] private Ease scaleUpEase = Ease.Linear;

        public string Label => data.Label;
        public bool IsFaceUp { get; private set; }

        private TableManager tableManager;
        private CardData data;
        private Sequence flipSequence;
        private Button button;
        private Image image;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();

            button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy() => KillTweens();

        public void SetData(CardData newData) => data = newData;
        public void SetInteractable(bool isEnabled) => button.interactable = isEnabled;

        public void Initialize(TableManager manager)
        {
            tableManager = manager;

            IsFaceUp = false;
            image.sprite = backSprite;
            matchOwnerText.gameObject.SetActive(false);
        }

        public void Flip(Action onComplete = null)
        {
            IsFaceUp = !IsFaceUp;

            KillTweens();

            flipSequence = DOTween.Sequence();
            flipSequence.Append(transform.DOScale(scaledUpValue, scaleUpDuration)).SetEase(scaleUpEase);
            flipSequence.Append(transform.DOScaleX(0, spriteChangeDuration));
            flipSequence.AppendCallback(UpdateSprite);
            flipSequence.Append(transform.DOScaleX(scaledUpValue, spriteChangeDuration));
            flipSequence.Append(transform.DOScale(Vector3.one, scaleDownDuration));
            flipSequence.AppendCallback(() => onComplete?.Invoke());
        }

        public void UpdateSprite() =>
            image.sprite = IsFaceUp ? data.Artwork : backSprite;

        public bool Matches(Card card) =>
            data.Label == card.data.Label;

        public void SetMatchOwnerText(Turn turn)
        {
            matchOwnerText.text = (turn == Turn.Player) ? "YOU" : "CPU";
            matchOwnerText.gameObject.SetActive(true);
        }

        private void OnClicked()
        {
            if (GameManager.Instance.CurrentTurn is Turn.Player)
                tableManager.FlipCard(this);
        }

        private void KillTweens()
        {
            flipSequence?.Kill();
            if (transform != null) transform.DOKill();
        }
    }
}
