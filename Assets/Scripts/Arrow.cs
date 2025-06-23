using UnityEngine;
using UnityEngine.UI;

public class DirectionArrow : MonoBehaviour
{
    public Transform player;
    public Transform target;

    private RectTransform arrowRect;

    void Start()
    {
        arrowRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 toTarget = target.position - player.position;
        Vector3 playerForward = player.forward;

        // Flatten both vectors to XZ plane
        toTarget.y = 0f;
        playerForward.y = 0f;

        // Get signed angle from player's forward to the target direction
        float angle = Vector3.SignedAngle(playerForward.normalized, toTarget.normalized, Vector3.up);

        // Rotate the arrow — no flip needed since 0° is right
        arrowRect.localRotation = Quaternion.Euler(0f, 0f, -angle + 90f);
    }
}
