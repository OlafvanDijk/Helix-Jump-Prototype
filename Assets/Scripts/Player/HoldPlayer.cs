using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCollision), typeof(Rigidbody))]
public class HoldPlayer : MonoBehaviour
{
    [Tooltip("Lowest point that the player may go. Reaching this point will invoke the Gap Event")]
    [SerializeField] private float minY;

    private Rigidbody rb;
    private PlayerCollision playerCollision;

    /// <summary>
    /// Get Components
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollision = GetComponent<PlayerCollision>();
    }

    /// <summary>
    /// Keeps the player from going below a certain point
    /// If the player reaches this point playerCollision.Gap will be invoked which will then start moving the layers up
    /// </summary>
    private void FixedUpdate()
    {
        if (rb.position.y < minY)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            playerCollision.Gap.Invoke();
        }
        else
        {
            rb.useGravity = true;
        }
    }
}
