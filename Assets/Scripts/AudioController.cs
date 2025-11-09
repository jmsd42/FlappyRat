using UnityEngine;

public class AudioController : MonoBehaviour
{

    [Header("Audio Sources")]
    public AudioSource audioSourceTheme;
    public AudioSource audioSourceSFXJump;
    public AudioSource audioSourceSFXButton;
    public AudioSource audioSourceSFXPoint;

    [Header("Music Clip")]
    public AudioClip menuTheme;
    public AudioClip gameTheme;
    public AudioClip gameOverTheme;

    //chatgpt
    [Header("SFX PowerUps")]
    public AudioSource audioSourceSFXPowerUp;
    public AudioClip sfxPowerUpPickup;
    public AudioClip sfxPowerUpActivate;

    private AudioClip lastThemeBeforePowerUp;
    private bool powerUpMusicActive = false;

    private void Awake()
    {
        audioSourceTheme.clip = menuTheme;
        audioSourceTheme.Play();
    }

    private void Update()
    {
        if (GameController.instance.canPlay)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                audioSourceSFXJump.Stop();
                audioSourceSFXJump.Play();
            }
        }

    }

    public void PointSFX()
    {

        audioSourceSFXPoint.PlayOneShot(audioSourceSFXPoint.clip);

    }
    public void ButtonSFX()
    {
        audioSourceSFXButton.PlayOneShot(audioSourceSFXButton.clip);
    }

    public void GameThemeMusic()
    {

        audioSourceTheme.Stop();
        audioSourceTheme.clip = gameTheme;
        audioSourceTheme.Play();
    }

    public void GameOverMusic()
    {
        audioSourceTheme.Stop();
        audioSourceTheme.clip = gameOverTheme;
        audioSourceTheme.Play();

        //yo xd
        audioSourceSFXPowerUp.Stop();
    }

    public void StopAllSFX()
    {
        audioSourceSFXJump.Stop();
        audioSourceSFXPoint.Stop();
    }

    //chatgpt
    public void PowerUpPickupSFX()
    {
        audioSourceSFXPowerUp.PlayOneShot(sfxPowerUpPickup);
    }

    public void PowerUpActivateSFX()
    {
        audioSourceSFXPowerUp.clip = sfxPowerUpActivate;
        audioSourceSFXPowerUp.Play();
    }
    public void StopPowerUpSFX()
    {
        audioSourceSFXPowerUp.Stop();
    }

    public void StartPowerUpMusic(AudioClip altTheme = null)
    {
        if (powerUpMusicActive) return;

        powerUpMusicActive = true;
        lastThemeBeforePowerUp = audioSourceTheme.clip;

        if (altTheme != null)
        {
            audioSourceTheme.Stop();
            audioSourceTheme.clip = altTheme;
            audioSourceTheme.Play();
        }
        else
        {
            audioSourceTheme.Pause();
        }
    }

    public void EndPowerUpMusic()
    {
        if (!powerUpMusicActive) return;

        powerUpMusicActive = false;
        audioSourceTheme.Stop();
        audioSourceTheme.clip = lastThemeBeforePowerUp;
        audioSourceTheme.Play();
    }
}