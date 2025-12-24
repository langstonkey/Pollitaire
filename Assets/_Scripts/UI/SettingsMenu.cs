using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] TextMeshProUGUI flowerTypesText;

    public void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        depthText.text = $"Depth: {StatsManager.Depth}";
        flowerTypesText.text = $"Types: {StatsManager.Types}";
    }
}
