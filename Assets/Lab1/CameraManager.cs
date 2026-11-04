using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public PlayerController player;
    public LayerMask groundMask = ~0;

    [Header("Third person")]
    public float distance = 5f, minDist = 2f, maxDist = 12f;
    public float height = 1.6f, mouseSens = 2f;

    [Header("Top-down")]
    public float topDistance = 20f, minTop = 8f, maxTop = 40f;
    public float topMoveSpeed = 15f;
    public float topAngle = 60f;
    public float minPitch = -60f, maxPitch = 70f;
    public float zoomSpeed = 20f;   // ScrollWheel daje ~0.1 na "ząbek"

    [Header("Aim")]
    public float aimDistance = 1.3f;      // odległość za głową
    public float shoulderOffset = 0.6f;   // przesunięcie w prawo
    public float aimHeight = 0.8f;        // wysokość kamery przy celowaniu (okolice głowy)
    public float aimFov = 50f, aimLerp = 8f;
    float aimBlend, baseFov;

    Camera cam;
    bool topDown;
    float pitch = 15f;
    Vector3 focus;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetMode(false);
        baseFov = cam.fieldOfView;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SetMode(!topDown);

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (!topDown)
        {
            distance = Mathf.Clamp(distance - scroll, minDist, maxDist);
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSens, minPitch, maxPitch);
        }
        else
        {
            topDistance = Mathf.Clamp(topDistance - scroll, minTop, maxTop);

            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            focus += dir.normalized * topMoveSpeed * Time.deltaTime;

            // pkt 8: PPM wybiera punkt
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask))
                    player.SetTarget(hit.point);
            }
        }
    }

    void LateUpdate()
    {
        aimBlend = Mathf.Lerp(aimBlend, player.aiming ? 1f : 0f, aimLerp * Time.deltaTime);
        cam.fieldOfView = Mathf.Lerp(baseFov, aimFov, aimBlend);

        if (!topDown)
        {
            Quaternion rot = Quaternion.Euler(pitch, player.transform.eulerAngles.y, 0);
            float h = Mathf.Lerp(height, aimHeight, aimBlend);
            Vector3 pivot = player.transform.position + Vector3.up * h
                        + rot * Vector3.right * (shoulderOffset * aimBlend);
            float d = Mathf.Lerp(distance, aimDistance, aimBlend);
            transform.SetPositionAndRotation(pivot - rot * Vector3.forward * d, rot);
        }
        else
        {
            Quaternion rot = Quaternion.Euler(topAngle, 0, 0);
            transform.SetPositionAndRotation(focus - rot * Vector3.forward * topDistance, rot);
        }
    }
    float topDistanceFix(float d) => d;

    void SetMode(bool top)
    {
        topDown = top;
        player.topDown = top;
        if (top) focus = player.transform.position;

        // w TPP kursor zablokowany, w top-down widoczny (do klikania PPM)
        Cursor.lockState = top ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = top;
    }
}