using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    color,
    transparent
}

public class PlayerHealth : MonoBehaviour
{
    [Header("Fruits Settings")]
    [Tooltip("Number of fruits collected by the player.")]
    [SerializeField] private int fruitCount;

    [Header("Fall limit")]
    [SerializeField] private float negativeYLimit = -10f;

    private bool canTakeDamage = true;

    public int FruitCount { get => fruitCount; set => fruitCount = value; }
    public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

    [Header("Blink Damage")]
    [Tooltip("Duration of each blink when the player takes damage.")]
    [SerializeField] private float blinkDuration = 0.2f;

    [Tooltip("Color used for the blink effect when the player takes damage.")]
    [SerializeField] private Color blinkColor = Color.red;

    [Header("Fruit Drop Settings")]
    [Tooltip("Maximum number of fruits to drop when taking damage.")]
    [Range(1, 20)]
    [SerializeField] private int maxFruitCount;

    //Components
    private Animator animator;
    public PlayerState blinkState;

    //Components references
    private SpriteRenderer spriteRenderer;

    private PlayerMovementNewInputSystem playerMovement;
    private EnemyController enemyController;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMovement = GameObject.FindGameObjectWithTag(GameConstants.PLAYERTAG_KEY).GetComponent<PlayerMovementNewInputSystem>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (transform.position.y < negativeYLimit)
        {
            LevelManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// When the player takes damage, if he has fruits, he blinks and loses all fruits. If he has no fruits, the game ends.
    /// The damage sound is played when the player takes damage.
    /// </summary>

    [ContextMenu("Take Damage")]
    public void TakeDamage()
    {
        if (canTakeDamage)
        {
            AudioManager.Instance?.PlayDamageSound();
            if (fruitCount == 0)
            {
                LevelManager.Instance.GameOver();
            }
            else
            {
                StartCoroutine(VibrateGamePad());
                animator.SetTrigger(GameConstants.TRIGGERHIT_KEY);
                playerMovement.AccelerationTimer = 0f;
                StartCoroutine(BlinkEffect(6));

            }
        }
    }

    /// <summary>
    /// When the player takes damage, he drops all the fruits that were collected,
    /// scattering them in the direction the player was pushed.
    /// </summary>
    /// <param name="direction">Direction in which the player was pushed (-1 for left, 1 for right).</param>
    public void DropFruits(float direction)
    {
        for (int i = 0; i < fruitCount && i <= maxFruitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1f, 0f);
            GameObject droppedFruit = Instantiate(LevelManager.Instance.fallingApplePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = droppedFruit.GetComponent<Rigidbody2D>();

            // Spreads the apples from the directions hit.
            float forceX = UnityEngine.Random.Range(8f, 10f) * direction;
            float forceY = UnityEngine.Random.Range(4f, 8f);

            rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
        }

        // Empty the fruit counter.
        fruitCount = 0;
    }


    /// <summary>
    /// Repeats blinksTimes a blink every blinckDuration seconds
    /// </summary>
    /// <param name="blinks"></param>
    /// <returns></returns>
    private IEnumerator BlinkEffect(int blinks)
    {
        canTakeDamage = false;
        if (blinkState == PlayerState.color)
        {
            blinkColor = Color.red;
        }
        else if (blinkState == PlayerState.transparent)
        {
            blinkColor = new Color(1f, 1f, 1f, 0.2f); // Transparent white
        }

        do
        {
            // Make the sprite transparent
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            // Restore the original color
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);

            blinks--;
        } while (blinks > 0);

        canTakeDamage = true;
    }

    /// <summary>
    /// Initiates a vibration on the current gamepad for a short duration.
    /// </summary>
    /// <remarks>This coroutine checks if a gamepad is connected and, if so, vibrates it for 0.25 seconds. The
    /// vibration is applied equally to both motors of the gamepad.</remarks>

    [ContextMenu("Vibrate GamePad")]
    private IEnumerator VibrateGamePad()
    {
        if (Gamepad.current != null)
        {
            // Vibrate the gamepad
            Gamepad.current.SetMotorSpeeds(0.5f, 0.5f);
            yield return new WaitForSeconds(0.1f); // Vibrate for 0.2 seconds
            Gamepad.current.SetMotorSpeeds(0f, 0f); // Stop vibration
        }
    }
}
