using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;       // assign your Player
    public float smoothSpeed = 5f; // higher = faster follow
    public Vector3 offset = new Vector3(0, 0, -10); // standard for 2D camera

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

}
