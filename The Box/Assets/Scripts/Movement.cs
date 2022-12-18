using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    static Movement INSTANCE;

    [Header("Stats")]
    public int Health;

    [Header("Settings")]
    public float speed;
    public int jumpForce;
    public int doubleJumpForce;
    public float jumpCancelForce;
    public float gravityDown;
    public float gravityUp;
    public float rollBack;
    public float wallSlideSpeed;
    public float wallJumpForce;
    public float wallJumpHorizontalForce;
    public float dashingPower;
    public float KbForce;

    [Header("Timers")]
    public float ghostSpawnRate;
    float ghostSpawnRateStart;
    public GameObject ghost;
    public float turnBackWaitTimer;
    float turnBackWaitTimerStart;
    bool isturnBackWait;
    public float DashTimer;
    public float DashCooldown;
    public float HurtTimer;
    public float InvencibleTimer;

    [Header("Boolean")]
    public bool isGround;
    public bool isWall;
    public bool hasJumped;
    public bool canTurnBack;
    public bool facingRight;
    public bool isDashing;
    public bool canDash;
    public bool isHurt;
    public bool isInvencible;

    [Header("Random")]
    public int wallClinged;
    
    public float horizontalAxis;
    public Transform feet;
    public Transform armL;
    public Transform armR;
    public Transform soul;
    public LayerMask groundLayer;
    
    public float RotationSpeed;
    Rigidbody2D rb;

    public Vector3 lastGhost;
    public Camera main;
    public bool stop_player = false;
    public bool locked_input;
    int act_dir;

    public Animator anim;
    public SpriteRenderer sprRenderer;

    public float ground_speed_x;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        INSTANCE = this;
        anim = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }
    public static Movement getInstance()
    {
        if(INSTANCE == null)
        {
            return new Movement();
        }
        return INSTANCE;
    }
    // Update is called once per frame
    void Update()
    {
        if (isDashing || isHurt)
        {
            return;
        }
        if (!stop_player)
        {
            if (isGrounded())
            {
                hasJumped = false;
            }

            horizontalAxis = Input.GetAxis("Horizontal");

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                act_dir = horizontalAxis > 0 ? 1 : horizontalAxis < 0 ? -1 : 0;
            }
            else
            {
                act_dir = 0;
            }
            anim.SetInteger("ActDir", act_dir);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("Jumped", true);
                if (isGrounded())//jump
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
                else if (isWalled() && act_dir == 0)//walljump
                {

                    facingRight = !facingRight;
                    anim.SetBool("FacingRight", facingRight);
                    rb.velocity = new Vector3(rb.velocity.x + (wallJumpHorizontalForce * (wallClinged * (-1))), wallJumpForce, 0);

                }
                else if (!hasJumped)//doublejump
                {
                    hasJumped = true;
                    rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                }

            }
            else if (Input.GetKeyDown(KeyCode.S) && !isGrounded())//fastfall
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpCancelForce);
            }
            else if (Input.GetKeyUp(KeyCode.Space))//break jump
            {
                anim.SetBool("Jumped", false);
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) && canTurnBack)//turn back
            {
                StartTurnBack();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && canDash)//DASH
            {
                StartCoroutine(Dash());
            }




            if (ghostSpawnRateStart > 0)
            {
                ghostSpawnRateStart -= Time.deltaTime;
            }
            else
            {
                GameObject g = Instantiate(ghost);
                Vector3 pos = transform.position;
                g.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                g.GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX;

                g.transform.position = pos;
                ghostSpawnRateStart = ghostSpawnRate;
            }

            if (!isWalled())
            {

                if (facingRight && act_dir == -1) { Flip(); }
                if (!facingRight && act_dir == 1) { Flip(); }
            }
            else if (!isGrounded())
            {

                if (wallClinged == 1)
                {
                    anim.Play("Wall_Slide");
                }
                else
                {
                    anim.Play("Wall_Slide_Flip");
                }
            }

        }
        else
        {
            
            if (isturnBackWait)
            {
                if (turnBackWaitTimerStart > 0)
                {
                    turnBackWaitTimerStart -= Time.deltaTime;
                }
                else
                {
                    isturnBackWait = false;
                    stop_player = false;
                    rb.gravityScale = 1f;
                    GetComponent<TrailRenderer>().time = 1.2f;
                    canTurnBack = false;
                }
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (isDashing || isHurt)
        {
            return;
        }
        if (!stop_player)
        {
            if (!isGrounded() && rb.velocity.y < 0.5)
            {
                anim.SetInteger("YDir", -1);
                if (isWalled())
                {
                    if (act_dir != wallClinged)
                    {

                        rb.velocity = new Vector3(horizontalAxis * speed * Time.deltaTime, rb.velocity.y - gravityDown);
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y * wallSlideSpeed );
                    }
                }
                else
                {
                    if (act_dir != 0)
                    {
                        rb.velocity = new Vector2(horizontalAxis * speed * Time.deltaTime, rb.velocity.y - gravityDown);
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityDown);
                    }
                }
            }
            else if (!isGrounded())
            {
                anim.SetInteger("YDir", 1);
                if (act_dir != 0)
                    {
                        rb.velocity = new Vector2(horizontalAxis * speed * Time.deltaTime, rb.velocity.y - gravityUp);
                    
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityUp);
                    }
            }
            else if (isGrounded())
            {
                anim.SetInteger("YDir", 0);
                
                if (horizontalAxis != 0) 
                { 
                    anim.SetBool("Moving", true);
                    rb.velocity = new Vector2(horizontalAxis * speed * Time.deltaTime, rb.velocity.y);

                } 
                else 
                { 
                    anim.SetBool("Moving", false);
                    rb.velocity = new Vector2(ground_speed_x, rb.velocity.y);
                }
            }

            
            

        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    private bool isGrounded()
    { 
        return isGround;
    }
    private bool isWalled()
    {
        return isWall;
    }
    public Vector3 getPosition()
    {
        return gameObject.transform.position;
    }
    private void StartTurnBack()
    {
        
        stop_player = true;
        rb.gravityScale = 0f;
        GetComponent<TrailRenderer>().time = 0f;
        GetComponent<Animator>().Play("StartTurnBack");
        

    }
    private void TurnBack()
    {
       
        anim.enabled = false;
        GameObject[] clones = GameObject.FindGameObjectsWithTag("Ghost");
        
        foreach(GameObject g in clones)
        {
            g.GetComponent<Animator>().speed = 0;
        }
       
        StartCoroutine(GoBack(clones));
       // main.GetComponent<RippleEffect>().Emit(0.5f, 0.5f);
       
    }
    private void EndTurnBack()
    {
        
        //yhmain.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        turnBackWaitTimerStart = turnBackWaitTimer;
        isturnBackWait = true;
    }
    public IEnumerator GoBack(GameObject[] go)
    {
        for (int i = go.Length-1; i >= 0; i--)
        {
            if (go[i] != null)
            {
                GameObject g = go[i];
                transform.position = g.transform.position;
                sprRenderer.flipX = g.GetComponent<SpriteRenderer>().flipX;
                sprRenderer.sprite = g.GetComponent<SpriteRenderer>().sprite;

                Destroy(go[i]);
                yield return new WaitForSecondsRealtime(rollBack);
            }
        }
        anim.enabled = true;
        GetComponent<Animator>().Play("EndTurnBack");
    }
    
    public IEnumerator Dash()
    {
        gameObject.layer = 6;
        soul.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        if (facingRight)
        {
            anim.Play("Dash_Flip");
        }
        else
        {
            anim.Play("Dash");
        }
        
        
        int dir;
        if (act_dir != 0) { dir = act_dir; }
        else
        {
            if (facingRight)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }
        }

        canDash = false;
        isDashing = true;
        float ogGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dir * dashingPower, 0f);
        yield return new WaitForSeconds(DashTimer);
        gameObject.layer = 0;
        soul.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = ogGravity;
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (isGrounded()) { canDash = true; }
    }
    void Flip()
    {
        facingRight = !facingRight;
        anim.SetBool("FacingRight", facingRight);
    }

    public IEnumerator KnockBack(int dir )
    {
        anim.SetBool("Hurt", true);
        anim.Play("Hurt");
        isInvencible = true;
        isHurt = true;

        rb.velocity = new Vector2(dir * KbForce, 1f* KbForce);
        yield return new WaitForSeconds(HurtTimer);
        anim.SetBool("Hurt", false);
        isHurt = false;
        yield return new WaitForSeconds(InvencibleTimer);
        isInvencible = false;
    }

    public void takeDmg(int n, int dir)
    {
        StartCoroutine(KnockBack(dir));
        Health -= n;
        if(Health <= 0) { Debug.Log("Dead"); }
    }
}


