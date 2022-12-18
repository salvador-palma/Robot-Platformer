using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCollider : MonoBehaviour
{
    Movement player;
    AxisRotation axis;
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Movement>();
        axis = GameObject.Find("Axis").GetComponent<AxisRotation>();
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.stop_player)
        {
            Vector3 v = player.getPosition();
            int dir = v.x > 0 ? 1 : -1;
            axis.RotateAxis(dir);
        }
        
        
    }
}
