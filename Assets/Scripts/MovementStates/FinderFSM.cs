using UnityEngine;

public class FinderFSM
{
    public FinderState currentState { get; private set; }
    public int currentExitIndex { get; private set; }


    public FinderFSM()
    {
        this.currentState = FinderState.IDLE;
    }

    public bool Idle()
    {

        this.currentState = FinderState.IDLE;
        return true;
    }

    public bool GuardingExit()
    {
        this.currentState = FinderState.GUARDING_EXIT;
        return true;
    }

    public bool ChangeGuardingExit(int exitIndex)
    {
        this.currentExitIndex = exitIndex;
        this.currentState = FinderState.CHANGE_GUARDING_EXIT;
        return true;
    }

    public bool ChasingEscaper()
    {
        this.currentState = FinderState.CHASING_ESCAPER;
        return true;
    }



    ///// Finder states /////
    public enum FinderState
    {
        IDLE,
        GUARDING_EXIT,
        CHANGE_GUARDING_EXIT,
        CHASING_ESCAPER,
    }
}