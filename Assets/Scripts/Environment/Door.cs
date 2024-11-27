using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private HingeJoint2D hinge;
    private Rigidbody2D doorLeafRigidbody;

    private JointAngleLimits2D openLimits;
    private JointAngleLimits2D closeLimits;

    [SerializeField]
    private float closingForce = 1f; // Adjustable force to close the door
    
    [SerializeField]
    private float DOOR_CLOSE_ANGLE = 1f; // Threshold angle to consider door closed

    private void Awake()
    {
        hinge = transform.Find("Hinge").GetComponent<HingeJoint2D>();
        doorLeafRigidbody = transform.Find("Door Leaf").GetComponent<Rigidbody2D>();

        openLimits = hinge.limits;
        closeLimits = new JointAngleLimits2D { min = 0, max = 0 };
    }

    void Update()
    {
        ApplyClosingForce();
    }

    private void ApplyClosingForce()
    {
        float angle = hinge.jointAngle;
        
        // If door is open (angle is not near 0)
        if (Mathf.Abs(angle) > DOOR_CLOSE_ANGLE)
        {
            // Apply torque in the direction that would close the door
            float closingDirection = -Mathf.Sign(angle);
            doorLeafRigidbody.AddTorque(closingDirection * closingForce);
        }
        else
        {
            // Door is nearly closed, stop any rotation
            doorLeafRigidbody.angularVelocity = 0;
        }
    }

    public void Open()
    {
        hinge.limits = openLimits;
    }

    public void Close()
    {
        hinge.limits = closeLimits;
    }
}