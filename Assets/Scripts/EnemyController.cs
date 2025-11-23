using System;
using System.Collections;
using UnityEngine;

public enum EnemyType
{
    AngryPig,
    Bee,
    Bunny,
    TikiTaka
}
public class EnemyController : MonoBehaviour
{
    [SerializeField] private static int bossHealth = 5;

    /// <summary>
    /// Resets the boss health for ACT III.
    /// </summary>
    public static void ResetBossHealth()
    {
        bossHealth = 5;
    }

    [Header("Enemy Components")]
    [Tooltip("Indicates the type of enemy and its moveset.")]
    public EnemyType enemyType;
    private Transform groundDetection;
    private SpriteRenderer spriteRenderer;

    //Ground Check
    [Tooltip("Layer mask for ground detection.")]
    private LayerMask groundLayer;
    private LayerMask enemyLimitsLayer;


    [Header("Enemy movement")]
    private Vector2 direction;

    [Tooltip("Determines the speed of the enemy.")]
    [Range(0.5f, 4f)]
    [SerializeField] private float movespeed = 2f;
    [Tooltip("Indicates the lenght of the line detection for the ground.")]
    [SerializeField] private float groundDetectionLine = 1.5f;
    [Tooltip("Indicates the lenght of the line detection for the walls.")]
    [SerializeField] private float wallDetectionLine = 0.5f;
    private Animator animator;

    [Header("Bee Manager (And TikiTaka)")]
    [Tooltip("Indicates how much amplitude the enemy will take.")]
    [SerializeField] private float amplitude = 0.5f;
    [Tooltip("Indicates how much time will take for the enemy to change direction.")]
    [SerializeField] private float frequency = 2f;
    private Vector3 startPos;

    [Header("Bunny Manager")]
    [Tooltip ("Indicates the enemyFeet Object in the enemy.")]
    [SerializeField] private GameObject enemyFeet;
    [Tooltip("Indicates the force of the jump.")]
    [SerializeField] private float jumpForce = 5f;
    [Tooltip("Indicates the time that each jump will take.")]
    [SerializeField] private float timeBetweenJumps = 2f;
    private float jumpTimer = 0f;
    private Rigidbody2D rb;

    [Header("TikiTaka Manager")]
    [SerializeField] private int tpPosition = 5;
    [Tooltip("Color used for the blink effect when the player takes damage.")]
    [SerializeField] private Color blinkColor = Color.red;
    [Tooltip("Duration of each blink when the player takes damage.")]
    [SerializeField] private float blinkDuration = 0.2f;
    [Tooltip("Indicates how many fruits will the boss spread per hit.")]
    [SerializeField] private int fruitCount = 10;
    [Tooltip("Indicates the trophy to win the level.")]
    [SerializeField] private GameObject trophy;
    private bool canTakeDamage = true;
    private bool canFlipEnemy = true;



    [Header("Player Components")]
    private PlayerMovementNewInputSystem playerMovement;
    private PlayerHealth playerHealth;

    public int BossHealth { get => bossHealth; set => bossHealth = value; }

    private void Awake()
    {
        if (!(enemyType == EnemyType.TikiTaka))
        {
            groundDetection = transform.GetChild(1);
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        groundLayer = LayerMask.GetMask(GameConstants.GROUNDMASK_KEY);
        enemyLimitsLayer = LayerMask.GetMask(GameConstants.ENEMYLIMITS_KEY);
        direction = Vector2.left;
        playerMovement = GameObject.FindGameObjectWithTag(GameConstants.PLAYERTAG_KEY).GetComponent<PlayerMovementNewInputSystem>();
        playerHealth = GameObject.FindGameObjectWithTag(GameConstants.PLAYERTAG_KEY).GetComponent<PlayerHealth>();
        animator = GetComponentInChildren<Animator>();
        // Only use for the Bonnie Enemy
        if (enemyType == EnemyType.Bunny)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                Debug.LogError($"[EnemyController] El enemigo tipo Bunny requiere un Rigidbody2D en {gameObject.name}.");

            if (enemyFeet == null)
                Debug.LogError($"[EnemyController] El enemigo tipo Bunny requiere un objeto asignado en 'enemyFeet' en {gameObject.name}.");
        }
    }

    void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        EnemyMove();
     
    }

    /// <summary>
    /// Depending on the enemy type, does a different movement.
    /// </summary>
    private void EnemyMove()
    {
        switch (enemyType)
        {
            case EnemyType.AngryPig:
                AngryPigMove();
                break;
            case EnemyType.Bee:
                BeeMove();
                break;
            case EnemyType.Bunny:
                BunnyMove();
                break;
            case EnemyType.TikiTaka:
                TikiTakaMove();
                break;
        }
    }

    /// <summary>
    /// Controls the way the AngryPig moves.
    /// </summary>
    void AngryPigMove()
    {
        FlipEnemy();

        //Move the crab
        transform.position += (Vector3)direction * movespeed * Time.deltaTime;

    }

    /// <summary>
    /// Controls the way the bee moves.
    /// </summary>
    void BeeMove()
    {
        // Vertical movement
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        FlipEnemy();

        // Horizontal movement
        transform.position += (Vector3)direction * movespeed * Time.deltaTime;
    }

    /// <summary>
    /// Controls the way th bunny moves.
    /// </summary>
    private void BunnyMove()
    {

        FlipEnemy();

        // Horizontal Movement
        transform.position += (Vector3)direction * movespeed * Time.deltaTime;

        // Jump Controller
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= timeBetweenJumps && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpTimer = 0f;
            animator.SetTrigger(GameConstants.TRIGGERJUMP_KEY); // opcional, si tienes animación
        }
    }

    private void FlipEnemy()
    {
        //if ground below is not deteced or the wall is detected, change direction.
        if (!(GameObject.Find(GameConstants.BOSSNAME_KEY)) && !(GeneralDetection(groundDetectionLine, Vector3.down, Color.yellow)) || (GeneralDetection(wallDetectionLine, direction, Color.green)))
        {
            direction = -direction; //same as direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            //Invert the GroundDetection X point
            groundDetection.localPosition = new Vector3(-groundDetection.localPosition.x, groundDetection.localPosition.y, groundDetection.localPosition.z);
        } else if (GameObject.Find(GameConstants.BOSSNAME_KEY) && canFlipEnemy && BossHealth % 2 == 0)
        {
            direction = -direction; //same as direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            //Invert the GroundDetection X point
            groundDetection.localPosition = new Vector3(-groundDetection.localPosition.x, groundDetection.localPosition.y, groundDetection.localPosition.z);
            canFlipEnemy = false;
        }
    }

    private void TikiTakaMove()
    {
        // Movimiento vertical con una onda senoidal
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(enemyFeet.transform.position, Vector2.down, 0.25f, groundLayer);
    }

    /// <summary>
    /// This detects if thew enemy is touching any ground collision.
    /// </summary>
    /// <param name="rayLenght"></param>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool GeneralDetection(float rayLenght, Vector3 direction, Color color)
    {
        Debug.DrawRay(groundDetection.position, direction * rayLenght, color);
        return (Physics2D.Raycast(groundDetection.position, direction, rayLenght, groundLayer) || Physics2D.Raycast(groundDetection.position, direction, rayLenght, enemyLimitsLayer));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag(GameConstants.PLAYERTAG_KEY))
        {
            if (playerMovement.IsSpining)
            {
                if (canTakeDamage)
                {
                    AudioManager.Instance?.PlayDamageSound();
                    if (enemyType == EnemyType.TikiTaka && bossHealth > 0)
                    {
                        DropFruitsBoss();
                        playerMovement.ApplyKnockback(transform.position, true);
                        StartCoroutine(BlinkEffect(6));
                        if (enemyType == EnemyType.TikiTaka)
                        {
                            bossHealth--;
                        }
                        if (transform.localScale.x > 0)
                        {
                            // Si mira a la derecha → mover 10 unidades a la derecha
                            transform.position = new Vector3(transform.position.x - tpPosition, transform.position.y, transform.position.z);
                        }
                        else
                        {
                            // Si mira a la izquierda → mover 10 unidades a la izquierda
                            transform.position = new Vector3(transform.position.x + tpPosition, transform.position.y, transform.position.z);
                        }
                        Vector3 scale = transform.localScale;
                        scale.x *= -1;
                        transform.localScale = scale;
                    }
                    else
                    {
                        if (enemyType == EnemyType.TikiTaka)
                        {
                            bossHealth--;
                        }
                        KillEnemy();
                    }
                }
                
            } else if (playerHealth.CanTakeDamage) {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
                playerMovement.ApplyKnockback(transform.position, false);
            }
                
        }
    }

    public void KillEnemy()
    {
        if (enemyType == EnemyType.TikiTaka)
        {
            trophy.SetActive(true);   
        }

        Destroy(GetComponent<BoxCollider2D>());
        animator.SetBool(GameConstants.ENEMYDEADBOOL_KEY, true);
        Destroy(gameObject, 0.3f);
    }

    /// <summary>
    /// When the player takes damage, he drops all the fruits that were collected,
    /// scattering them in the direction the player was pushed.
    /// </summary>
    /// <param name="direction">Direction in which the player was pushed (-1 for left, 1 for right).</param>
    private void DropFruitsBoss()
    {
        for (int i = 0; i <= fruitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 1f, 0f);
            GameObject droppedFruit = Instantiate(LevelManager.Instance.fallingApplePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = droppedFruit.GetComponent<Rigidbody2D>();

            // Lanza las frutas principalmente hacia la dirección del golpe
            float forceX = UnityEngine.Random.Range(8f, 10f) * 1;
            if (BossHealth % 2 != 0)
            {
                forceX = UnityEngine.Random.Range(8f, 10f) * -1;
            }
            
            float forceY = UnityEngine.Random.Range(4f, 8f);

            rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Repeats blinksTimes a blink every blinckDuration seconds
    /// </summary>
    /// <param name="blinks"></param>
    /// <returns></returns>
    private IEnumerator BlinkEffect(int blinks)
    {
        canTakeDamage = false;
        blinkColor = Color.red;

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

}
