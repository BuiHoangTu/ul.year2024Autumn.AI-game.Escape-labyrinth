using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius = 5f;                      // Detection radius
    [Range(1, 360)] public float angle = 45f;      // Field of view angle
    public LayerMask targetLayer;                  // Layer for targets like Seeker
    public LayerMask obstructionLayer;             // Layer for obstructions

    private GameObject playerRef;                  // Reference to the Seeker
    public bool CanSeePlayer { get; private set; } // Boolean to track if Seeker is in view

    void Start()
    {
        // Find the Seeker object by tag
        playerRef = GameObject.FindGameObjectWithTag("Player");

        // Start the Field of View check coroutine
        StartCoroutine(FOVCheck());
    }

    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        // Continuously check if the Seeker is in view
        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    {
        // Overlap circle to find targets within radius
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

        // If any target is in range
        if (rangeCheck.Length > 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the specified angle
            if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                // Raycast to detect any obstruction between Finder and Seeker
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                {
                    CanSeePlayer = true;
                }
                else
                {
                    CanSeePlayer = false;
                }
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else if (CanSeePlayer)
        {
            CanSeePlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a circle to visualize the seeker's field of view
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

        Vector3 angle01 = DirectionFromAngle(transform.eulerAngles.z, -angle / 2);
        Vector3 angle02 = DirectionFromAngle(-transform.eulerAngles.z, angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if (CanSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }

    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees -= eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
