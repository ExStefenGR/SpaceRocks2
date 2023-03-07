using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private int enemiesLimit = 5;
    [SerializeField] private int spawnCooldown = 5;
    [SerializeField] private bool finishedSpawning = false;
    [SerializeField] private Enemy prefabEnemy;
    [SerializeField] private Transform offset;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!finishedSpawning)
        {
            _ = StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        finishedSpawning = true;
        if (finishedSpawning)
        {
            for (int i = 0; i < enemiesLimit; i++)
            {
                _ = Instantiate(prefabEnemy, offset.position, offset.rotation);
                yield return new WaitForSeconds(spawnCooldown);
            }
        }
        yield return finishedSpawning = false;
    }
}
