using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EscaperWin()
    {
        Debug.Log("Escaper has won the game!");

        // Notify EscaperAgent
        if (EscaperAgent.INSTANCE != null)
        {
            EscaperAgent.INSTANCE.Win();
        }
        else
        {
            Debug.LogWarning("EscaperAgent instance is not set!");
        }
    }
}
