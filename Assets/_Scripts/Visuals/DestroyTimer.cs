using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] float lifetime;
    float time;

    public void Start()
    {
        time = lifetime;
    }

    public void Update()
    {
        if (time <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            time -= Time.deltaTime;
        }
    }
}
