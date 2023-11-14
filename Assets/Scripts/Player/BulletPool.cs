using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private BulletBehavior bigBulletPrefab;
    [SerializeField] private int initialPoolSize = 10;
    private List<BulletBehavior> regularBulletPool;
    private List<BulletBehavior> bigBulletPool;

    public static BulletPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        regularBulletPool = new List<BulletBehavior>(initialPoolSize);
        bigBulletPool = new List<BulletBehavior>(initialPoolSize);
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            ExpandPool();
        }
    }

    private void ExpandPool()
    {
        BulletBehavior newRegularBullet = Instantiate(bulletPrefab, transform);
        BulletBehavior newBigBullet = Instantiate(bigBulletPrefab, transform);
        newRegularBullet.gameObject.SetActive(false);
        newBigBullet.gameObject.SetActive(false);
        regularBulletPool.Add(newRegularBullet);
        bigBulletPool.Add(newBigBullet);
    }

    public BulletBehavior GetRegularBullet()
    {
        foreach (BulletBehavior bullet in regularBulletPool)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                return bullet;
            }
        }
        ExpandPool();
        return regularBulletPool[regularBulletPool.Count - 1];
    }

    public BulletBehavior GetBigBullet()
    {
        foreach (BulletBehavior bullet in bigBulletPool)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                return bullet;
            }
        }
        ExpandPool();
        return bigBulletPool[bigBulletPool.Count - 1];
    }
}
