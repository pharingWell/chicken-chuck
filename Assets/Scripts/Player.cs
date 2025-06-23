using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    private float maxSpeed = 30f;
    private float maxReverseSpeed = -5f;
    private float acceleration = 10f;
    private float turnSpeed = 0;
    private float turnSpeedAtMaxSpeed = 0.1f;
    private float turnSpeedAtLowSpeed = 0.15f;
    private float brakeForce = 20f;
    private float frictionForce = 5f;

    private Vector3 direction = new Vector3(0, 0, 1);

    public Text speedText;

    public bool hasQuest = true;
    public Transform destination;
    public Transform arrow;
    public Text distanceText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toTarget = destination.position - transform.position;

        // Flatten on Y so arrow doesn't tilt up/down
        Vector3 flatDirection = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

        // Point the arrow in the direction
        if (arrow != null)
            arrow.rotation = Quaternion.LookRotation(flatDirection);

        // Update distance text
        if (distanceText != null)
        {
            float distance = toTarget.magnitude;
            distanceText.text = Mathf.Round(distance).ToString() + "m";
        }

        InputHandling();
        turnSpeed = turnSpeedAtLowSpeed + (turnSpeedAtMaxSpeed - turnSpeedAtLowSpeed) * Mathf.Pow((rb.velocity.magnitude / maxSpeed), 2);
        speedText.text = Mathf.Round(rb.velocity.magnitude * 2.237f * 10)/10f + " MPH";

        if (rb.velocity.magnitude > 0.1f &&
            !Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.D))
        {
            Vector3 velocityDir = rb.velocity.normalized;

            // Only update direction if velocity is mostly forward (dot > 0)
            float alignment = Vector3.Dot(direction, velocityDir);

            if (alignment > 0f) // between 0 (90°) and 1 (fully forward)
            {
                direction = Vector3.Slerp(direction, velocityDir, 2f * Time.deltaTime);
                direction.Normalize();
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }


    }

    private void InputHandling()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ActiveBrake();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Reverse();
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

    // applies a strong friction force when actively braking
    public void ActiveBrake()
    {
        float appliedForce = brakeForce;

        if (rb.velocity.magnitude > 0.2f)
        {
            Vector3 deceleration = rb.velocity.normalized * appliedForce * Time.deltaTime;

            // Check if applying deceleration would flip direction
            if (Vector3.Dot(rb.velocity, rb.velocity - deceleration) < 0)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity -= deceleration;
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    // applies constant friction when not accelerating
    public void FrictionBrake()
    {
        float appliedForce = frictionForce;

        if (rb.velocity.magnitude > 0.2f)
        {
            Vector3 deceleration = rb.velocity.normalized * appliedForce * Time.deltaTime;

            // Check if applying deceleration would flip velocity direction
            if (Vector3.Dot(rb.velocity, rb.velocity - deceleration) < 0)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity -= deceleration;
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }


    public void Reverse()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, direction);

        if (forwardSpeed > maxReverseSpeed)
        {
            rb.velocity += acceleration * -direction * Time.deltaTime;
        }
    }

    public void Turn(bool right)
    {
        // Calculate signed speed along the forward direction
        float forwardSpeed = Vector3.Dot(rb.velocity, direction);

        // Flip turning direction if reversing
        float rotationDirection = (right ? 1f : -1f) * (forwardSpeed < 0f ? -1f : 1f);

        // Only turn if moving
        //if (rb.velocity.magnitude > 0.01f)
        {
            float angle = turnSpeed * rotationDirection;

            // Rotate direction vector
            Quaternion turnRotation = Quaternion.Euler(0f, angle, 0f);
            direction = turnRotation * direction;
            direction.Normalize();

            // Rotate the GameObject to face the new direction
            transform.rotation = Quaternion.LookRotation(direction);

            // Preserve signed speed while reorienting velocity
            float signedSpeed = Mathf.Sign(forwardSpeed) * rb.velocity.magnitude;
            rb.velocity = Vector3.Lerp(rb.velocity, direction * signedSpeed, 0.2f);
        }
    }

    public void Accelerate()
    {
        float currentSpeed = Vector3.Dot(rb.velocity, direction);

        if (currentSpeed < maxSpeed)
        {
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            float boost = 1f - speedRatio; // More boost at lower speeds
            float scaledAccel = acceleration * (0.35f + boost * 0.65f); // Range: 0.5x to 1x

            rb.velocity += direction * scaledAccel * Time.deltaTime;
        }
    }

}
