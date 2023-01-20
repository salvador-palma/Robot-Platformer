using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int Health;
    public GameObject hurtParticle;
    public GameObject deathParticle;
    protected virtual void Init(int Health)
    {
        hurtParticle = Resources.Load<GameObject>("Hitted");
        deathParticle = Resources.Load<GameObject>("Smoke Particle");
        this.Health = Health;
    }
    public void Die()
    {
        GameObject go = Instantiate(deathParticle);
        go.transform.position = transform.position;
        Destroy(this.gameObject);
    }
    public void takeDmg(int amount)
    {
        Hurt();
        Health -= amount;
        if (Health <= 0) { Die(); }
    }
    public void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public void setSprite(Sprite spr)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = spr;
    }

    public void Hurt()
    {
        GameObject go = Instantiate(hurtParticle);
        go.transform.position = transform.position;
        GetComponent<Animator>().Play("Hurt");
    }

    public abstract void TriggerHandler(GameObject target_tag, string child_name, int state);
    
   






}
