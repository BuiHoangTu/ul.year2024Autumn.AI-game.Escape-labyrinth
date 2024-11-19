using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;


    // Update last
    void LateUpdate()
    {
        // Move the camera to the player's position
        transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
    }
}
