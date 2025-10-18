using UnityEngine;

public class GoalController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 0.5f;

    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) < detectionRadius)
        {
            Debug.Log("🎯 Player reached goal!");
            GameManager.Instance?.OnGoalReached();
            Destroy(gameObject);
        }
    }
}
