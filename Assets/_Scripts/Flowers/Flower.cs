using UnityEngine;
using UnityEngine.UI;

public class Flower : MonoBehaviour
{
    [field: SerializeField] public FlowerType Type { get ; private set; }

    //TEMP
    Image image;

    private void Start()
    {
        Type = FlowerGroupManager.Instance.GetRandomType();
        image = GetComponent<Image>();
        SetColor();
    }

    public void SetFlowerType(FlowerType type)
    {
        Type = type;
    }

    public void SetColor()
    {
        image.color = Type.Color;
    }
}
