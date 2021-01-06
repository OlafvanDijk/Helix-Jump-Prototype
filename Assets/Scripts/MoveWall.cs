using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    [Tooltip("Speed at which the wall should rotate")]
    [Range(0,20)]
    [SerializeField] private float rotationSpeed = 5f;

    private Transform wallTransform;
    private bool leftOrRight;

    /// <summary>
    /// Get transform
    /// </summary>
    private void Awake()
    {
        wallTransform = transform;
    }

    /// <summary>
    /// Rotates the wall to the left or right depending on leftOrRight
    /// </summary>
    private void Update()
    {
        float rotateBy = 0;
        if (leftOrRight)
        {
            rotateBy -= Time.deltaTime * rotationSpeed * 10;
        }
        else
        {
            rotateBy = Time.deltaTime * rotationSpeed * 10;
        }
        wallTransform.Rotate(new Vector3(0f, rotateBy, 0f));
    }

    /// <summary>
    /// Set the rotation and the direction to rotate towards
    /// </summary>
    private void OnEnable()
    {
        float newYRot = Random.Range(40f, 320f);
        wallTransform.rotation = Quaternion.Euler(0f, newYRot, 0f);
        System.Random rand = new System.Random();
        leftOrRight = rand.NextDouble() >= 0.5;
    }
}
