using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHinge : MonoBehaviour
{

    private HingeJoint2D hinge;
    private JointAngleLimits2D openLimits;
    private JointAngleLimits2D closeLimits;


    private void Awake()
    {
        hinge = transform.Find("Hinge").GetComponent<HingeJoint2D>();
        openLimits = hinge.limits;
        closeLimits = new JointAngleLimits2D { min = 0, max = 0 };        

        Close();
    }

    public void Open()
    {
        hinge.limits = openLimits;
    }

    public void Close()
    {
        hinge.limits = closeLimits;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
