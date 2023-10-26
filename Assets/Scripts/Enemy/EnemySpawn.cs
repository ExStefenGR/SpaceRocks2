using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private float spawnCooldown = 5.0f;
    [SerializeField] private float powerUpSpawnCooldown = 60.0f;
    [SerializeField] private Enemy prefabEnemy;
    [SerializeField] private Enemy powerUpEnemyPrefab;
    [SerializeField] private Transform offset;
    private readonly int enemiesLimit = 5;
    private readonly float minYOffset = -4.5f;
    private readonly float maxYOffset = 4.5f;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerUpEnemy());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (IsPlayerActive())
            {
                for (int i = 0; i < enemiesLimit; i++)
                {
                    SpawnEnemy(prefabEnemy);
                    yield return new WaitForSeconds(spawnCooldown);
                }
            }
            else
            {
                // Delete all existing enemies
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    Destroy(enemy.gameObject);
                }
                break;
            }
        }
    }

    private IEnumerator SpawnPowerUpEnemy()
    {
        float powerUpSpawnTimer = 0;

        while (true)
        {
            if (IsPlayerActive())
            {
                powerUpSpawnTimer += Time.deltaTime;
                
                if (powerUpSpawnTimer >= powerUpSpawnCooldown)
                {
                    SpawnEnemy(powerUpEnemyPrefab);
                    powerUpSpawnTimer = 0;
                }
            }
            yield return null;  // Wait for the next frame
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab)
    {
        Vector3 spawnPosition = offset.position + new Vector3(0, Random.Range(minYOffset, maxYOffset), 0);
        Instantiate(enemyPrefab, spawnPosition, offset.rotation);
    }

    private bool IsPlayerActive()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            return player.activeSelf;
        }

        return false;
    }
}
