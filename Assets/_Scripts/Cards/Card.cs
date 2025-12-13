using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Canvas canvas; //canvas reference used for scaling the mouse delta
    private RectTransform rect; //rect transform to set position and anchors
    private CanvasGroup canvasGroup; //canvas group to set alpha and toggle raycast while dragging

    [Header("Visuals")]
    [Tooltip("Wether or not to instantiate a seperate visual")]
    [SerializeField] protected bool instantiateVisual = true;
    [Tooltip("How much to move the card up by when being selected")]
    [SerializeField] protected float selectionOffset = 40;
    [Tooltip("Prefab to instantiate")]
    [SerializeField] protected GameObject cardVisualPrefab;
    [Tooltip("Card visual reference associated with prefab")]
    /*[HideInInspector]*/
    public CardVisual cardVisual;

    //the parent of card visual associated with this card
    /*[HideInInspector]*/
    public Transform cardVisualRoot;
    public bool isDragging { get; private set; }
    [field: SerializeField] public bool isHovering { get; private set; }
    public bool isSelected { get; private set; }

    [field: Header("Runtime Variables")]
    [field: SerializeField] public bool draggable { get; protected set; }
    [field: SerializeField] public bool selectable { get; protected set; }

    [Header("Hover Parameters")]
    [SerializeField] bool offsetOnHover = false;
    [SerializeField] float hoverOffset = 20f;

    public virtual void SetUp()
    {
        //get components
        canvas = transform.GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }


    public virtual void OverrideAndFill(object obj)
    {
        Debug.Log("Override and fill not implemented for " + obj.GetType());
    }

    public virtual void RefreshVisual() { cardVisual.FillCard(this); }

    /// <summary>
    /// This method is one that is made to be called by subclasses of card
    /// </summary>
    /// <param name="eventData">event data that contains whatever object may be dragged onto the card</param>
    public virtual void OnCardReceived(PointerEventData eventData)
    {
        CardGroup cardGroup = transform.parent.parent.GetComponent<CardGroup>();

        //call the group associated with this card
        if (cardGroup == null)
        {
            Debug.LogError("On Card drop, Card group not found for " + gameObject.name);
            return;
        }
        else
        {
            cardGroup.OnDrop(eventData);
        }
    }

    //runs when another card is dropped onto this one
    public void OnDrop(PointerEventData eventData)
    {
        OnCardReceived(eventData);
    }

    public void InstantiateVisual()
    {
        //Debug.Log("Instantiating visual", this);
        if (cardVisualRoot == null)
        {
            Debug.LogError($"Current CardVisualRoot: {cardVisualRoot}", this);
            return;
        }

        cardVisual = Instantiate(cardVisualPrefab, cardVisualRoot).GetComponent<CardVisual>();
        cardVisual.Init(this);
    }

    #region Drag & Drop Logic
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        //set bool
        isDragging = true;

        //invoke event
        CardManager.Instance.BeginDragEvent?.Invoke(this);

        //set canvas group ignore raycast 
        canvasGroup.blocksRaycasts = false;

        //set the visual group on top
        cardVisualRoot.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        isDragging = true;

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        //set bool
        isDragging = false;

        //wait until the end of the frame (instead of calling cursed update method)
        StartCoroutine(FrameWait());
        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
        }

        //invoke event
        CardManager.Instance.EndDragEvent?.Invoke(this);

        //reset canvas group raycast 
        canvasGroup.blocksRaycasts = true;

        //reset local transform to origin
        transform.localPosition = Vector3.zero;
    }

    public virtual bool TryChangeCardGroup(CardGroup newGroup)
    {
        return true;
    }

    public virtual void OnDestroy()
    {
        if (cardVisual != null)
        {
            Destroy(cardVisual.gameObject);
        }
    }

    #endregion

    #region Pointer Logic
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (transform.parent.parent.GetComponent<CardGroup>().isHorizontal)
            transform.localPosition = (Vector3.up * hoverOffset);

        //set the visual group on top, if it isn't already holding a card
        if (!isDragging && !CardManager.Instance.selectedCard) cardVisualRoot.SetAsLastSibling();

        //invoke event
        CardManager.Instance.PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (transform.parent.parent.GetComponent<CardGroup>().isHorizontal)
            transform.localPosition = Vector3.zero;

        //invoke event
        CardManager.Instance.PointerExitEvent?.Invoke(this);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //ensure its left click
        if (eventData.button != PointerEventData.InputButton.Left) return;

        //invoke event
        CardManager.Instance.PointerUpEvent?.Invoke(this);

        if (!selectable) return;

        isSelected = !isSelected;

        //invoke event
        CardManager.Instance.SelectEvent?.Invoke(this, isSelected);

        //selection logic
        if (isSelected)
        {
            transform.localPosition += (cardVisual.transform.up * selectionOffset);
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //ensure its left click
        if (eventData.button != PointerEventData.InputButton.Left) return;
    }
    #endregion

    #region Getting Index Methods
    public int ParentIndex()
    {
        return transform.parent.GetSiblingIndex();
    }
    public int SiblingAmount()
    {
        return transform.parent.parent.childCount;
    }

    public float NormalizedPosition()
    {
        return Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1);
    }

    #endregion

    #region Setters For Runtime Variables
    public void SetDraggable(bool isDraggable)
    {
        draggable = isDraggable;
    }
    public void SetSelectable(bool isSelectable)
    {
        draggable = isSelectable;
    }
    #endregion

    #region Util
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #endregion
}

