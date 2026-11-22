using UnityEngine;
using TMPro;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public float holdDistance = 2f, holdHeight = 0.5f;
    public TMP_Text prompt;      // "Naciśnij E"
    public TMP_Text message;     // komunikat po wejściu w interakcję

    PlayerController pc;
    CharacterController cc;
    Camera cam;
    Interactable current;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
        prompt.gameObject.SetActive(false);
        message.gameObject.SetActive(false);
    }

    void Update()
    {
        if (current == null)
        {
            Interactable look = pc.topDown ? null : LookingAt();
            prompt.gameObject.SetActive(look != null);

            if (look != null && Input.GetKeyDown(interactKey))
            {
                current = look;
                current.BeginInteract(cc);
                prompt.gameObject.SetActive(false);
                message.text = $"Interakcja z: {look.name}\n(trzymaj E, aby przenosić)";
                message.gameObject.SetActive(true);
            }
        }
        else if (Input.GetKey(interactKey))
        {
            Vector3 target = transform.position + Vector3.up * holdHeight
                           + cam.transform.forward * holdDistance;
            current.Follow(target);
        }
        else
        {
            current.EndInteract(cc);
            current = null;
            message.gameObject.SetActive(false);
        }
    }

    Interactable LookingAt()
    {
        // promień ze środka ekranu, zaczynający się za graczem (żeby nie trafił w jego kapsułę)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin += ray.direction * Vector3.Distance(cam.transform.position, transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.GetComponentInParent<Interactable>();
        return null;
    }
}