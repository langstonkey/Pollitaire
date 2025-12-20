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
}
