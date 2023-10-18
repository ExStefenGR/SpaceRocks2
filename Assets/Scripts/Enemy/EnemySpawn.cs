using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private readonly int enemiesLimit = 5;
    [SerializeField] private float spawnCooldown = 5.0f;
    [SerializeField] private Enemy prefabEnemy;
    [SerializeField] private Transform offset;
    [SerializeField] private float minYOffset = -2.0f;
    [SerializeField] private float maxYOffset = 2.0f;

    private bool isSpawning;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        if (isSpawning)
        {
            yield break;
        }

        isSpawning = true;

        while (true)
        {
            if (IsPlayerActive()) // Check if the player is active
            {
                for (int i = 0; i < enemiesLimit; i++)
                {
                    Vector3 spawnPosition = offset.position + new Vector3(0, Random.Range(minYOffset, maxYOffset), 0);
                    Instantiate(prefabEnemy, spawnPosition, offset.rotation);
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

                // Stop spawning enemies
                break;
            }
        }

        isSpawning = false;
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
