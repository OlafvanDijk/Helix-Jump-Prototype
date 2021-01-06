using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RemoveLayer : MonoBehaviour
{
    [Tooltip("Reference to the SpawnLayers so we can Enqueue the layers again.")]
    [SerializeField] private SpawnLayers spawnLayers;

    /// <summary>
    /// Removes the layer that triggers our hitbox
    /// Enqueues all the child objects and spawns a new layer
    /// </summary>
    /// <param name="other">Object triggering our hitbox</param>
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Layer":
                EnqueuePlatforms(other);
                break;
            case "MovingWall":
                spawnLayers.EnqueueWall(other.gameObject);
                break;
        }
    }

    /// <summary>
    /// Enqueues all platforms and removes them from their parent in case they don't get used immeadiatly
    /// </summary>
    /// <param name="other">Collider of the object we are colliding with</param>
    private void EnqueuePlatforms(Collider other)
    {
        List<Transform> unparent = new List<Transform>();

        foreach (Transform child in other.transform)
        {
            if (!child.tag.Equals("Layer"))
            {
                spawnLayers.Enqueue(child.gameObject);
                unparent.Add(child);
            }
        }

        foreach (Transform child in unparent)
        {
            child.parent = other.transform.parent;
        }

        Destroy(other.gameObject);
        spawnLayers.SpawnNewLayer();
    }
}
