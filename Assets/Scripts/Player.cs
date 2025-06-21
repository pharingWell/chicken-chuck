using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    private float currentSpeed = 0f;
    private float maxSpeed = 10f;
    private float acceleration = 1f;
    private float turnSpeed = 1f;

    public Vector3 direction = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * currentSpeed;
    }

    public void OnAccelerate()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration;
        }
    }
}
