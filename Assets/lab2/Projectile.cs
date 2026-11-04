using UnityEngine;

public interface IHittable { void OnHit(Projectile p, Vector3 point); }

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public bool sticky;                 // typ 2: zostaje po uderzeniu
    public float stickyLifetime = 5f;   // znika 5 s po uderzeniu
    public float maxLifetime = 10f;     // gdy nic nie trafi
    public GameObject hitEffect;
    public AudioClip hitSound;

    Rigidbody rb;
    bool hitDone, launched;
    float dieAt;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 dir, Collider ignore)
    {
        launched = true;
        var col = GetComponent<Collider>();
        if (ignore && col) Physics.IgnoreCollision(col, ignore);
        rb.linearVelocity = dir * speed;   // w starszym Unity: rb.velocity
        dieAt = Time.time + maxLifetime;
    }

    void Start()
    {
        // zabezpieczenie, gdyby Launch się nie wykonał
        if (!launched)
        {
            rb.linearVelocity = transform.forward * speed;
            dieAt = Time.time + maxLifetime;
        }
    }

    void Update()
    {
        if (dieAt > 0 && Time.time >= dieAt) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
        if (hitDone) return;
        hitDone = true;
        Vector3 point = c.GetContact(0).point;

        foreach (var h in c.collider.GetComponentsInParent<IHittable>())
            h.OnHit(this, point);

        if (hitEffect) Instantiate(hitEffect, point, Quaternion.identity);
        if (hitSound) AudioSource.PlayClipAtPoint(hitSound, point);

        if (sticky)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            dieAt = Time.time + stickyLifetime;
        }
        else Destroy(gameObject);
    }
}