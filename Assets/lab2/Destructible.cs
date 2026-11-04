using System.Collections;
using UnityEngine;

public class Destructible : MonoBehaviour, IHittable
{
    public int pieces = 3;           // pieces^3 kawałków
    public float force = 6f, pieceLifetime = 4f;
    public float respawnTime = 30f;  // po ilu sekundach obiekt wraca
    bool broken;

    public void OnHit(Projectile p, Vector3 point)
    {
        if (broken) return;
        broken = true;

        Renderer r = GetComponent<Renderer>();
        Collider col = GetComponent<Collider>();
        Bounds b = r.bounds;
        Vector3 size = b.size / pieces;
        Vector3 start = b.min + size / 2f;

        for (int x = 0; x < pieces; x++)
        for (int y = 0; y < pieces; y++)
        for (int z = 0; z < pieces; z++)
        {
            var piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.position = start + Vector3.Scale(new Vector3(x, y, z), size);
            piece.transform.localScale = size;
            piece.GetComponent<Renderer>().sharedMaterial = r.sharedMaterial;
            var rb = piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(force, point, 3f, 0.2f, ForceMode.Impulse);
            Destroy(piece, pieceLifetime);
        }

        // ukrywamy obiekt zamiast go niszczyć, żeby mógł wrócić
        r.enabled = false;
        col.enabled = false;
        StartCoroutine(Respawn(r, col));
    }

    IEnumerator Respawn(Renderer r, Collider col)
    {
        yield return new WaitForSeconds(respawnTime);
        r.enabled = true;
        col.enabled = true;
        broken = false;
    }
}