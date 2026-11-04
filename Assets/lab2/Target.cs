using System.Collections;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour, IHittable
{
    public TMP_Text message;
    public float showTime = 1.5f;
    int hits;
    Coroutine co;

    void Start() { if (message) message.gameObject.SetActive(false); }

    public void OnHit(Projectile p, Vector3 point)
    {
        hits++;
        message.text = $"Trafiono w {gameObject.name}! Trafienia: {hits}";
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        message.gameObject.SetActive(true);
        yield return new WaitForSeconds(showTime);
        message.gameObject.SetActive(false);
    }
}