using UnityEngine;

public class CaughtEscaper : MonoBehaviour
{
    private GameManager gameManager;


    private void Awake()
    {
        this.gameManager = this.GetComponentInParent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Escaper"))
        {
            gameManager.FinderWin();
        }
    }
}
