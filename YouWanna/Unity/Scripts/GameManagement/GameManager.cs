using UnityEngine;
using UnityEngine.SceneManagement;

namespace IWanna.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        [SerializeField] private Vector3 defaultSpawnPoint = Vector3.zero;
        [SerializeField] private float respawnDelay = 1f;
        
        [Header("UI References")]
        [SerializeField] private UIManager uiManager;
        
        // Game State
        private Vector3 currentCheckpoint;
        private int deathCount = 0;
        private bool isGamePaused = false;
        
        // Player Reference
        private PlayerController player;
        
        // Events
        public System.Action<int> OnDeathCountChanged;
        public System.Action OnGamePaused;
        public System.Action OnGameResumed;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            InitializeGame();
        }
        
        private void Start()
        {
            FindPlayer();
            SetCheckpoint(defaultSpawnPoint);
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void InitializeGame()
        {
            // Set target framerate for consistent physics
            Application.targetFrameRate = 60;
            
            // Initialize game state
            deathCount = 0;
            isGamePaused = false;
        }
        
        private void FindPlayer()
        {
            player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.OnPlayerDeath += PlayerDied;
            }
        }
        
        private void HandleInput()
        {
            // Pause/Resume game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
            
            // Restart level
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
            
            // Quick suicide (common in I Wanna games)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (player != null)
                {
                    player.Die();
                }
            }
        }
        
        public void PlayerDied()
        {
            deathCount++;
            OnDeathCountChanged?.Invoke(deathCount);
            
            if (uiManager != null)
            {
                uiManager.UpdateDeathCount(deathCount);
            }
            
            // Respawn player after delay
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
        
        private void RespawnPlayer()
        {
            if (player != null)
            {
                player.Respawn(currentCheckpoint);
            }
        }
        
        public void SetCheckpoint(Vector3 position)
        {
            currentCheckpoint = position;
            Debug.Log($"Checkpoint set at: {position}");
        }
        
        public void TogglePause()
        {
            isGamePaused = !isGamePaused;
            
            if (isGamePaused)
            {
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
            }
            else
            {
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
            }
            
            if (uiManager != null)
            {
                uiManager.SetPauseMenuActive(isGamePaused);
            }
        }
        
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            deathCount = 0;
            OnDeathCountChanged?.Invoke(deathCount);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void LoadLevel(string levelName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelName);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        // Getters
        public int GetDeathCount() => deathCount;
        public bool IsGamePaused() => isGamePaused;
        public Vector3 GetCurrentCheckpoint() => currentCheckpoint;
        
        private void OnDestroy()
        {
            if (player != null)
            {
                player.OnPlayerDeath -= PlayerDied;
            }
        }
    }
}
