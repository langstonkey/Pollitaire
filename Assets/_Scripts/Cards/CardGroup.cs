using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardGroup : MonoBehaviour, IDropHandler
{
    [Header("References")]
    [field: SerializeField] public RectTransform CardRoot { get; private set; }

    [Header("Settings")]
    [SerializeField] int cardsInGroup;

    [Header("Runtime")]
    [SerializeField, ReadOnly] List<Card> cards = new List<Card>();

    public UnityEvent<Card> OnCardRemoved = new UnityEvent<Card>();
    public UnityEvent<Card>  OnCardAdded = new UnityEvent<Card>();

    [Header("Debug")]
    [SerializeField] bool verbose;

    private void Start()
    {
        cards.Clear();

        //init the group with any cards current in the group
        for (int i = 0; i < CardRoot.childCount; i++)
        {
            AddCard(CardRoot.GetChild(i).GetComponentInChildren<Card>());
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        if (card)
        {
            TryLog($"Card {card.name} Dropped On {name} Group");
            TransferCard(card);
        }
    }

    public void TransferCard(Card card)
    {
        //ignore if the card is already in the group
        if (cards.Contains(card))
        {
            TryLog($"Group {name} already has {card.name}");
            return;
        }

        //ignore if there are more cards in the list than cards in group
        if (cards.Count >= cardsInGroup)
        {
            TryLog($"Group {name} has max cards");
            return;
        }

        //remove the card from its old group and add it to this one
        card.GetGroup().RemoveCard(card);
        AddCard(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        OnCardRemoved?.Invoke(card);
    }

    public void DestroyCard(Card card)
    {
        cards.Remove(card);
        Destroy(card.gameObject);
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        card.SetGroup(this);
        OnCardAdded?.Invoke(card);
    }

    public void ClearGroup()
    {
        Card[] cardsToRemove = cards.ToArray();
        for (int i = 0; i < cardsToRemove.Length; i++)
        {
            DestroyCard(cardsToRemove[i]);
        }
    }

    void TryLog(string message)
    {
        if (verbose) Debug.Log(message);
    }
}
