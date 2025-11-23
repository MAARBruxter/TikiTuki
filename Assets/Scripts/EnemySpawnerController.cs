using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform[] spawnPoints;         // 0: AngryPig, 1: Bee, 2: Bunny
    [SerializeField] private GameObject[] enemySets;          // Prefabs de enemigos (AngryPig, Bee, Bunny)
    [SerializeField] private EnemyController bossController;  // Referencia al jefe (TikiTaka)

    [Header("Parent Folders in Hierarchy")]
    [SerializeField] private Transform angryPigParent;
    [SerializeField] private Transform beeParent;
    [SerializeField] private Transform bunnyParent;

    [Header("Spawn Timing")]
    [Tooltip("Tiempo entre la aparición de un enemigo y otro del mismo tipo.")]
    [SerializeField] private float spawnDelay = 6f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int fullBossHealth;
    private int lastKnownBossHealth;
    private bool delayShort = false;
    private Coroutine spawnLoopCoroutine; // Controls the infinite loop

    private void Start()
    {
        fullBossHealth = bossController.BossHealth;
        lastKnownBossHealth = bossController.BossHealth;
        StartNewSpawnCycle(); // Starts the spawn cycle
    }

    private void Update()
    {
        if ((bossController.BossHealth < fullBossHealth / 2) && !delayShort)
        {
            spawnDelay /= 2;
            delayShort = true;
        }

        // If the boss loses a life, destroys all the spawned enemies.
        if (bossController.BossHealth < lastKnownBossHealth)
        {
            OnBossHit();
            lastKnownBossHealth = bossController.BossHealth;
        }
    }

    /// <summary>
    /// When the boss is hit, deletes all the spawned enemies.
    /// </summary>
    private void OnBossHit()
    {
        KillAllEnemies();
        StartNewSpawnCycle();
    }

    /// <summary>
    /// Destroys all the spawned enemies.
    /// </summary>
    private void KillAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                    controller.KillEnemy();
                else
                    Destroy(enemy);
            }
        }

        activeEnemies.Clear();

        // Stops the infinite loop.
        if (spawnLoopCoroutine != null)
            StopCoroutine(spawnLoopCoroutine);
    }

    /// <summary>
    /// Starts a new cycle of infinite spawn of enemies.
    /// </summary>
    private void StartNewSpawnCycle()
    {
        int randomEnemyIndex = Random.Range(0, 3);
        GameObject enemyPrefab = enemySets[randomEnemyIndex];
        Transform spawnPoint = spawnPoints[randomEnemyIndex];

        spawnLoopCoroutine = StartCoroutine(SpawnEnemiesIndefinitely(enemyPrefab, spawnPoint));
    }

    /// <summary>
    /// Generates enemies randomly until the boss is killed.
    /// </summary>
    private IEnumerator SpawnEnemiesIndefinitely(GameObject enemyPrefab, Transform spawnPoint)
    {
        while (true)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyController ec = newEnemy.GetComponent<EnemyController>();

            // Assing the parent to the type of enemy
            if (ec != null)
            {
                switch (ec.enemyType)
                {
                    case EnemyType.AngryPig:
                        if (angryPigParent != null)
                            newEnemy.transform.SetParent(angryPigParent);
                        break;

                    case EnemyType.Bee:
                        if (beeParent != null)
                            newEnemy.transform.SetParent(beeParent);
                        break;

                    case EnemyType.Bunny:
                        if (bunnyParent != null)
                            newEnemy.transform.SetParent(bunnyParent);
                        break;
                }
            }

            activeEnemies.Add(newEnemy);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}