using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerController playerController;
    public AudioController audioController;

    public GameObject StartPanel;

    [Header("Character Select")]
    public GameObject characterSelectPanel;
    public Button kevinButton;
    public Button willyButton;
    public Button mcdonButton;

    public GameObject gameOverPanel;
    public GameObject ScorePanel;

    [Header("Info Panel")]
    public GameObject infoPanelInGame;
    public Button infoCloseButton;
    private bool infoShown = false;
    public GameObject infoPanelMenu;
    public GameObject infoPanelMenuButton;

    public bool canPlay = false;
    public bool gameOver = false;
    public bool isPaused = false;
    public GameObject pipesParent;
    public Button playButton;
    public Button pauseButton;

    [Header("Sprites del botón Pausa/Play")]
    public Sprite pauseSprite;
    public Sprite playSprite;

    private Image pauseButtonImage;
    private float restartRequestTime = -1f;
    private void Awake()
    {
        instance = this;

        canPlay = false;
        Time.timeScale = 0f;

        StartPanel.SetActive(true);
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(false);

        //intentar borrar esta linea
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);

        if (playButton != null)
            playButton.onClick.AddListener(OpenCharacterSelect);

        if (kevinButton != null)
            kevinButton.onClick.AddListener(() => SelectCharacter(0));
        if (willyButton != null)
            willyButton.onClick.AddListener(() => SelectCharacter(1));
        if (mcdonButton != null)
            mcdonButton.onClick.AddListener(() => SelectCharacter(2));

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
            pauseButtonImage = pauseButton.GetComponent<Image>();
        }

        if (infoPanelInGame != null)
            infoPanelInGame.SetActive(false);

    }

    public void ToggleInfoPanelMenu()
    {
        if (infoPanelMenu != null)
        {
            bool isActive = infoPanelMenu.activeSelf;
            infoPanelMenu.SetActive(!isActive);
        }
    }

    private void OpenCharacterSelect()
    {
        StartPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void SelectCharacter(int num)
    {
        playerController.SelectCharacter(num);
        characterSelectPanel.SetActive(false);
        ShowInfoPanel();
    }

    private void ShowInfoPanel()
    {
        if (infoPanelInGame != null)
        {
            infoPanelInGame.SetActive(true);
            Time.timeScale = 0f;   // Congela el juego
            canPlay = false;
            infoShown = true;

            if (infoCloseButton != null)
                infoCloseButton.onClick.AddListener(CloseInfoPanel);
        }
    }

    private void CloseInfoPanel()
    {
        if (infoPanelInGame != null && infoShown)
        {
            infoPanelInGame.SetActive(false);
            infoShown = false;

            StartGame(); // Inicia el juego normalmente
        }
    }

    public void StartGame()
    {
        canPlay = true;
        playerController.ToggleRigidBody();
        Time.timeScale = 1f;

        StartPanel.SetActive(false);
        ScorePanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        infoPanelMenuButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        if (pauseButtonImage != null && pauseSprite != null)
            pauseButtonImage.sprite = pauseSprite;

        audioController.GameThemeMusic();
    }
    private void Update()
    {
        if (restartRequestTime > 0f && Time.unscaledTime - restartRequestTime >= 0.5f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            // Reanudar
            isPaused = false;
            Time.timeScale = 1f;
            canPlay = true;

            if (pauseButtonImage != null && pauseSprite != null)
                pauseButtonImage.sprite = pauseSprite;
        }
        else
        {
            // Pausar
            isPaused = true;
            Time.timeScale = 0f;
            canPlay = false;

            audioController.StopAllSFX();

            if (pauseButtonImage != null && playSprite != null)
                pauseButtonImage.sprite = playSprite;
        }
    }

    public void ToggleCanPlay()
    {
        if (canPlay)
        {
            canPlay = false;
            playerController.ToggleRigidBody();
        }
        else
        {
            canPlay = true;
            playerController.ToggleRigidBody();
        }
    }
    public void CallGameOver()
    {
        gameOverPanel.SetActive(true);
        ScorePanel.SetActive(false);

        gameOver = true;
        ToggleCanPlay();

        Time.timeScale = 0f;

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);

        audioController.GameOverMusic();
        audioController.StopAllSFX();
    }
    public void RestartButton()
    {
        audioController.ButtonSFX();
        restartRequestTime = Time.unscaledTime;
    }
    public void RestartGame()
    {
        //PowerUpItem.activePowerUps = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // ---------------- POWER UP UI ----------------
    [Header("PowerUp UI")]
    public Image[] powerUpIcons;          // Asigna aquí tus 3 imágenes de UI
    public Sprite activePowerUpSprite;    // Sprite cuando el PowerUp está disponible
    public Sprite inactivePowerUpSprite;  // Sprite cuando no está disponible

    public void UpdatePowerUpUI(int currentCount, int maxCount)
    {
        if (powerUpIcons == null || powerUpIcons.Length == 0) return;

        for (int i = 0; i < powerUpIcons.Length; i++)
        {
            if (i < currentCount)
                powerUpIcons[i].sprite = activePowerUpSprite;
            else
                powerUpIcons[i].sprite = inactivePowerUpSprite;

            // Oculta íconos si superan el máximo permitido
            powerUpIcons[i].enabled = (i < maxCount);
        }
    }



}