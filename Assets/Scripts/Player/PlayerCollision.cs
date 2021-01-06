using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(Animator))]
public class PlayerCollision : MonoBehaviour
{
    [Range(0,5)]
    [SerializeField] private float bounceMagnitude = 3.5f;
    [Tooltip("Layers of interactable objects")]
    [SerializeField] private LayerMask layerMask;

    [Header("Events")]
    public UnityEvent Gap;
    public UnityEvent Bounce;
    public UnityEvent GameOver;
    public UnityEvent Finish;

    private Rigidbody rb;
    private Animator animator;

    private bool bouncing = false;
    private bool canMove = true;

    /// <summary>
    /// Get Components
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Performs the Bounce if bouncing is true
    /// </summary>
    private void FixedUpdate()
    {
        if (canMove && bouncing)
        {
            Bounce.Invoke();
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * bounceMagnitude, ForceMode.Impulse);
            bouncing = false;
        }
    }

    /// <summary>
    /// Check tag to see what we are colliding with
    /// </summary>
    /// <param name="collision">Other object we are colliding with</param>
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.tag)
        {
            case "Good Platform":
                //Stop moving
                Debug.Log("Bounce");
                bouncing = true;
                break;
            case "Bad Platform":
                //Dead animation and trigger game over
                Debug.Log("Dead");
                Dead();
                GameOver.Invoke();
                break;
            case "Finish":
                //Trigger Finish
                Debug.Log("Finished");
                Finish.Invoke();
                StopMoving();
                break;
        }
    }

    /// <summary>
    /// Make sure the player doesn't move anymore and can be animated;
    /// </summary>
    private void Dead()
    {
        StopMoving();
        animator.SetTrigger("GameOver");
    }

    /// <summary>
    /// Stops the player from being able to move
    /// and allows an animator to take over the position of the rigidbody
    /// </summary>
    private void StopMoving()
    {
        canMove = false;
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}