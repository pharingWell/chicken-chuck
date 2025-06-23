using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 offset = new Vector3(0f, 5f, -5f); // Adjust as needed

    private float rotationSmoothSpeed = 3f;

    void FixedUpdate()
    {
        transform.position = player.position + player.rotation * offset;

        Quaternion targetRotation = Quaternion.Euler(30f, player.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
