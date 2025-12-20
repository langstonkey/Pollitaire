using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public UnityEvent<Card> BeginDragEvent = new UnityEvent<Card>();
    public UnityEvent<Card> EndDragEvent = new UnityEvent<Card>();
    public UnityEvent<Card> PointerEnterEvent = new UnityEvent<Card>();
    public UnityEvent<Card> PointerExitEvent = new UnityEvent<Card>();
    public UnityEvent<Card, bool> SelectEvent = new UnityEvent<Card, bool>();
    public UnityEvent<Card> PointerUpEvent = new UnityEvent<Card>();

    public Card selectedCard;
    [field: SerializeField] public Transform CardVisualRoot { get; private set; }
    [field: SerializeField] public float LayoutScale { get; private set; }

    [SerializeField] List<CardGroup> groups = new List<CardGroup>();
    [SerializeField] GridLayout[] groupLayouts;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        groups = GetComponentsInChildren<CardGroup>().ToList();

        foreach (CardGroup group in groups)
        {
            group.transform.localScale = Vector3.one * LayoutScale;
        }

        foreach (GridLayout layout in groupLayouts)
        {

        }
    }
}
