using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<EscaperAgent> escapers;
    private List<FinderAgent> finders;


    public void Start()
    {
        this.escapers = new List<EscaperAgent>();
        this.finders = new List<FinderAgent>();

        // Find all Escapers and Finders that are chidren of this
        this.finders.AddRange(GetComponentsInChildren<FinderAgent>());
        this.escapers.AddRange(GetComponentsInChildren<EscaperAgent>());
    }


    public void EscaperWin()
    {
        Debug.Log("Escaper has won the game!");

        // Notify EscaperAgent
        foreach (var escaper in escapers)
        {
            escaper.Win();
        }

        // Notify FinderAgent
        foreach (var finder in finders)
        {
            finder.Lose();
        }
        
    }
}
