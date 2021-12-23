using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
    public gameController Game;

    [Header("Variables")]  
    [SerializeField] float  speed = 10.0f;
    [SerializeField] float  jumpForce = 7.5f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] LayerMask wallLayerMask;
    [Header("Effects")]
    [SerializeField] GameObject JumpDust;
    [SerializeField] GameObject LandingDust;

    private Animator        animator;
    private Rigidbody2D     rb;
    private BoxCollider2D   col;
    private Sensor_Prototype groundSensor;
    private int             facingDirection = 1;

    #region Getters and Setters
    public float Speed {
        get {return speed;} set {speed = value;}
    }

    public BoxCollider2D Collider {
        get {return col;}
    }
    #endregion

    // initialize; called once at the first frame
    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        animator.SetLayerWeight(1, 1); // hide the sword from the prefab
    }

    private bool IsGrounded() {
        float buffer = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, buffer, groundLayerMask);

        Color rayColor;
        if (raycastHit.collider != null) { // grounded
            rayColor = Color.green;
        } else { // not grounded
            rayColor = Color.red;
        }

        Debug.DrawRay(col.bounds.center, Vector2.down * (col.bounds.extents.y + buffer), rayColor);
        return raycastHit.collider != null;
    }

    private void Movement() {
        // using unity axis to move horizontal
        float inputHorizontal = Input.GetAxis("Horizontal");

        if (inputHorizontal > 0) {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        } else if (inputHorizontal < 0) {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }

        rb.velocity = new Vector2((inputHorizontal * speed), rb.velocity.y);

        // take keydown input for jumping; W, Space, Up Arrow
        if (IsGrounded() && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // handle animation
            animator.SetTrigger("Jump");
            animator.SetBool("Grounded", false);
        }
    }

    private void Animations() {
        // update grounded for animation
        animator.SetBool("Grounded", IsGrounded());
        // set airspeed in animator
        animator.SetFloat("AirSpeedY", rb.velocity.y);
        
        // idle or moving
        if (Mathf.Abs(rb.velocity.x) > 0)
            animator.SetInteger("AnimState", 1);
        else
            animator.SetInteger("AnimState", 0);
    }

    // function taken from PrototypeHeroDemo.cs; function for spawning the dust particles
    void SpawnDustEffect(GameObject dust, float dustXOffset = 0) {
        if (dust != null) {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * facingDirection, 0.0f, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(facingDirection, 1, 1);
        }
    }

    private void VelocityLimit() {
        // function to prevent the player from going subsonic speeds; part solution to wall sliding issue
        if (rb.velocity.y >= 7.5f)
            rb.velocity = new Vector2(rb.velocity.x, 7.5f);
        else if (rb.velocity.y <= -7.5f)
            rb.velocity = new Vector2(rb.velocity.x, -7.5f);

        if (rb.velocity.x >= 10f)
            rb.velocity = new Vector2(10f, rb.velocity.y);
        else if (rb.velocity.x <= -10f)
            rb.velocity = new Vector2(-10f, rb.velocity.y);
    }
    
    private void PreventWallSliding() {
        float buffer = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, buffer, wallLayerMask);

        if (raycastHit)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 2f);
    }

    // select animation events that are used from PrototypeHeroDemo.cs
    void AE_Jump() {
        SpawnDustEffect(JumpDust);
    }

    void AE_Landing() {
        SpawnDustEffect(LandingDust);
    }

    // called once per frame
    void Update() {
        if (Game.GameState == GameStates.Active) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            PreventWallSliding();
            Movement();
            VelocityLimit();
        }
        else {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        Animations();
    }
}
