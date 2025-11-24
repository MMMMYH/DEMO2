using UnityEngine;

namespace IWanna.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float gravityScale = 3f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayerMask;
        
        [Header("Wall Detection")]
        [SerializeField] private Transform wallCheck;
        [SerializeField] private float wallCheckDistance = 0.5f;
        
        // Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        
        // Input
        private float horizontalInput;
        private bool jumpInput;
        private bool jumpInputDown;
        
        // State
        private bool isGrounded;
        private bool isTouchingWall;
        private bool facingRight = true;
        
        // Events
        public System.Action OnPlayerDeath;
        public System.Action OnPlayerJump;
        public System.Action OnPlayerLand;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            
            rb.gravityScale = gravityScale;
        }
        
        private void Update()
        {
            HandleInput();
            CheckGrounded();
            CheckWallCollision();
            HandleMovement();
            HandleJump();
            HandleGravity();
            HandleAnimations();
        }
        
        private void HandleInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            jumpInput = Input.GetButton("Jump");
            jumpInputDown = Input.GetButtonDown("Jump");
        }
        
        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
            
            if (!wasGrounded && isGrounded)
            {
                OnPlayerLand?.Invoke();
            }
        }
        
        private void CheckWallCollision()
        {
            RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, 
                facingRight ? Vector2.right : Vector2.left, 
                wallCheckDistance, groundLayerMask);
            
            isTouchingWall = hit.collider != null;
        }
        
        private void HandleMovement()
        {
            // Horizontal movement
            float targetVelocityX = horizontalInput * moveSpeed;
            rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
            
            // Flip sprite based on movement direction
            if (horizontalInput > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontalInput < 0 && facingRight)
            {
                Flip();
            }
        }
        
        private void HandleJump()
        {
            if (jumpInputDown && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                OnPlayerJump?.Invoke();
            }
        }
        
        private void HandleGravity()
        {
            // Enhanced gravity for better jump feel
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !jumpInput)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
        
        private void HandleAnimations()
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
                animator.SetBool("IsGrounded", isGrounded);
                animator.SetFloat("VelocityY", rb.velocity.y);
            }
        }
        
        private void Flip()
        {
            facingRight = !facingRight;
            spriteRenderer.flipX = !facingRight;
        }
        
        public void Die()
        {
            OnPlayerDeath?.Invoke();
            // Reset position or trigger death animation
            GameManager.Instance.PlayerDied();
        }
        
        public void Respawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            rb.velocity = Vector2.zero;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Hazard"))
            {
                Die();
            }
            else if (other.CompareTag("SavePoint"))
            {
                GameManager.Instance.SetCheckpoint(transform.position);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw ground check
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
            
            // Draw wall check
            if (wallCheck != null)
            {
                Gizmos.color = isTouchingWall ? Color.blue : Color.white;
                Vector3 direction = facingRight ? Vector3.right : Vector3.left;
                Gizmos.DrawRay(wallCheck.position, direction * wallCheckDistance);
            }
        }
    }
}
