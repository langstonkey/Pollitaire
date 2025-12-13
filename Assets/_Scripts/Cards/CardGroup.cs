
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class CardGroup : MonoBehaviour, IDropHandler
{
    [field: SerializeField] public bool isHorizontal { get; private set; }
    [Header("Spawn Settings")]
    [Tooltip("The transform where the card visual group will be instantiated")]
    [SerializeField] private Transform cardVisualRoot;
    [Tooltip("The prefab that will be instantiated to hold the card script & gameobject")]
    [SerializeField] private GameObject cardRootPrefab;
    [Tooltip("Empty group prefab to hold all teh visuals assoicated with this group together in the hierarchy")]
    [SerializeField] private GameObject cardVisualGroupPrefab;
    [Tooltip("TEMP VAR number of cards to spawn at runtime")]
    [SerializeField] int cardsToSpawn;

    [field: Header("Runtime Variables")]
    [field: Tooltip("List of cards associated with this card group")]
    [field: SerializeField] public List<Card> cards { get; private set; } = new();
    [Tooltip("Wether or not cards within this group can re-ordered")]
    [SerializeField] private bool swappable = true;
    [SerializeField] private bool canDragInCards = true;
    [Tooltip("the parent of all card visuals associated with this card group")]
    [HideInInspector] Transform cardVisualGroupRoot;
    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> CardAdded;
    [HideInInspector] public UnityEvent<Card> CardRemoved;
    //whether or not cards are  currently swapping parents
    bool isSwapping = false;
    private void Awake()
    {
        //create a card visual group that will be associated with this card group
        Debug.Log($"CardManager is null? {CardManager.Instance == null}");
    }
    private void Start()
    {
        //Debug.Log($"card group starting {gameObject.name} {transform.position}");
        cardVisualGroupRoot = Instantiate(cardVisualGroupPrefab, cardVisualRoot).transform;
        //if this is the first time this card group gets activated, it may need to init visuals
        foreach (Card card in cards)
        {
            if (card.cardVisual == null)
            {
                card.cardVisualRoot = cardVisualGroupRoot;
                card.InstantiateVisual();
            }
        }
        //init list to spawn
        if (cardsToSpawn > 0)
        {
            Debug.Log("Cards to sapwn");
            cards = new List<Card>();
            for (int i = 0; i < cardsToSpawn; i++)
            {
                //spawn card
                Card card = Instantiate(cardRootPrefab, transform).GetComponentInChildren<Card>();
                //set its visual root to the one assoicated with this card group
                card.cardVisualRoot = cardVisualGroupRoot;
                card.SetUp();
                card.InstantiateVisual();
                //add to the list
                AddCardToList(card);
            }
        }
    }
    public void Update()
    {
        //if this card group isn't swappable return
        if (!swappable) return;
        Card selectedCard = CardManager.Instance.selectedCard;
        //if there is no card selected return
        if (selectedCard == null) return;
        //if the card selected isn't in this group return
        if (!cards.Contains(selectedCard)) return;
        //if the card is currently being swapped return
        if (isSwapping) return;
        if (isHorizontal)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (selectedCard.transform.position.x > cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                    {
                        Swap(i, selectedCard);
                        break;
                    }
                }
                if (selectedCard.transform.position.x < cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                    {
                        Swap(i, selectedCard);
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (selectedCard.transform.position.y > cards[i].transform.position.y)
                {
                    if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                    {
                        Swap(i, selectedCard);
                        break;
                    }
                }
                if (selectedCard.transform.position.y < cards[i].transform.position.y)
                {
                    if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                    {
                        Swap(i, selectedCard);
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// swaps a card of a specified index with the currently selected card
    /// </summary>
    /// <param name="index">index of the card to swap</param>
    void Swap(int index, Card selectedCard)
    {
        isSwapping = true;
        Transform focusedParent = selectedCard.transform.parent;
        Transform swappedParent = cards[index].transform.parent;
        cards[index].transform.SetParent(focusedParent);
        selectedCard.transform.SetParent(swappedParent);
        cards[index].transform.localPosition = Vector3.zero;
        //when this is true, there is an issue where only some cards 'detect' the card passing over it as its moving too fast
        if (Mathf.Abs(index - selectedCard.ParentIndex()) > 1)

        isSwapping = false;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.selectedObject == null) return;
        Card card = eventData.selectedObject.GetComponent<Card>();
        if (card != null && canDragInCards && card.TryChangeCardGroup(this))
        {
            AddCard(card);
        }
        else
        {
            Debug.Log("Cannot add card to this group");
        }
    }
    private void OnDisable()
    {
        //disable the visuals associated with this card group
        if (cardVisualGroupRoot != null) cardVisualGroupRoot.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        //enable the visuals associated with this card group
        if (cardVisualGroupRoot == null) return;
        cardVisualGroupRoot.gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        //clean up associated visuals
        if (cardVisualGroupRoot != null) Destroy(cardVisualGroupRoot.gameObject);
    }
    public void DeleteCard(Card card)
    {
        Debug.Log("Deleting Card");
        //remove from the list
        RemoveCardFromList(card);
        //destory the game object
        Destroy(card.transform.parent.gameObject);
        Destroy(card.gameObject);
    }
    public void ClearGroup()
    {
        Debug.Log($"Clearing Group {gameObject.name}");
        Card[] cardsToClear = cards.ToArray();
        foreach (Card card in cardsToClear)
        {
            DeleteCard(card);
        }
    }
    public void SpawnCard(GameObject cardPrefab)
    {
        if (cardPrefab.GetComponentInChildren<Card>() == null) { Debug.LogWarning("Spawn card failed, not a valid card"); return; }
        Card card = Instantiate(cardPrefab, transform).GetComponentInChildren<Card>();
        AddCardToList(card);
    }
    /// <summary>
    /// Adds an already existing card object to this group
    /// </summary>
    /// <param name="cardObj">Object to add</param>
    /// <param name="reparent">defaults to true. If true, reparents the object and the visual</param>
    public void AddCardObj(GameObject cardObj, bool reparent = true)
    {
        Card card = cardObj.GetComponentInChildren<Card>();
        if (card == null) { Debug.LogWarning("Add card failed, not a valid card"); return; }
        if (reparent)
        {
            card.transform.parent.SetParent(transform);
            //card.cardVisualRoot = cardVisualGroupRoot;
        }
        AddCardToList(card);
    }
    public void AcceptCardAndSetUp(GameObject cardRoot)
    {
        //get card component
        Card card = cardRoot.GetComponentInChildren<Card>();
        //add the card to this groups list
        AddCardToList(card);
        //set card root to new group
        card.transform.parent.SetParent(transform, false);
        card.cardVisualRoot = cardVisualGroupRoot;
        // Setup the card
        card.SetUp();
        //if the card visual isn't created yet, don't create it
        if (cardVisualGroupRoot == null) return;
        //Debug.Log($"Is CardVisualRoot null? {cardVisualGroupRoot == null}");
        if (card.cardVisual == null)
        {
            Debug.LogWarning("BTW, Cardvisual is null. Just skipping for now");
        }
        //create card visual
        card.InstantiateVisual();
        //set card visual root to new visual group
        card.cardVisual.transform.SetParent(cardVisualGroupRoot);

        //wait until the end of the frame so that the 'set last siblimg' method functions as intended
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FrameWait());
            IEnumerator FrameWait()
            {
                yield return new WaitForEndOfFrame();
            }
        }
        card.cardVisual.transform.SetAsLastSibling();
    }
    /// <summary>
    /// Adds an already existing card object to this group
    /// </summary>
    /// <param name="cardRoot">card root object to add</param>
    public void AddCard(GameObject cardRoot)
    {
        //get card component
        Card card = cardRoot.GetComponentInChildren<Card>();
        //if its already in the group ignore
        if (cards.Contains(card)) return;
        //check the type of the card to make sure it matches
        if (card.GetType() != cardRootPrefab.GetComponentInChildren<Card>().GetType())
        {
            Debug.Log("Not the same card type: " + card.GetType() + "=/=" + cardRootPrefab.GetComponentInChildren<Card>().GetType());
            return;
        }
        else
        {
            Debug.Log("Same Card Type: " + card.GetType());
        }
        //get the card group it belongs to
        //its parent is the card root, the card root parent is the card group
        CardGroup group = card.transform.parent.parent.GetComponent<CardGroup>();
        //saftey check for ungrouped cards (which shouldn't exist)
        if (group == null) { Debug.LogError("On CardGroup drop, Card group not found for " + card.gameObject.name); return; }
        //remove card from old group
        group.RemoveCardFromList(card);
        //add the card to this groups list
        AddCardToList(card);
        //set card root to new group
        card.transform.parent.transform.SetParent(transform);
        //set card visual root to new visual group
        card.cardVisual.transform.SetParent(cardVisualGroupRoot);

        //wait until the end of the frame so that the 'set last siblimg' method functions as intended
        StartCoroutine(FrameWait());
        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
        }
        card.cardVisual.transform.SetAsLastSibling();
    }
    /// <summary>
    /// Adds an already existing card object to this group
    /// </summary>
    /// <param name="card">card to add</param>
    public void AddCard(Card card)
    {
        //if its already in the group ignore
        if (cards.Contains(card)) return;
        //check the type of the card to make sure it matches
        if (card.GetType() != cardRootPrefab.GetComponentInChildren<Card>().GetType())
        {
            Debug.Log("Not the same card type: " + card.GetType() + "=/=" + cardRootPrefab.GetComponentInChildren<Card>().GetType());
            return;
        }
        else
        {
            Debug.Log("Same Card Type: " + card.GetType());
        }
        //get the card group it belongs to
        //its parent is the card root, the card root parent is the card group
        CardGroup group = card.transform.parent.parent.GetComponent<CardGroup>();
        //saftey check for ungrouped cards (which shouldn't exist)
        if (group == null) { Debug.LogError("On CardGroup drop, Card group not found for " + card.gameObject.name); return; }
        //remove card from old group
        group.RemoveCardFromList(card);
        //add the card to this groups list
        AddCardToList(card);
        //set card root to new group
        card.transform.parent.transform.SetParent(transform);
        //set card visual root to new visual group
        card.cardVisual.transform.SetParent(cardVisualGroupRoot);

        card.cardVisual.transform.localScale = Vector3.one;
        card.cardVisual.transform.localRotation = Quaternion.identity;
        card.cardVisual.transform.localPosition = new Vector3(card.transform.parent.transform.localPosition.x,
                                                            card.transform.parent.transform.localPosition.y, 0);

        //set the local scale of the card
        card.transform.parent.transform.localScale = Vector3.one;
        card.transform.parent.transform.localRotation = Quaternion.identity;
        card.transform.parent.transform.localPosition = new Vector3(card.transform.parent.transform.localPosition.x,
                                                                    card.transform.parent.transform.localPosition.y, 0);
        //wait until the end of the frame so that the 'set last siblimg' method functions as intended
        StartCoroutine(FrameWait());
        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
        }
        card.cardVisual.transform.SetAsLastSibling();
    }
    /// <summary>
    /// Adds an array of existing cards to this group
    /// </summary>
    /// <param name="cards">array of cards</param>
    public void AddCards(Card[] cards)
    {
        foreach (Card card in cards)
        {
            AddCard(card);
        }
    }
    /// <summary>
    /// Adds an array of existing cards to this group
    /// </summary>
    /// <param name="cards">array of card roots</param>
    public void AddCards(GameObject[] cards)
    {
        foreach (GameObject card in cards)
        {
            AddCard(card);
        }
    }
    public void RemoveCardFromList(Card card)
    {
        cards.Remove(card);
        CardRemoved?.Invoke(card);
    }
    public void AddCardToList(Card card)
    {
        cards.Add(card);
        CardAdded?.Invoke(card);
    }
}