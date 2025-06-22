using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 offset = new Vector3(0f, 20f, -3f); // Adjust as needed

    private float rotationSmoothSpeed = 2f;

    void FixedUpdate()
    {
        transform.position = player.position + player.rotation * offset;

        Quaternion targetRotation = Quaternion.Euler(60f, player.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
