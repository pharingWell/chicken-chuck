using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    private float maxSpeed = 30f;
    private float maxReverseSpeed = -5f;
    private float acceleration = 8f;
    private float turnSpeed = 0;
    private float minAngularAcceleration = 0.08f;
    private float maxAngularAcceleration = 0.15f;

    private Vector3 direction = new Vector3(0, 0, 1);

    public Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputHandling();
        turnSpeed = maxAngularAcceleration + (minAngularAcceleration - maxAngularAcceleration) * Mathf.Pow((rb.velocity.magnitude / maxSpeed), 2);
        speedText.text = Mathf.Round(rb.velocity.magnitude * 2.237f * 10)/10f + " MPH";
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * rb.velocity.magnitude;
    }

    private void InputHandling()
    {
        if (Input.GetKey(KeyCode.S))
        {
            ActiveBrake();
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Accelerate();
        }
        else
        {
            FrictionBrake();
        }

        if (Input.GetKey(KeyCode.A))
        {
            Turn(false);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Turn(true);
        }
    }

    public void ActiveBrake()
    {
        rb.velocity *= 0.9985f;

        // Optional: stop completely when very slow
        if (rb.velocity.magnitude < 0.2f)
        {
            Reverse();
        }
    }

    public void FrictionBrake()
    {
        // Apply friction
        rb.velocity *= 0.99975f;

        // Optional: stop completely when very slow
        if (rb.velocity.magnitude < 0.2f && rb.velocity.magnitude > -0.2f)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void Reverse()
    {
        if (rb.velocity.magnitude > maxReverseSpeed)
        {
            rb.velocity -= acceleration * direction * Time.deltaTime;
        }
    }

    public void Turn(bool right)
    {
        float rotationDirection = right ? 1f : -1f;

        // Only turn if moving
        if (Mathf.Abs(rb.velocity.magnitude) > 0.01f)
        {
            float angle = turnSpeed * rotationDirection;

            // Rotate direction vector around Y-axis
            Quaternion turnRotation = Quaternion.Euler(0f, angle, 0f);
            direction = turnRotation * direction;
            direction.Normalize();

            // Rotate the GameObject to face the new direction
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Accelerate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity += acceleration * direction * Time.deltaTime;
        }
    }
}
