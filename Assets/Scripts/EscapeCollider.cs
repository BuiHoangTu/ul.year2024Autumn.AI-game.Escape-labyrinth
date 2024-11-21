using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCollider : MonoBehaviour
{
    // check if collide with player
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collide with " + other.tag);

        if (other.CompareTag("Escaper"))
        {
            // Escaper wins
            GameManager.Instance.EscaperWin();
        }
    }
}
