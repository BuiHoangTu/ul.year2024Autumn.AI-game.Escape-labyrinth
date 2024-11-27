using System.Collections.Generic;
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
        var searchFinder = GetComponentsInChildren<FinderAgent>();
        Debug.Log("Found " + searchFinder.Length + " FinderAgents");

        var searchEscaper = GetComponentsInChildren<EscaperAgent>();
        Debug.Log("Found " + searchEscaper.Length + " EscaperAgents");

        // Add them to the list
        this.finders.AddRange(searchFinder);
        this.escapers.AddRange(searchEscaper);
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

    public void FinderWin()
    {
        Debug.Log("Finder has won the game!");

        // Notify EscaperAgent
        foreach (var escaper in escapers)
        {
            escaper.Lose();
        }

        // Notify FinderAgent
        foreach (var finder in finders)
        {
            finder.Win();
        }
    }
}
