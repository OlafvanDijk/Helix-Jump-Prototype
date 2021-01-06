using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPooler
{
    private Dictionary<PlatformType, Queue> platformPools;
    private List<UseablePlatform> useablePlatforms;
    private Level level;

    public PlatformPooler(List<UseablePlatform> useablePlatforms, Level level)
    {
        this.platformPools = new Dictionary<PlatformType, Queue>();
        this.useablePlatforms = useablePlatforms;
        this.level = level;
    }

    /// <summary>
    /// Create Queues so we can add object to them at a later stage
    /// </summary>
    public void CreateQueues()
    {
        if (useablePlatforms.Count > 0)
        {
            foreach (UseablePlatform useablePlatform in useablePlatforms)
            {
                PlatformType platformType = useablePlatform.platformType;

                if (!platformPools.ContainsKey(platformType))
                {
                    platformPools.Add(useablePlatform.platformType, new Queue());
                }
            }
        }
    }

    /// <summary>
    /// Call FillQueue for each object pool in the platformPool dictionary
    /// </summary>
    /// <param name="amountOfLayersToLoad">Amount of layers to have loaded</param>
    /// <param name="parentTransform">Transform of the parent object</param>
    public void PrepareQueues(int amountOfLayersToLoad, Transform parentTransform)
    {
        foreach (UseablePlatform useablePlatform in useablePlatforms)
        {
            PlatformType platformType = useablePlatform.platformType;
            Queue queue = platformPools[platformType];
            FillQueue(ref queue, useablePlatform.platform, useablePlatform.maxPerLayer, amountOfLayersToLoad, parentTransform, useablePlatform.platformType);
            platformPools[platformType] = queue;
        }
    }

    /// <summary>
    /// Enqueues the given object
    /// </summary>
    /// <param name="platformType"></param>
    /// <param name="gameObject"></param>
    public void Enqueue(PlatformType platformType, GameObject gameObject)
    {
        platformPools[platformType].Enqueue(gameObject);
    }

    /// <summary>
    /// Get next available object of type
    /// </summary>
    /// <param name="platformType"></param>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public GameObject Dequeue(PlatformType platformType)
    {
        return platformPools[platformType].Dequeue() as GameObject;
    }

    /// <summary>
    /// Fills the given queue with an instantiated GameObject
    /// </summary>
    /// <param name="queue">Queue to fill</param>
    /// <param name="gameObject">gameObject to be instantiated</param>
    /// <param name="maxPerLayer">Max amount per ring</param>
    /// <param name="parentTransform">Transform of the parent object</param>
    private void FillQueue(ref Queue queue, GameObject gameObject, int maxPerLayer, int amountOfLayersToLoad, Transform parentTransform, PlatformType platformType)
    {
        int amountOfPlatforms = amountOfLayersToLoad * maxPerLayer;
        for (int i = 0; i < amountOfPlatforms; i++)
        {
            GameObject platformObject = level.InstantiateGameObject(gameObject, parentTransform);
            platformObject.name = platformType.ToString();
            queue.Enqueue(platformObject);
        }
    }
}
