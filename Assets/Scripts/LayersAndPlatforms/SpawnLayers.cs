using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SpawnLayers : MonoBehaviour
{
    [Tooltip("GameObject that will serve as the parent of the layers")]
    [SerializeField]private Transform layerParentTransform;
    [Tooltip("Reference to the ScoreManager")]
    [SerializeField] private ScoreManager scoreManager;
    [Tooltip("Reference to the AudioPlayer")]
    [SerializeField] private AudioPlayer audioPlayer;
    [Tooltip("Prefab of the layer object that wil contain the platforms")]
    [SerializeField] private GameObject layerPrefab;
    [Tooltip("List of levels")]
    [SerializeField] private List<Level> levels;

    private Dictionary<PlatformType, int> maxPlatformList = new Dictionary<PlatformType, int>();

    private Transform lastSpawnedLayer;
    private Level currentLevel;

    private int numberOfPlatforms = 9;
    private int currentLayer = 0;
    private int scorePerGap;

    private bool spawnLayers = true;

    #region Unity Methods
    /// <summary>
    /// Prepares the level at the start of the game
    /// </summary>
    private void Awake()
    {
        PrepareLevel();
    }

    /// <summary>
    /// Spawns the first layers based on the amount of layers to load
    /// </summary>
    private void Start()
    {
        SpawnFirstLayers();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the layer parent set in the editor
    /// </summary>
    /// <returns>Parent of all the Layers</returns>
    public Transform GetLayerParent()
    {
        return layerParentTransform;
    }

    /// <summary>
    /// Returns the current level
    /// </summary>
    /// <returns>Current Level that is loaded</returns>
    public Level GetCurrentLevel()
    {
        return currentLevel;
    }

    /// <summary>
    /// Enqueues the given object and deactivates it.
    /// </summary>
    /// <param name="gameObject">GameObject to enqueue</param>
    public void Enqueue(GameObject gameObject)
    {
        gameObject.SetActive(false);
        currentLevel.Enqueue(gameObject);
    }

    /// <summary>
    /// Enqueues the given wall object and deactivates it.
    /// </summary>
    /// <param name="gameObject">Moving wall GameObject to enqueue</param>
    public void EnqueueWall(GameObject gameObject)
    {
        gameObject.SetActive(false);
        currentLevel.EnqueueWall(gameObject);
    }

    /// <summary>
    /// Spawns a new Layer
    /// If the amount of total layers has been reached then it will spawn the finish
    /// Only spawns a new layer when spawnLayers is true
    /// </summary>
    public void SpawnNewLayer()
    {
        if (spawnLayers)
        {
            GameObject layer;
            Vector3 newPos = Vector3.zero;
            if (lastSpawnedLayer != null)
            {
                newPos = lastSpawnedLayer.position;
                newPos.y -= 1.5f;
            }

            if (currentLayer < currentLevel.amountOfTotalLayers)
            {
                Dictionary<PlatformType, int> availablePlatforms = new Dictionary<PlatformType, int>(maxPlatformList);
                layer = CreateLayerObject(newPos);

                
                if (currentLevel.movingWall)
                {
                    int spawnchance = Random.Range(0, 3);
                    if(spawnchance == 0)
                        SpawnWall(newPos);
                }

                for (int i = 0; i < numberOfPlatforms; i++)
                {
                    SpawnPlatform(ref availablePlatforms, i, layer.transform);
                }
                currentLayer++;
            }
            else
            {
                spawnLayers = false;
                layer = Instantiate(currentLevel.Finish, layerParentTransform);
                layer.transform.position = newPos;
            }
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Spawn the first layers
    /// </summary>
    private void SpawnFirstLayers()
    {
        for (int i = 0; i < currentLevel.amountOfLayersToLoad; i++)
        {
            SpawnNewLayer();
        }
    }

    /// <summary>
    /// Get the level index from the playerprefs.
    /// If the index is equal to or higher than the amount of levels then it will load the first level
    /// This method also sets the maxPlatformList which wil later be used as an available platform list
    /// </summary>
    private void PrepareLevel()
    {
        int levelIndex = PlayerPrefs.GetInt("currentLevel");
        if (levels.Count == 0)
        {
            Debug.Log("No levels to load");
            Destroy(this);
        }
        else if (levelIndex >= levels.Count)
        {
            levelIndex = 0;
            PlayerPrefs.SetInt("currentLevel", levelIndex);
        }

        currentLevel = levels[levelIndex];
        scorePerGap = levelIndex + 1;
        currentLevel.Init(this.transform);

        foreach (UseablePlatform useablePlatform in currentLevel.useablePlatforms)
        {
            if (!maxPlatformList.ContainsKey(useablePlatform.platformType))
            {
                maxPlatformList.Add(useablePlatform.platformType, useablePlatform.maxPerLayer);
            }
        }
    }

    /// <summary>
    /// Creates the layer GameObject that will contain all the platforms
    /// </summary>
    /// <param name="position">Position to spawn the object in.</param>
    /// <returns>Layer GameObject to serve as a parent for the platforms</returns>
    private GameObject CreateLayerObject(Vector3 position)
    {
        GameObject layer = Instantiate(layerPrefab, layerParentTransform);
        layer.transform.position = position;
        lastSpawnedLayer = layer.transform;
        return layer;
    }

    /// <summary>
    /// Spawns the next platform based on the available platforms
    /// </summary>
    /// <param name="availablePlatforms">Dictionary containing the available platforms</param>
    /// <param name="platformNumber">Number of platform in the ring. Used to calculate the rotation.</param>
    /// <param name="parent">GameObject that will serve as the parent</param>
    /// <returns></returns>
    private void SpawnPlatform(ref Dictionary<PlatformType, int> availablePlatforms, int platformNumber, Transform parent)
    {
        int index = GetPlatformIndex(availablePlatforms, platformNumber);

        if (index == -1)
            return;

        KeyValuePair<PlatformType, int> platform = availablePlatforms.ElementAt(index);
        if (platform.Value > 0)
        {
            SetPlatform(ref availablePlatforms, platform.Key, platform.Value, platformNumber, parent);
        }
        else //Failsafe if the max available has been set to 0
        {
            availablePlatforms.Remove(platform.Key);
            SpawnPlatform(ref availablePlatforms, platformNumber, parent);
        }
    }

    /// <summary>
    /// Get the platform that is next in queue and set the rotation, position and parent
    /// Also updates the availability in the availablePlatform dictionary
    /// </summary>
    /// <param name="availablePlatforms">reference to the available platforms</param>
    /// <param name="platformType">Platform type of the platform to spawn</param>
    /// <param name="platformAmount">Amount available of the given type</param>
    /// <param name="platformNumber">Number of platform in the ring. Used to calculate the rotation.</param>
    /// <param name="parent">GameObject that will serve as the parent</param>
    private void SetPlatform(ref Dictionary<PlatformType, int> availablePlatforms, PlatformType platformType, int platformAmount, int platformNumber, Transform parent)
    {
        availablePlatforms[platformType] -= 1;

        GameObject platformObject = currentLevel.GetNextInQueue(platformType);
        platformObject.transform.parent = parent;
        platformObject.transform.rotation = Quaternion.Euler(-90f, 40 * platformNumber, 0f);
        platformObject.transform.localPosition = Vector3.zero;
        platformObject.SetActive(true);

        if (platformAmount == 0)
        {
            availablePlatforms.Remove(platformType);
        }

        if (platformType.Equals(PlatformType.Gap))
        {
            UpdateScore updateScore = platformObject.GetComponent<UpdateScore>();
            updateScore.Init(scoreManager, scorePerGap, audioPlayer);
        }
    }

    /// <summary>
    /// This method makes sure that the first platform is always a good platform and that there is always one gap in the ring
    /// This does mean that available platforms has to have at least one gap
    /// </summary>
    /// <param name="availablePlatforms">List of available platforms</param>
    /// <returns>Index of the next platform</returns>
    private int GetPlatformIndex(Dictionary<PlatformType, int> availablePlatforms, int platformNumber)
    {
        if (availablePlatforms == null || availablePlatforms.Count <= 0)
        {
            Debug.Log("There are not enough platforms to make a circle");
            return -1;
        }

        int index;
        if (platformNumber == 0) //First Platform
        {
            index = availablePlatforms.Keys.ToList().IndexOf(PlatformType.Good);
        }
        else if (platformNumber == numberOfPlatforms - 1 && availablePlatforms.ContainsKey(PlatformType.Gap)) //Last Platform
        {
            index = availablePlatforms.Keys.ToList().IndexOf(PlatformType.Gap);
        }
        else
        {
            index = Random.Range(0, availablePlatforms.Count);
        }
        return index;
    }

    /// <summary>
    /// Spawns a wall from the wall queue in currentlevel
    /// The wall is not placed inside of the current layer object to avoid Enqueueing problems
    /// </summary>
    /// <param name="newPos">New Position at which the wall has to spawn</param>
    private void SpawnWall(Vector3 newPos)
    {
        GameObject wall = currentLevel.DequeueWall();
        wall.transform.parent = layerParentTransform;
        wall.transform.position = newPos;
        wall.SetActive(true);
    }
    #endregion
}
