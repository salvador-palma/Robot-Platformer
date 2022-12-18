using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plates : MonoBehaviour
{
    public bool hasFather;

    private void Start()
    {
        if (transform.parent == null)
        {
            hasFather = false;
        }
        else
        {
            hasFather = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
       
        if(collision.CompareTag("Player"))
        {
            if (hasFather)
            {
                Movement.getInstance().ground_speed_x = transform.parent.GetComponent<Rigidbody2D>().velocity.x;
            }
            else
            {
                Movement.getInstance().ground_speed_x = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            }
           
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Movement.getInstance().ground_speed_x = 0;

        }
    }
}
