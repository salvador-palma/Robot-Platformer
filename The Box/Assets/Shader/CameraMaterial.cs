using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaterial : MonoBehaviour
{
    Renderer r;
    
    float TimerStart;
    float TimerFirstThird;
    float TimerSecondThird;
    bool Emiting;
    public float increments =  0.03f;
    public Material m;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!Emiting) {return;}
      
        if(TimerStart > TimerFirstThird)
        {
            TimerStart -= Time.deltaTime;
            SetCount(m.GetFloat("_RippleCount") + increments) ;
        }
        else if(TimerStart > TimerSecondThird)
        {
            TimerStart -= Time.deltaTime;
           
        }
        else if(TimerStart > 0)
        {
            TimerStart -= Time.deltaTime;
            SetCount(m.GetFloat("_RippleCount") - increments);
           
        }
        else
        {
            SetCount(0);
            Emiting = false;
           
        }
       
    }

    public void Emit(Vector2 pos, float f, float inc)
    {
        increments = inc;
        SetPosition(pos);
        TimerStart = f;
        TimerFirstThird = (f / 3) * 2;
        TimerSecondThird = f / 3;
        Emiting = true;
    }
    public void SetPosition(Vector2 pos)
    {
        m.SetVector("_RippleCenter", pos);
    }
    public void SetCount(float f)
    {
        m.SetFloat("_RippleCount", f);
    }
}
