using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Player Values")]
    [SerializeField] private int health = 1;
    [SerializeField] private int gems = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int jumpsAmount;
    private int jumpsLeft;

    [Header("Assign in Inspector")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private float scaleX;

    [Header("Bools")]
    public bool grounded;
    public bool facingRight = true;
    public bool canCompleteLevel = true;

    [Header("Ball Variables")]
    [SerializeField] private float kickForce = 10f;
    public bool withBall;
    public bool isShooting;
    [SerializeField] public BoxCollider2D ballCheck;
    [SerializeField] public Rigidbody2D ballRB;
    public ControlTrail ballScript;


    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        scaleX = transform.localScale.x;
        ballRB = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>();
        ballScript = ballRB.gameObject.GetComponent<ControlTrail>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.current.PauseGame();
        }

        if (GameManager.current.gamePaused) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        // jump
        Jump();

        // check for movement input
        Move();

        // kick the ball -- if player has it
        if (Input.GetMouseButtonDown(0) && withBall)
        {
            Shoot();
        }

        // win game
        // conditions: player is at the door, player is pressing space and the ball has already gone through the door
        if(Input.GetKey(KeyCode.Space) && canCompleteLevel && GameManager.current.ballInDoor)
        {
            gameObject.SetActive(false);
            GameManager.current.WinGame();
        }

        // determines if the trail effect on the ball should be on
        if(withBall)
        {
            ballScript.effectOn = false;
        }
        else
        {
            ballScript.effectOn = true;
        }
    }

    void Move()
    {
        Flip();

        // is player running?
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = 7;
        }
        else
        {
            moveSpeed = 5;
        }

        // move player if there's input
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ResetJumps();
            if (jumpsLeft > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsLeft--;
            }
        }
    }

    void Flip()
    {
        // look left
        if (moveInput > 0)
        {
            // temporarily parent the ball object so it flips with the player
            if (withBall)
            {
                ballRB.transform.parent = this.transform;
            }

            // flip obj
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

            // deparent ball
            if (withBall)
            {
                ballRB.transform.parent = null;
            }
        }
        // look right
        if (moveInput < 0)
        {
            // temporarily parent the ball object so it flips with the player
            if (withBall)
            {
                ballRB.transform.parent = this.transform;
            }

            // flip obj
            transform.localScale = new Vector3((-1) * scaleX, transform.localScale.y, transform.localScale.z);

            // deparent ball
            if (withBall)
            {
                ballRB.transform.parent = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // collided with bullet, should take damage
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
        // collided with enemy, should take damage
        else if (other.gameObject.tag == "Enemy")
        {
            TakeDamage();
        }
        // player is on the ground
        else if (other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        // collect gem
        else if (other.gameObject.tag == "Coin")
        {
            CollectGem();
            Destroy(other.gameObject);
        }
        // is in range of the level exit
        else if (other.gameObject.name == "LevelComplete")
        {
            canCompleteLevel = true;
        }
    }

    private void CollectGem()
    {
        gems++;
        GameManager.current.CollectCoin();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // no longer on ground
        if (other.gameObject.tag == "Ground")
        {
            grounded = false;
        }
        // no longer in exit door range
        if(other.gameObject.name == "LevelComplete")
        {
            canCompleteLevel = false;
        }
    }

    // currently player dies immediately from any damage
    private void TakeDamage()
    {
        GameManager.current.LoseGame();
    }

    // called from BallDetection script to determine if player is moving ball
    public void BallCheckTriggered(bool enter, Rigidbody2D other)
    {
        // if enter is true: player is moving ball, and no longer shooting
        if (enter)
        {
            withBall = true;
            isShooting = false;
        }
        // if enter is false: ball is no longer with player
        // either the player kicked the ball or the ball rolled out of reach
        else if (!enter && withBall == true)
        {
            withBall = false;
            // if the ball rolled out of reach, stop the ball from rolling 
            if (!isShooting)
            {
                ballRB.velocity = Vector2.zero;
                ballRB.angularVelocity = 0;
            }
        }
    }

    // player kicking the ball
    void Shoot()
    {
        isShooting = true;

        // Get mouse position on screen and turn that into a
        // direction for the ball to go in
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (Vector2)((worldMousePos - transform.position));
        direction.Normalize();

        // Add velocity to ball
        ballRB.velocity = direction * kickForce;
    }
}

