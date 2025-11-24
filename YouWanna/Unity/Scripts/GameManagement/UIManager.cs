using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IWanna.GameManagement
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game UI")]
        [SerializeField] private TextMeshProUGUI deathCountText;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Image healthBar;
        
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalDeathCountText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        
        [Header("Settings")]
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        
        private void Start()
        {
            InitializeUI();
            SetupButtonListeners();
        }
        
        private void InitializeUI()
        {
            // Initialize UI elements
            UpdateDeathCount(0);
            SetPauseMenuActive(false);
            SetGameOverActive(false);
            
            // Set initial level name
            if (levelNameText != null)
            {
                levelNameText.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
        }
        
        private void SetupButtonListeners()
        {
            // Pause menu buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
            
            if (restartButton != null)
                restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => GameManager.Instance.LoadLevel("MainMenu"));
            
            if (quitButton != null)
                quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
            
            // Game over buttons
            if (retryButton != null)
                retryButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
            
            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameManager.Instance.LoadLevel("MainMenu"));
            
            // Settings
            if (volumeSlider != null)
            {
                volumeSlider.value = AudioListener.volume;
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
            
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
        }
        
        public void UpdateDeathCount(int deathCount)
        {
            if (deathCountText != null)
            {
                deathCountText.text = $"Deaths: {deathCount}";
            }
        }
        
        public void SetPauseMenuActive(bool active)
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(active);
            }
        }
        
        public void SetGameOverActive(bool active)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(active);
                
                if (active && finalDeathCountText != null)
                {
                    finalDeathCountText.text = $"Final Deaths: {GameManager.Instance.GetDeathCount()}";
                }
            }
        }
        
        public void SetLevelName(string levelName)
        {
            if (levelNameText != null)
            {
                levelNameText.text = levelName;
            }
        }
        
        public void UpdateHealthBar(float healthPercentage)
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = Mathf.Clamp01(healthPercentage);
            }
        }
        
        public void ShowMessage(string message, float duration = 3f)
        {
            // TODO: Implement message display system
            Debug.Log($"UI Message: {message}");
        }
        
        public void SetVolume(float volume)
        {
            AudioListener.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("Volume", volume);
        }
        
        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        }
        
        private void OnDestroy()
        {
            // Clean up button listeners
            if (resumeButton != null)
                resumeButton.onClick.RemoveAllListeners();
            
            if (restartButton != null)
                restartButton.onClick.RemoveAllListeners();
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.RemoveAllListeners();
            
            if (quitButton != null)
                quitButton.onClick.RemoveAllListeners();
            
            if (retryButton != null)
                retryButton.onClick.RemoveAllListeners();
            
            if (menuButton != null)
                menuButton.onClick.RemoveAllListeners();
            
            if (volumeSlider != null)
                volumeSlider.onValueChanged.RemoveAllListeners();
            
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.RemoveAllListeners();
        }
    }
}
