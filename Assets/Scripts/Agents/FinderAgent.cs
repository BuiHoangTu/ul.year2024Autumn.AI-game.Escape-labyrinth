using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;

public class FinderAgent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }

    public void Lose()
    {
        return;  // TODO: Implement
    }

    internal void Win()
    {
        throw new NotImplementedException();
    }
}
