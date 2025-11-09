using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 0;
    public float travelTime = 0.2f;
    public Rigidbody2D rigidbody2D;
    public float rotationSpeedUp = 0.2f;
    public float rotationSpeedDown = 0.4f;

    public PlayerConfiguration[] playerConfiguration;
    public SpriteRenderer spriteRenderer;

    public Animator animator;

    // --- NUEVAS VARIABLES ---
    private int powerUpCount = 0;
    private int maxPowerUps = 3;
    private bool powerUpActive = false;
    private float powerUpDuration = 5f;
    private float powerUpTimer = 0f;

    [Header("Referencias")]
    public PipeController pipeController; // arrástralo desde el inspector

    private float originalPipeSpeed;
    private float originalMaxTime;

    private void Update()
    {
        if (GameController.instance.canPlay)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetVelocity();
                LeanTween.cancel(gameObject);
                LeanTween.moveY(gameObject, transform.position.y + jumpForce, travelTime).setOnComplete(Rotate);
                //Debug.Log("viva el salto");

                LeanTween.rotateZ(gameObject, 20, rotationSpeedUp);
            }

            UpdatePowerUp();
        }
    }

    public void Rotate()
    {
        LeanTween.rotateZ(gameObject, - 40, rotationSpeedDown);
    }


    public void ToggleRigidBody()
    {
        if (rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            ResetVelocity();
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            ResetVelocity();
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void ResetVelocity()
    {
        rigidbody2D.linearVelocity = new Vector2(0, 0);
        rigidbody2D.angularVelocity = 0f;
    }
    public void SelectCharacter(int character)
    {
        switch (character)
        {
            case 0:
                spriteRenderer.sprite = playerConfiguration[0].sprite;
                animator.runtimeAnimatorController = playerConfiguration[0].animatorController;
                break;
            case 1:
                spriteRenderer.sprite = playerConfiguration[1].sprite;
                animator.runtimeAnimatorController = playerConfiguration[1].animatorController;
                break;
            case 2:
                spriteRenderer.sprite = playerConfiguration[2].sprite;
                animator.runtimeAnimatorController = playerConfiguration[2].animatorController;
                break;
            default:
                break;
        }
    }

    //metodos de chatgpt todos estos
    public void CollectPowerUp()
    {
        if (powerUpCount < maxPowerUps)
        {
            powerUpCount++;
            Debug.Log("PowerUp recogido! Total: " + powerUpCount);

            //chatgpt 2
            AudioController audioCtrl = FindFirstObjectByType<AudioController>();
            if (audioCtrl != null) audioCtrl.PowerUpPickupSFX();

            if (GameController.instance != null)
                GameController.instance.UpdatePowerUpUI(powerUpCount, maxPowerUps);
        }
    }

    private void UpdatePowerUp()
    {
        if (powerUpActive)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                DeactivatePowerUp();
            }
        }

        // Activar con tecla E (puedes cambiarla)
        if (Input.GetKeyDown(KeyCode.E) && !powerUpActive && powerUpCount > 0)
        {
            ActivatePowerUp();
        }
    }

    private void ActivatePowerUp()
    {
        powerUpCount--;
        powerUpActive = true;
        powerUpTimer = powerUpDuration;

        //BORRAR SI UI DE POWERUPS NO SIRVE
        if (GameController.instance != null)
            GameController.instance.UpdatePowerUpUI(powerUpCount, maxPowerUps);

        // Guardamos velocidad original
        originalPipeSpeed = pipeController.pipeSpeed;

        pipeController.AdjustPipeSpeed(originalPipeSpeed * 2.5f);

        // Duplicamos los puntos
        ScoreManager.instance.pointMultiplier = 2;

        Debug.Log($"PowerUp ACTIVADO! Restantes: {powerUpCount}");

        //chatgpt 2
        AudioController audioCtrl = FindFirstObjectByType<AudioController>();
        if (audioCtrl != null)
        {
            audioCtrl.PowerUpActivateSFX();
            audioCtrl.StartPowerUpMusic(); // o pasa un clip alternativo: audioCtrl.StartPowerUpMusic(audioCtrl.powerUpTheme);
        }
    }

    private void DeactivatePowerUp()
    {
        powerUpActive = false;

        // Restauramos velocidad original
        pipeController.AdjustPipeSpeed(originalPipeSpeed);

        // Restauramos multiplicador de puntos
        ScoreManager.instance.pointMultiplier = 1;

        Debug.Log("PowerUp TERMINADO");

        //chatgpt 2
        AudioController audioCtrl = FindFirstObjectByType<AudioController>();
        if (audioCtrl != null)
        {
            audioCtrl.StopPowerUpSFX();   // Detiene el sonido si aún está reproduciéndose
            audioCtrl.EndPowerUpMusic();  // Restaura la música normal
        }
    }

    public bool CanCollectMorePowerUps()
    {
        return powerUpCount < maxPowerUps;
    }
}