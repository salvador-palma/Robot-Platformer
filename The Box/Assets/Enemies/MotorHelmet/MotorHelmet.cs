using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class MotorHelmet : Enemy
{
    [Header("Other Settings")]
    public int speed;
    Rigidbody2D rb;
    
    private void Start()
    {
        base.Init(2);
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(transform.localScale.x * -speed * Time.deltaTime, rb.velocity.y);
    }

    public override void TriggerHandler(GameObject target, string child_name, int state)
    {
        switch (child_name)
        {
            case "GroundCheck":
                if (target.CompareTag("Ground") && state == 1)
                {
                    Flip();
                }
                break;
            case "WallCheck":
                if (state == 0)
                {
                    Flip();
                }
                break;
            case "BodyCheck":
                if(target.CompareTag("Player"))
                {
                    Movement m = Movement.getInstance();
                    if (state == 0 && !m.isDashing && !m.isInvencible)
                    {
                        int d = target.transform.position.x < transform.position.x ? -1 : 1;
                        m.takeDmg(1,d);
                    }
                }
                break;
        }
    }



}
