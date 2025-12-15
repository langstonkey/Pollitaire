using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFlowerType")]
public class FlowerType : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public Color Color;
}
