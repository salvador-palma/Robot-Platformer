using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckers : MonoBehaviour
{
    // Start is called before the first frame update
    Movement m;
    public GameObject LandingParticle;
    public GameObject JumpingParticle;
    
    public void Start()
    {
        m = Movement.getInstance();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ground":
                switch (gameObject.name)
                {
                    case "Feet":
                        m.isGround = true;
                        m.canDash = true;
                        GameObject p = Instantiate(LandingParticle);
                        p.transform.position = transform.position;
                        break;
                    case "ArmLeft":
                        GetComponent<ParticleSystem>().emissionRate = 17f;
                        m.isWall = true;
                        m.anim.SetBool("Walled", true);
                        break;
                }
            break;
            case "Enemy":
                switch (gameObject.name)
                {
                    case "Soul":
                        StartCoroutine(Utils.Pause(0.06f));
                        Movement m = Movement.getInstance();
                        m.DashReset = true;
                        m.inEnemy = true;
                        m.DashTimerStart = m.DashTimer;
                        Enemy e = collision.GetComponent<Enemy>();
                        e.takeDmg(1);
                        break;
                }
                break;
            case "Water":
                switch (gameObject.name)
                {
                    case "Feet":
                        Movement.getInstance().OnWater();
                        break;
                }
                break;
        }
        
       


    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ground":
                switch (gameObject.name)
                {
                    case "Feet":
                        m.isGround = false;
                        GameObject p = Instantiate(JumpingParticle);
                        p.transform.position = transform.position;

                        break;
                    case "ArmLeft":
                        GetComponent<ParticleSystem>().emissionRate = 0f;
                        m.isWall = false;
                        m.anim.SetBool("Walled", false);
                        break;
                }
                break;
            case "Enemy":
                switch (gameObject.name)
                {
                    case "Soul":
                        Movement.getInstance().inEnemy = false;
                        break;
                }
                break;
            case "Water":
                switch (gameObject.name)
                {
                    case "Feet":
                        Movement.getInstance().OffWater();
                        break;
                }
                break;
        
        }
       

    }
}
