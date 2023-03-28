using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private int enemiesLimit = 5;
    [SerializeField] private int spawnCooldown = 5;
    [SerializeField] private Enemy prefabEnemy;
    [SerializeField] private Transform offset;
    [SerializeField] private float minYOffset = -2.0f;
    [SerializeField] private float maxYOffset = 2.0f;

    private bool isSpawning;

    // Start is called before the first frame update
    private void Start()
    {
        _ = StartCoroutine(SpawnEnemies());
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
            for (int i = 0; i < enemiesLimit; i++)
            {
                Vector3 spawnPosition = offset.position + new Vector3(0, Random.Range(minYOffset, maxYOffset), 0);
                _ = Instantiate(prefabEnemy, spawnPosition, offset.rotation);
                yield return new WaitForSeconds(spawnCooldown);
            }

            // break condition here to stop spawning enemies at some point
            // if (someCondition) break;
        }

        //isSpawning = false;
    }
}