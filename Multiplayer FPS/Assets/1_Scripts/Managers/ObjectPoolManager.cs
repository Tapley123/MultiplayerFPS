using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum QueueType
{
    BulletImpact, BulletTrail
};

public class ObjectPoolManager : MonoBehaviour
{
    #region Singleton
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<ObjectPoolManager>(); } return _instance; } }
    #endregion

    [Expandable] public ObjectPoolSettings poolSetting;

    public Queue<GameObject> bulletImpactQueue = new Queue<GameObject>();
    public Transform bulletImpactHolder;

    public Queue<GameObject> bulletTrailQueue = new Queue<GameObject>();
    public Transform bulletTrailHolder;

    public void ReturnToPool(QueueType type, GameObject pooledObj)
    {
        pooledObj.SetActive(false);

        switch (type)
        {
            //Hit Enemy
            case QueueType.BulletImpact:
                //Debug.Log("Return an enemy impact");
                bulletImpactQueue.Enqueue(pooledObj);
                break;

            //Bullet Trail
            case QueueType.BulletTrail:
                //Debug.Log("Return Bullet Trail");
                bulletTrailQueue.Enqueue(pooledObj);
                break;
        }
    }

    public IEnumerator ReturnToPoolWithDelay(QueueType type, GameObject pooledObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(type, pooledObj);
    }

    public GameObject TakeFromPool(QueueType type, GameObject prefab, Transform holder)
    {
        //Grabbing game object from the queue if ther is one or instansiating one if need be
        GameObject current = null;

        switch (type)
        {
            case QueueType.BulletImpact:
                //if there is an object available in the pool
                if (bulletImpactQueue.Count > 0)
                {
                    //Get object from pool
                    current = bulletImpactQueue.Dequeue();
                    //turn on the got object
                    current.SetActive(true);
                }
                //there arent any available objects in the pool
                else
                {
                    //spawn a new object for the pool
                    current = Instantiate(prefab, holder);
                }
                break;

            case QueueType.BulletTrail:
                //if there is an object available in the pool
                if (bulletTrailQueue.Count > 0)
                {
                    //Get object from pool
                    current = bulletTrailQueue.Dequeue();
                    //turn on the got object
                    current.SetActive(true);
                }
                //there arent any available objects in the pool
                else
                {
                    //spawn a new object for the pool
                    current = Instantiate(prefab, holder);
                }
                break;
        }

        return current;
    }
}
