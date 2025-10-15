using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance { get; private set; }

    public GameObject bulletPrefab;
    private Dictionary<string, Queue<Bullet>> bulletPools = new Dictionary<string, Queue<Bullet>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Bullet GetBullet(string ownerId)
    {
        if (!bulletPools.ContainsKey(ownerId))
        {
            bulletPools[ownerId] = new Queue<Bullet>();
        }

        Queue<Bullet> pool = bulletPools[ownerId];
        Bullet bullet;

        if (pool.Count > 0)
        {
            bullet = pool.Dequeue();
            bullet.gameObject.SetActive(true);
        }
        else
        {
            GameObject newBulletObj = Instantiate(bulletPrefab);
            bullet = newBulletObj.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("Bullet component not found on prefab!");
                Destroy(newBulletObj);
                return null;
            }
        }

        bullet.ownerId = ownerId; // 총알에 소유자 ID 설정
        return bullet;
    }

    public void ReturnBullet(string ownerId, Bullet bullet)
    {
        if (bullet == null) return;

        bullet.gameObject.SetActive(false);

        if (!bulletPools.ContainsKey(ownerId))
        {
            bulletPools[ownerId] = new Queue<Bullet>();
        }

        bulletPools[ownerId].Enqueue(bullet);
    }
}
