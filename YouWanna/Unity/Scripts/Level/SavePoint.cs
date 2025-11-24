using UnityEngine;
using IWanna.Player;
using IWanna.GameManagement;

namespace IWanna.Level
{
    public class SavePoint : MonoBehaviour
    {
        [Header("Save Point Settings")]
        [SerializeField] private bool isActive = true;
        [SerializeField] private bool isOneTimeUse = false;
        [SerializeField] private float activationRadius = 1f;
        
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color inactiveColor = Color.gray;
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color usedColor = Color.green;
        [SerializeField] private GameObject activationEffect;
        
        [Header("Animation")]
        [SerializeField] private bool hasIdleAnimation = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.3f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip activationSound;
        [SerializeField] private AudioSource audioSource;
        
        // State
        private bool hasBeenUsed = false;
        private bool playerInRange = false;
        private Color originalColor;
        private Vector3 originalScale;
        
        // Components
        private Animator animator;
        private Collider2D col;
        
        // Events
        public System.Action<Vector3> OnSavePointActivated;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
            
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
        
        private void Start()
        {
            InitializeSavePoint();
        }
        
        private void Update()
        {
            if (hasIdleAnimation && isActive && !hasBeenUsed)
            {
                HandleIdleAnimation();
            }
        }
        
        private void InitializeSavePoint()
        {
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
                originalScale = transform.localScale;
                
                UpdateVisualState();
            }
            
            // Set up collider as trigger
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
        
        private void HandleIdleAnimation()
        {
            if (spriteRenderer == null) return;
            
            // Pulse effect
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            float alpha = 1f + pulse;
            
            Color currentColor = spriteRenderer.color;
            currentColor.a = alpha;
            spriteRenderer.color = currentColor;
            
            // Scale pulse
            Vector3 scale = originalScale * (1f + pulse * 0.1f);
            transform.localScale = scale;
        }
        
        private void UpdateVisualState()
        {
            if (spriteRenderer == null) return;
            
            if (!isActive)
            {
                spriteRenderer.color = inactiveColor;
            }
            else if (hasBeenUsed)
            {
                spriteRenderer.color = usedColor;
            }
            else
            {
                spriteRenderer.color = activeColor;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                playerInRange = true;
                
                if (isActive && (!isOneTimeUse || !hasBeenUsed))
                {
                    ActivateSavePoint();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                playerInRange = false;
            }
        }
        
        private void ActivateSavePoint()
        {
            if (!isActive || (isOneTimeUse && hasBeenUsed))
                return;
            
            // Mark as used
            hasBeenUsed = true;
            
            // Set checkpoint in game manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetCheckpoint(transform.position);
            }
            
            // Play activation sound
            if (activationSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(activationSound);
            }
            
            // Show activation effect
            if (activationEffect != null)
            {
                GameObject effect = Instantiate(activationEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Update visual state
            UpdateVisualState();
            
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger("Activate");
            }
            
            // Invoke event
            OnSavePointActivated?.Invoke(transform.position);
            
            Debug.Log($"Save point activated at {transform.position}");
        }
        
        public void SetActive(bool active)
        {
            isActive = active;
            UpdateVisualState();
            
            if (col != null)
            {
                col.enabled = active;
            }
        }
        
        public void ResetSavePoint()
        {
            hasBeenUsed = false;
            UpdateVisualState();
        }
        
        public bool IsUsed()
        {
            return hasBeenUsed;
        }
        
        public bool IsPlayerInRange()
        {
            return playerInRange;
        }
        
        // Manual activation (for testing or special cases)
        [ContextMenu("Activate Save Point")]
        public void ManualActivate()
        {
            ActivateSavePoint();
        }
        
        private void OnDrawGizmos()
        {
            // Draw activation radius
            Gizmos.color = isActive ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, activationRadius);
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw detailed info when selected
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, activationRadius);
            
            // Draw save point info
            if (Application.isPlaying)
            {
                Gizmos.color = hasBeenUsed ? Color.green : Color.yellow;
                Gizmos.DrawSphere(transform.position, 0.2f);
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Update visual state in editor
            if (spriteRenderer != null && !Application.isPlaying)
            {
                UpdateVisualState();
            }
        }
        #endif
    }
}
