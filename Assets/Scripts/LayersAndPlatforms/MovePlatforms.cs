using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpawnLayers))]
public class MovePlatforms : MonoBehaviour
{
    [Tooltip("Reference to the player collision to be able to use the events")]
    [SerializeField] private PlayerCollision player;
    [Tooltip("Speed at which the layers will move")]
    [Range(0,10)]
    [SerializeField] private float fallingSpeed = 3.5f;
    [Tooltip("Tags of the layers to move.")]
    [SerializeField] private List<string> layersToMove;

    private SpawnLayers spawnLayers;
    private Transform layerParentTransform;

    private bool falling = false;
    private bool canMove = true;    

    #region Unity Methods
    /// <summary>
    /// Set layerParentTransform
    /// </summary>
    private void Awake()
    {
        spawnLayers = GetComponent<SpawnLayers>();
        layerParentTransform = spawnLayers.GetLayerParent();
    }

    /// <summary>
    /// Moves the child objects with the givens tags in the layerParentTransform
    /// </summary>
    private void FixedUpdate()
    {
        if (canMove && falling)
        {
            foreach (Transform child in layerParentTransform)
            {
                if (layersToMove.Contains(child.gameObject.tag))
                {
                    child.GetComponent<Rigidbody>().MovePosition(child.position + Vector3.up * (Time.fixedDeltaTime * fallingSpeed));
                }
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Start moving the layers up
    /// </summary>
    public void StartFalling()
    {
        falling = true;
    }

    /// <summary>
    /// Stop the layers from moving up
    /// </summary>
    public void StopFalling()
    {
        falling = false;
    }

    /// <summary>
    /// Set canMove to given value. if canMove is false then also call StopFalling
    /// </summary>
    /// <param name="canMove">Defines if the platforms are allowed to move</param>
    public void CanMove(bool canMove)
    {
        this.canMove = canMove;
        if (!this.canMove)
        {
            StopFalling();
        }
    }
    #endregion
}
