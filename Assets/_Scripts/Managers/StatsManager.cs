using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

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
        
        DontDestroyOnLoad(gameObject);
    }


}
