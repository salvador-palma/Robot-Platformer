using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class MotorPad : Enemy
{
    [Header("Other Settings")]
    public int speed;
    Rigidbody2D rb;
    Animator anim;
    public bool isCarrying;
    public float Throw_Force;
    public float Throw_Wait;
    public float Throw_Wait_Delay;
    public bool stoped;
    private void Start()
    {
        base.Init(5);
        anim = GetComponent<Animator>();
        anim.SetBool("Carry", isCarrying);
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (stoped) { return; }
        rb.velocity = new Vector2(transform.localScale.x * -speed * Time.deltaTime, rb.velocity.y);
    }

    public override void TriggerHandler(GameObject target, string child_name, int state)
    {
        switch (child_name)
        {
            case "GroundCheck":
                if (state == 1)
                {
                    if (isCarrying)
                    {
                        StartCoroutine(Throw());
                    }
                    else { Flip(); }        
                    

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

    public IEnumerator Throw()
    {

        isCarrying = false;
        stoped = true;
        
        yield return new WaitForSeconds(Throw_Wait);
        GetComponent<Animator>().Play("Throw");
        anim.SetBool("Carry", isCarrying);
        GameObject plate = gameObject.transform.GetChild(3).gameObject;
        plate.transform.parent = null;
        plate.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        plate.GetComponent<Plates>().hasFather = false;
        int dir = transform.localScale.x < 0 ? 1 : -1;
        plate.GetComponent<Rigidbody2D>().velocity = new Vector2(dir * Throw_Force, 1f * Throw_Force);
        yield return new WaitForSeconds(Throw_Wait_Delay);
        Flip();
        stoped = false;
    }

    public void ThrowPlate()
    {
        
    }



}
