using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    protected override bool IsDontDestroying => true;

    public readonly Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public void AddPooling(GameObject origin, Transform parent)
    {
        if (!pools.ContainsKey(origin))
            pools.Add(origin, new List<GameObject>());

        foreach (Transform tarns in parent)
        {
            if (tarns.gameObject == origin) continue;
            
            pools[origin].Add(tarns.gameObject);
        }
    }

    public GameObject Init(GameObject origin)
    {
        if (origin == null) return null;
        
        GameObject copy;
        if (pools.ContainsKey(origin))
        {
            if (pools[origin].FindAll((x) => !x.activeSelf).Count > 0)
            {
                copy = pools[origin].Find((x) => !x.activeSelf);
                copy.SetActive(true);

                var poolObject = copy.GetComponent<IPoolObject>();
                poolObject?.Init();
                    
                return copy;
            }
        }
        else
        {
            pools.Add(origin, new List<GameObject>());
        }

        copy = Instantiate(origin);
        copy.SetActive(true);
            
        var poolObject2 = copy.GetComponent<IPoolObject>();
        poolObject2?.Init();
            
        pools[origin].Add(copy);
        return copy;
    }

    protected override void OnReset()
    {
        foreach (var obj in pools.Values.SelectMany(objs => objs))
            obj.gameObject.SetActive(false);
    }
}