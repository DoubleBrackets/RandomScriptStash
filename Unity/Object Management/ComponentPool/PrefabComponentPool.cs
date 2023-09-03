using System.Collections.Generic;
using UnityEngine;

public class PrefabComponentPool<T> where T : Component, IPoolable
{
    private readonly Stack<T> objectPool = new();

    private readonly List<T> usedObjects = new();

    public PrefabComponentPool(T prefab, uint staticPoolSize, Transform parentTransform)
    {
        for (var i = 0; i < staticPoolSize; i++)
        {
            var instance = Object.Instantiate(prefab, parentTransform);
            instance.Initialize();
            objectPool.Push(instance);
        }
    }

    public T GetFromPool()
    {
        if (objectPool.Count == 0)
        {
            Debug.LogError("Pool empty, cannot get a new instance!");
            return null;
        }

        var instance = objectPool.Pop();
        usedObjects.Add(instance);
        instance.OnRetrievedFromPool();
        return instance;
    }

    public void ReturnToPool(T instance)
    {
        var returnedHitbox = usedObjects.Find(x => x == instance);
        if (returnedHitbox != null)
        {
            instance.OnReturnedToPool();
            usedObjects.Remove(instance);
            objectPool.Push(instance);
        }
        else
        {
            Debug.LogError($"Tried to return instance {instance} that is not being used. What the heck??");
        }
    }
}