using UnityEngine;

public class Particle : MonoBehaviour
{
    public float real;
    public float imaginary;

    public float moveSpeed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 direction = Vector2.zero;

        if (real > 0) direction.x += 1f;
        else if (real < 0) direction.x -= 1f;

        if (imaginary > 0) direction.y += 1f;
        else if (imaginary < 0) direction.y -= 1f;

        rb.linearVelocity = direction * moveSpeed;
    }
}