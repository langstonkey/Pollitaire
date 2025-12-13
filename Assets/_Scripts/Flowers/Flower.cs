using UnityEngine;
using UnityEngine.UI;
public enum FlowerType
{
    Tulip,
    Rose,
    Daisy,
    Peony
}

public class Flower : MonoBehaviour
{
    [field: SerializeField] public FlowerType Type { get ; private set; }

    //TEMP
    Image image;

    private void Start()
    {
        Type = (FlowerType)Random.Range(0, 4);
        image = GetComponent<Image>();
        SetColor();
    }

    public void SetFlowerType(FlowerType type)
    {
        Type = type;
    }

    public void SetColor()
    {
        switch (Type)
        {
            case FlowerType.Tulip:
            image.color = Color.yellow;
            break;

            case FlowerType.Rose:
            image.color = Color.red;
            break;

            case FlowerType.Daisy:
            image.color = Color.white;
            break;

            case FlowerType.Peony:
            image.color = Color.pink;
            break;
        }
    }
}
