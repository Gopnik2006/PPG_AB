using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public float followSpeed = 12f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;          
    }

    public void BeginInteract(CharacterController player)
    {
        rb.isKinematic = false;         // pkt 3: włączamy fizykę
        rb.useGravity = false;          // w trakcie trzymania nie spada
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        foreach (var c in GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(c, player, true);   // nie szarpie gracza
    }

    public void Follow(Vector3 target)  // pkt 4: podążanie za kamerą
    {
        rb.linearVelocity = Vector3.ClampMagnitude((target - rb.position) * followSpeed, 25f);
        rb.angularVelocity = Vector3.zero;
    }

    public void EndInteract(CharacterController player)
    {
        rb.useGravity = true;           // po puszczeniu E obiekt spada
        rb.linearVelocity *= 0.5f;
        foreach (var c in GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(c, player, false);
    }
}
