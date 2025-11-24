using UnityEngine;
using IWanna.Player;

namespace IWanna.Level
{
    public class Hazard : MonoBehaviour
    {
        [Header("Hazard Settings")]
        [SerializeField] private HazardType hazardType = HazardType.Spike;
        [SerializeField] private bool isActive = true;
        [SerializeField] private float damage = 1f;
        
        [Header("Animation")]
        [SerializeField] private bool hasAnimation = false;
        [SerializeField] private float animationSpeed = 1f;
        
        [Header("Movement (for moving hazards)")]
        [SerializeField] private bool isMoving = false;
        [SerializeField] private Vector3[] waypoints;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private bool loopMovement = true;
        
        [Header("Audio")]
        [SerializeField] private AudioClip triggerSound;
        [SerializeField] private AudioSource audioSource;
        
        public enum HazardType
        {
            Spike,
            Saw,
            Crusher,
            Laser,
            MovingPlatform
        }
        
        // Components
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        
        // Movement variables
        private int currentWaypointIndex = 0;
        private bool movingForward = true;
        
        // State
        private bool hasTriggered = false;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
        
        private void Start()
        {
            InitializeHazard();
        }
        
        private void Update()
        {
            if (!isActive) return;
            
            HandleMovement();
            HandleAnimation();
        }
        
        private void InitializeHazard()
        {
            // Set up hazard based on type
            switch (hazardType)
            {
                case HazardType.Spike:
                    // Spikes are static by default
                    break;
                    
                case HazardType.Saw:
                    hasAnimation = true;
                    break;
                    
                case HazardType.Crusher:
                    // Crushers might have specific behavior
                    break;
                    
                case HazardType.MovingPlatform:
                    isMoving = true;
                    break;
            }
            
            // Initialize waypoints if moving
            if (isMoving && waypoints.Length == 0)
            {
                waypoints = new Vector3[] { transform.position, transform.position + Vector3.right * 5f };
            }
        }
        
        private void HandleMovement()
        {
            if (!isMoving || waypoints.Length < 2) return;
            
            Vector3 targetPosition = waypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Check if reached waypoint
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                if (loopMovement)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                }
                else
                {
                    if (movingForward)
                    {
                        currentWaypointIndex++;
                        if (currentWaypointIndex >= waypoints.Length - 1)
                        {
                            currentWaypointIndex = waypoints.Length - 1;
                            movingForward = false;
                        }
                    }
                    else
                    {
                        currentWaypointIndex--;
                        if (currentWaypointIndex <= 0)
                        {
                            currentWaypointIndex = 0;
                            movingForward = true;
                        }
                    }
                }
            }
        }
        
        private void HandleAnimation()
        {
            if (!hasAnimation || animator == null) return;
            
            animator.speed = animationSpeed;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                TriggerHazard(player);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isActive) return;
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                TriggerHazard(player);
            }
        }
        
        private void TriggerHazard(PlayerController player)
        {
            if (hasTriggered) return;
            
            hasTriggered = true;
            
            // Play sound effect
            if (triggerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(triggerSound);
            }
            
            // Trigger player death
            player.Die();
            
            // Reset trigger after a short delay
            Invoke(nameof(ResetTrigger), 0.5f);
            
            // Special effects based on hazard type
            switch (hazardType)
            {
                case HazardType.Spike:
                    // Maybe add blood particle effect
                    break;
                    
                case HazardType.Saw:
                    // Add sparks or cutting effect
                    break;
                    
                case HazardType.Crusher:
                    // Add crushing animation
                    break;
            }
        }
        
        private void ResetTrigger()
        {
            hasTriggered = false;
        }
        
        public void SetActive(bool active)
        {
            isActive = active;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = active ? Color.white : Color.gray;
            }
        }
        
        public void AddWaypoint(Vector3 position)
        {
            var waypointList = new System.Collections.Generic.List<Vector3>(waypoints);
            waypointList.Add(position);
            waypoints = waypointList.ToArray();
        }
        
        public void ClearWaypoints()
        {
            waypoints = new Vector3[0];
        }
        
        private void OnDrawGizmos()
        {
            // Draw waypoints for moving hazards
            if (isMoving && waypoints != null && waypoints.Length > 1)
            {
                Gizmos.color = Color.red;
                
                for (int i = 0; i < waypoints.Length; i++)
                {
                    Gizmos.DrawWireSphere(waypoints[i], 0.3f);
                    
                    if (i < waypoints.Length - 1)
                    {
                        Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
                    }
                    else if (loopMovement)
                    {
                        Gizmos.DrawLine(waypoints[i], waypoints[0]);
                    }
                }
            }
            
            // Draw hazard area
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw detailed info when selected
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
        }
    }
}
