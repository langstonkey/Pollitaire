using UnityEngine;

public class FlowerDeathEffect : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("Settings")]
    public float gravity = -981f; // Standard pixels/sec gravity
    public Vector2 angleRange = new Vector2(45, 135); // Upward arc
    public Vector2 speedRange = new Vector2(300, 600);
    public float rotationSpeed = 180f;

    private Vector3 velocity;
    private float angularVelocity;
    private bool isSimulating = false;

    [SerializeField] GameObject pollen;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Launch();
    }

    public void Start()
    {
        Instantiate(pollen, transform.parent).transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10);
    }

    public void Launch()
    {
        //Choose a random direction (Angle to Vector)
        float randomAngle = Random.Range(angleRange.x, angleRange.y) * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0);

        //Choose random velocity
        float speed = Random.Range(speedRange.x, speedRange.y);
        velocity = direction * speed;

        //Set random angular velocity (rotation)
        angularVelocity = Random.Range(-rotationSpeed, rotationSpeed);

        isSimulating = true;
    }

    void Update()
    {
        if (!isSimulating) return;
        if (rectTransform == null) return;

        // Apply Gravity to Velocity
        velocity.y += gravity * Time.deltaTime;

        // Apply Velocity to Position
        rectTransform.anchoredPosition += (Vector2)velocity * Time.deltaTime;

        // Apply Rotation
        rectTransform.Rotate(0, 0, angularVelocity * Time.deltaTime);

        // Optional: Stop simulating if it falls off screen
        if (rectTransform.anchoredPosition.y < -1500)
        {
            isSimulating = false;
        }
    }
}
