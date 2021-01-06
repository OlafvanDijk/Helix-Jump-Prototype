using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    [Tooltip("Object to Rotate")]
    [SerializeField] private GameObject ObjectToRotate;
    [Tooltip("Speed at which the level rotates")]
    [Range(0,0.3f)]
    [SerializeField] private float rotationSpeed = 0.1f;

    private Quaternion rotationY;

    /// <summary>
    /// Destroys this script if ObjectToRotate is null
    /// </summary>
    private void Awake()
    {
        if (!ObjectToRotate)
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Rotate object based on the horizontal movement of the first touch input
    /// </summary>
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                rotationY = Quaternion.Euler(0f, -touch.deltaPosition.x * rotationSpeed, 0f);
                ObjectToRotate.transform.rotation *= rotationY;
            }
        }
    }
}
