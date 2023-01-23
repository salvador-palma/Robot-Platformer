using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterNode
{
    

    public float velocity;
    public float height;
    public float force;

    public float stiff;
    public float decay;
    public WaterNode(float stiff, float decay)
    {
        this.stiff = stiff;
        this.decay = decay;
    }
    public void UpdateHeight()
    {
        force = -stiff * height;
        velocity += force;
        velocity *= decay;
        height += velocity;
    }
    public void SetForce(float f)
    {
        velocity = f;
    }

   

    
}
