using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private List<BulletBehavior> bulletPool;

    public static BulletPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        bulletPool = new List<BulletBehavior>(initialPoolSize);
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            ExpandPool();
        }
    }

    private BulletBehavior ExpandPool()
    {
        BulletBehavior newBullet = Instantiate(bulletPrefab, transform);
        newBullet.gameObject.SetActive(false);
        bulletPool.Add(newBullet);
        return newBullet;
    }

    public BulletBehavior GetBullet()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                return bullet;
            }
        }

        return ExpandPool();
    }
}