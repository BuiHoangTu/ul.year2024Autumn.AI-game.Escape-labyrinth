using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    // up-down left-right movement
    public float speed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        // Get the input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move the player
        transform.Translate(new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime);
    }
}
