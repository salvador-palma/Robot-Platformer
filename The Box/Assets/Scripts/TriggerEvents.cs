using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    Enemy e;
    private void Start()
    {
        e = transform.parent.GetComponent<Enemy>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        e.TriggerHandler(collision.gameObject, gameObject.name, 0);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        e.TriggerHandler(collision.gameObject, gameObject.name, 1);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        e.TriggerHandler(collision.gameObject, gameObject.name, 2);
    }
}
