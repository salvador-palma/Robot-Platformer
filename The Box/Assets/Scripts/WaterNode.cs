using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterNode : MonoBehaviour
{


    [HideInInspector] public float velocity;
    [HideInInspector] public float height;
    [HideInInspector] public float force;
    [HideInInspector] public float stiff;
    [HideInInspector] public float decay;

    public static float TimeDelay = 0.5f;
    public static float massForce = 10f;
    float TimeDelayStart;
    bool inForce;
    public void Start()
    {
        stiff = Water.default_stiffness;
        decay = Water.default_decay;
    }
    public void Update()
    {
        if (inForce)
        {
            if (TimeDelayStart > 0)
            {
                TimeDelayStart -= Time.deltaTime;
            }
            else
            {
                inForce = false;
                
            }
        }
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inForce)
        {
            inForce = true;
            TimeDelayStart = TimeDelay;
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Rigidbody2D>().velocity.y >= 0)
                {
                    SetForce(massForce/2);
                }
                else
                {
                    SetForce(-massForce);
                }
            }
            
           
        }
    }




}
