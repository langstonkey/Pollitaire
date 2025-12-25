using NaughtyAttributes;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static int Wins = 0;
    public static int Depth = 2;
    public static int Types = 6;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("Wins")) Wins = PlayerPrefs.GetInt("Wins");
        if (PlayerPrefs.HasKey("Depth")) Depth = PlayerPrefs.GetInt("Depth");
        if (PlayerPrefs.HasKey("Types")) Types = PlayerPrefs.GetInt("Types");

        if (Types == 0) SetTypes(6);
        if (Depth == 0) SetTypes(2);
    }

    public static void AddWin()
    {
        Wins++;
        PlayerPrefs.SetInt("Wins", Wins);
    }

    public static void SetDepth(int depth)
    {
        Depth = depth;
        PlayerPrefs.SetInt("Depth", depth);
    }

    public static void AddDepth()
    {
        Depth++;
        SetDepth(Depth);
    }

    public static void SubDepth()
    {
        if (Depth > 1)
        {
            Depth--;
            SetDepth(Depth);
        }
    }

    public static void SetTypes(int types)
    {
        Types = types;
        PlayerPrefs.SetInt("Types", types);
    }

    public static void AddTypes()
    {
        if (Types < 6)
        {
            Types++;
            SetTypes(Types);
        }
    }

    public static void SubTypes()
    {
        if (Types > 2)
        {
            Types--;
            SetTypes(Types);
        }
    }


    [Button]
    public void ResetStats()
    {
        PlayerPrefs.DeleteAll();
    }
}
