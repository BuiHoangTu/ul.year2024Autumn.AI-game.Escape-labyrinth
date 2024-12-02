using UnityEngine;

public class EscaperFSM
{
    public EscaperState currentState { get; private set; }


    public EscaperFSM()
    {
        this.currentState = EscaperState.IDLE;
    }

    public bool UpdateState(EscaperState state)
    {
        this.currentState = state;
        return true;
    }



    ///// Escaper states /////
    public enum EscaperState
    {
        IDLE,
        TO_EXIT,
        FLEEING,
    }
}