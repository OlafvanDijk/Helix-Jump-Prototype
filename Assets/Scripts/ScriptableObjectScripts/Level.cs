using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level_#", menuName = "Level/Create Level", order = 1)]
public class Level : ScriptableObject
{
    [Tooltip("Amount of layers in the whole level")]
    public int amountOfTotalLayers;
    [Tooltip("Amount of layers to have loaded")]
    public int amountOfLayersToLoad;
    [Tooltip("Check if there should be a chance to spawn a walking enemy")]
    public bool movingWall;
    [Tooltip("Finish GameObject. This object is spawned after the amountOfTotalLayers has been reached")]
    public GameObject Finish;
    [Tooltip("Moving Wall GameObject.")]
    [SerializeField] private GameObject movingWallObject;
    [Tooltip("List containing the usable platforms")]
    public List<UseablePlatform> useablePlatforms;

    private PlatformPooler objectPooler;
    private Transform layerParentTransform;
    private Queue movingWallQueue;

    #region Public Methods
    /// <summary>
    /// Creates an object pool for each useable platform type
    /// This pool is then saved in a dictionary together with the platform type
    /// </summary>
    public void Init(Transform transform)
    {
        objectPooler = new PlatformPooler(useablePlatforms, this);
        objectPooler.CreateQueues();
        SetLayerParentTransform(transform);
        objectPooler.PrepareQueues(amountOfLayersToLoad, layerParentTransform);

        if (movingWall)
        {
            PrepareMovingWallQueue();
        }
    }

    /// <summary>
    /// Get the next in Queue of the given PlatformType
    /// </summary>
    /// <param name="platformType">Type of platform you wish to recieve</param>
    /// <returns>Platform GameObject of given PlatformType</returns>
    public GameObject GetNextInQueue(PlatformType platformType)
    {
        return objectPooler.Dequeue(platformType);
    }

    /// <summary>
    /// Enqueue this Gameobject
    /// </summary>
    /// <param name="gameObject">GameObject to enqueueu</param>
    public void Enqueue(GameObject gameObject)
    {
        try
        {
            Enum.TryParse(gameObject.name, out PlatformType platformType);
            objectPooler.Enqueue(platformType, gameObject);
        }
        catch (Exception e)
        {
            Debug.Log("This object is not of type: PlatformType", gameObject);
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// Enqueues the given wall
    /// </summary>
    /// <param name="wall">Wall object to enqueue</param>
    public void EnqueueWall(GameObject wall)
    {
        movingWallQueue.Enqueue(wall);
    }

    /// <summary>
    /// Returns the next available wall object
    /// </summary>
    /// <returns>next available wall object</returns>
    public GameObject DequeueWall()
    {
        return movingWallQueue.Dequeue() as GameObject;
    }

    /// <summary>
    /// Instantiates Objects
    /// PlatformPooler is not a Monobehaviour script so it can't use the Instantiate function of Unity
    /// </summary>
    /// <param name="gameObject">GameObject to Instantiate</param>
    /// <param name="parentTransform">Transform of the parent</param>
    /// <returns></returns>
    public GameObject InstantiateGameObject(GameObject gameObject, Transform parentTransform)
    {
        return Instantiate(gameObject, parentTransform);
    }

    /// <summary>
    /// Get Transform
    /// </summary>
    /// <returns></returns>
    public Transform GetLayerParentTransform()
    {
        return this.layerParentTransform;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Create parent object for layers and set the transform variable
    /// </summary>
    private void SetLayerParentTransform(Transform transform)
    {
        GameObject layersObject = new GameObject("Layers");
        layersObject.transform.parent = transform;
        layerParentTransform = layersObject.transform;
    }

    /// <summary>
    /// Creates the Moving Wall queue and fills it with walls
    /// </summary>
    private void PrepareMovingWallQueue()
    {
        movingWallQueue = new Queue();
        for (int i = 0; i < amountOfLayersToLoad; i++)
        {
            GameObject movingWall = Instantiate(movingWallObject, layerParentTransform);
            movingWallQueue.Enqueue(movingWall);
        }
    }
    #endregion
}

[Serializable]
public struct UseablePlatform 
{
    [Tooltip("Maximum amount of this type of platform per ring")]
    public int maxPerLayer;
    public GameObject platform;
    public PlatformType platformType;
}

public enum PlatformType
{
    Good,
    Bad,
    Gap
}