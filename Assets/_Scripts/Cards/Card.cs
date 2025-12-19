using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Visuals")]
    [Tooltip("Wether or not to instantiate a seperate visual")]
    [SerializeField] protected bool instantiateVisual = true;
    [Tooltip("Card visual reference associated with prefab")]
    [SerializeField] public Transform cardVisual;

    [Header("References")]
    [SerializeField] private RectTransform root;

    [Header("Settings")]
    [field: SerializeField] bool draggable;
    [field: SerializeField] bool selectable;

    [field: Header("Runtime Variables")]
    [field: SerializeField, ReadOnly] public bool IsDragging { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsHovering { get; private set; }
    [field: SerializeField, ReadOnly] public bool IsSelected { get; private set; }

    private Canvas canvas; //canvas reference used for scaling the mouse delta
    private RectTransform rect; //rect transform to set position and anchors
    private CanvasGroup canvasGroup; //canvas group to set alpha and toggle raycast while dragging
    private CardGroup cardGroup; //card group that this is a part of

    public void Start()
    {
        cardGroup = GetComponentInParent<CardGroup>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        if (cardVisual) cardVisual.SetParent(CardManager.Instance.CardVisualRoot);
    }

    #region Drag & Drop Logic
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        //set bool
        IsDragging = true;

        //invoke event
        CardManager.Instance.BeginDragEvent?.Invoke(this);

        //set canvas group ignore raycast 
        canvasGroup.blocksRaycasts = false;

        //set the visual group on top
        //cardVisualRoot.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        IsDragging = true;

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        //set bool
        IsDragging = false;

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

    #endregion

    #region Pointer Logic
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovering = true;

        //invoke event
        CardManager.Instance.PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;

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

        IsSelected = !IsSelected;

        //invoke event
        CardManager.Instance.SelectEvent?.Invoke(this, IsSelected);

    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //ensure its left click
        if (eventData.button != PointerEventData.InputButton.Left) return;

        cardVisual.SetAsLastSibling();
    }

    #endregion

    #region Group Logic

    public void SetGroup(CardGroup group)
    {
        cardGroup = group;
        root.SetParent(group.CardRoot, false);
    }
    public CardGroup GetGroup() { return cardGroup;}

    #endregion

    private void OnDestroy()
    {
        //also destory my root
        Destroy(root.gameObject);
        Destroy(cardVisual.gameObject);
    }
}

