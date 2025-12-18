using UnityEngine;
using UnityEngine.UI;

public class FlowerCard : MonoBehaviour
{
    [field: SerializeField] public FlowerType Type { get ; private set; }
    [SerializeField] FlowerCardVisual flowerVisual;

    private void Start()
    {
        SetFlowerType(Type);
    }

    public void SetFlowerType(FlowerType type)
    {
        Type = type;
        flowerVisual.SetFlowerVisual(this, transform);
    }
}
