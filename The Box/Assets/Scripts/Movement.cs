using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    static Movement INSTANCE;

    [Header("Stats")]
    public int Health;

    [Header("Settings")]
    private float speedActive;
    private int jumpForceActive;
    private float gravityDownActive;
    private float gravityUpActive;

    public float speed;
    public int jumpForce;
    public float gravityDown;
    public float gravityUp;

    float speedWater = 200;
    int jumpForceWater = 6;
    float gravityDownWater = 0.05f;
    float gravityUpWater = 0.1f;
    
    public int doubleJumpForce;
    public float jumpCancelForce;
    public float rollBack;
    public float wallSlideSpeed;
    public float wallJumpForce;
    public float wallJumpHorizontalForce;
    public float dashingPower;
    public float KbForce;

    [Header("Timers")]
    private float jumpBuffer = 0.2f;
    private float jumpBufferCounter;

    private float coyoteBuffer = 0.2f;
    private float coyoteBufferCounter;

    public float ghostSpawnRate;
    float ghostSpawnRateStart;
    public GameObject ghost;
    public float turnBackWaitTimer;
    float turnBackWaitTimerStart;
    bool isturnBackWait;
    public float DashTimer;
    public float DashTimerStart;
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
    public bool DashReset;
    public bool inEnemy;
    public bool isDashingExtended;
    public bool onWater;
    public bool headAboveWater;
    public bool onSurface;
    
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

    float ogGravity;

    public CameraMovement CamShake;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        INSTANCE = this;
        anim = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        jumpForceActive = jumpForce;
        speedActive = speed;
        gravityDownActive = gravityDown;
        gravityUpActive = gravityUp;
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
            if(DashTimerStart > 0)
            {
                DashTimerStart -= Time.deltaTime;
            }
            else
            {
                if (inEnemy)
                {
                    isDashingExtended = true;
                }
                else
                {
                    EndDash();
                }
            }
            updateGhost();
            return;
        }
        if (!stop_player)
        {
            if (isGround)
            {
                hasJumped = false;
                coyoteBufferCounter = coyoteBuffer;
            }else if(coyoteBufferCounter > 0)
            {
                coyoteBufferCounter -= Time.deltaTime;
            }
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBuffer;
            }
            else if(jumpBufferCounter > 0)
            {
                jumpBufferCounter -= Time.deltaTime;
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

            if (jumpBufferCounter>0)
            {
                anim.SetBool("Jumped", true);
                if (coyoteBufferCounter > 0 || onWater)//jump
                {
                    jumpBufferCounter = 0f;
                    rb.velocity = new Vector2(rb.velocity.x, jumpForceActive);
                    if (onWater)
                    {
                        anim.Play("SwimReset");
                    }
                }
                else if (isWall && act_dir == 0)//walljump
                {
                    jumpBufferCounter = 0f;
                    int w = facingRight ? 1 : -1;
                    rb.velocity = new Vector3(rb.velocity.x + (wallJumpHorizontalForce * ( w  * (-1))), wallJumpForce, 0);
                    canDash = true;
                    Flip();
                }/*
                else if (!hasJumped)//doublejump
                {
                    hasJumped = true;
                    rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                }*/

            }
            else if (Input.GetKeyDown(KeyCode.S) && !isGround)//fastfall
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
                //StartCoroutine(Dash());
                StartDash();
            }




            updateGhost();

            if (!isWall)
            {

                if (facingRight && act_dir == -1) { Flip(); }
                if (!facingRight && act_dir == 1) { Flip(); }
            }
            else if (!isGround && !onWater)
            {
                
                anim.Play("Wall_Slide");

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
            if (!isGround && rb.velocity.y < 0.5)
            {
                anim.SetInteger("YDir", -1);
                if (isWall)
                {
                    int w = facingRight ? 1 : -1;
                    if (act_dir != w)
                    {

                        rb.velocity = new Vector3(horizontalAxis * speedActive * Time.deltaTime, rb.velocity.y - gravityDownActive);
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
                        rb.velocity = new Vector2(horizontalAxis * speedActive * Time.deltaTime, rb.velocity.y - gravityDownActive);
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityDownActive);
                    }
                }
            }
            else if (!isGround)
            {
                anim.SetInteger("YDir", 1);
                if (act_dir != 0)
                    {
                        rb.velocity = new Vector2(horizontalAxis * speedActive * Time.deltaTime, rb.velocity.y - gravityUpActive);
                    
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravityUpActive);
                    }
            }
            else if (isGround)
            {
                anim.SetInteger("YDir", 0);
                
                if (horizontalAxis != 0) 
                { 
                    anim.SetBool("Moving", true);
                    rb.velocity = new Vector2(horizontalAxis * speedActive * Time.deltaTime, rb.velocity.y);

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
       
       
    }
    private void EndTurnBack()
    {
        turnBackWaitTimerStart = turnBackWaitTimer;
        isturnBackWait = true;
    }
    public IEnumerator GoBack(GameObject[] go)
    {
        FindObjectOfType<CameraMaterial>().Emit(Camera.main.WorldToViewportPoint(transform.position), 0.5f, 0.0075f);
        sprRenderer.color = Color.cyan;
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
        sprRenderer.color = Color.white;
        anim.enabled = true;
        GetComponent<Animator>().Play("EndTurnBack");
    }
    public void StartDash()
    {
        StartCoroutine(CamShake.Shake(0.1f, 0.05f));
        
        gameObject.layer = 6;
        soul.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        anim.SetBool("Dashing", true);
        anim.Play("Dash");
        int dir;
        if (act_dir != 0) { dir = act_dir; }
        else
        {
            dir = facingRight ? 1 : -1;
        }
        canDash = false;
        ogGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dir * dashingPower, 0f);
        isDashing = true;

        DashTimerStart = DashTimer;

    }
    void EndDash()
    {
        if (onWater) { anim.Play("SwimReset"); }
        anim.SetBool("Dashing", false);
        gameObject.layer = 0;
        soul.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = ogGravity;
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (isGround || onWater) { canDash = true; }
        else if (DashReset)
        {
            DashReset = false;
            canDash = true;
        }
    }
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    void updateGhost()
    {
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
            g.transform.localScale = transform.localScale;
            g.transform.position = pos;
            ghostSpawnRateStart = ghostSpawnRate;
        }
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

    
    public void OnWater()
    {
        switchWaterSettings(true);
        anim.SetBool("Water", true);
        onWater = true;
    }
    public void OffWater()
    {
        switchWaterSettings(false);
        anim.SetBool("Water", false);
        onWater = false;
    }
    public void switchWaterSettings(bool wentInWater)
    {
        if (wentInWater)
        {
            speedActive = speedWater;
            jumpForceActive = jumpForceWater;
            gravityUpActive = gravityUpWater;
            gravityDownActive = gravityDownWater;
        }
        else
        {
            speedActive = speed;
            jumpForceActive = jumpForce;
            gravityUpActive = gravityUp;
            gravityDownActive = gravityDown;
        } 
    }
    
    public void updateOnSurface()
    {
         jumpForceActive = headAboveWater && onWater ? jumpForce : jumpForceActive;
    }
    
    

}


