using NaughtyAttributes;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static int Wins = 0;
    private void Start()
    {
        Wins = PlayerPrefs.GetInt("Wins");
    }

    public static void AddWin()
    {
        Wins++;
        PlayerPrefs.SetInt("Wins", Wins);
    }

    [Button]
    public void ResetStats()
    {
        PlayerPrefs.DeleteAll();
    }
}
