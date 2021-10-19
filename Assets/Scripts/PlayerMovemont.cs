using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovemont : MonoBehaviour
{
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float airborneSpeed;
    [SerializeField] private float airborneControlFactor;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private bool momentJustAfterJump = false; //Moment after jump, player is still close to ground. Horizontal input at running speed must be avoided.

    private bool AnimatorGroundedState
    {
        get { return animator.GetBool("isGrounded"); }
        set { animator.SetBool("isGrounded", value); }
    }

    private bool AnimatorRunningState
    {
        get { return animator.GetBool("isRunning"); }
        set { animator.SetBool("isRunning", value); }
    }

    private float HorizontalInput
    {
        get { return Input.GetAxis("Horizontal"); }
    }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        checkIsGrounded();
        checkJumpInput();
        checkHorizontalShiftInput();
    }

    private void checkHorizontalShiftInput()
    {
        if (isGrounded() && !momentJustAfterJump)
            playerRigidbody.velocity = new Vector2(HorizontalInput * runningSpeed, playerRigidbody.velocity.y);
        else
        {
            playerRigidbody.velocity = new Vector2(getHorizontalAirborneVelocity(), playerRigidbody.velocity.y);
        }
        checkHorizontalSpriteFlip();
        AnimatorRunningState = HorizontalInput != 0;
    }

    private void checkHorizontalSpriteFlip()
    {
        if (HorizontalInput >= 0.1f)
            transform.localScale = Vector3.one;
        else if (HorizontalInput <= -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void checkJumpInput()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            jump();
            momentJustAfterJump = true;
        }
        else
        {
            momentJustAfterJump = false;
        }
    }

    private void checkIsGrounded()
    {
        AnimatorGroundedState = isGrounded();
    }

    private void jump()
    {
        playerRigidbody.velocity = new Vector2(HorizontalInput * airborneSpeed, jumpSpeed);
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.01f, groundLayer);
        return raycastHit.collider != null;
    }

    private float getHorizontalAirborneVelocity()
    {
        float tempVelocity = playerRigidbody.velocity.x + HorizontalInput * airborneControlFactor;
        if (playerRigidbody.velocity.x < 0 && tempVelocity < -airborneSpeed)
            return -airborneSpeed;
        else if (playerRigidbody.velocity.x > 0 && tempVelocity > airborneSpeed)
            return airborneSpeed;
        else
            return tempVelocity;
    }
}
