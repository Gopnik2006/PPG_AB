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

    public float zoomSpeed = 20f;   // ScrollWheel daje ~0.1 na "ząbek"

    Camera cam;
    bool topDown;
    float pitch = 15f;
    Vector3 focus;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetMode(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SetMode(!topDown);

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        if (!topDown)
        {
            distance = Mathf.Clamp(distance - scroll, minDist, maxDist);
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSens, -20f, 70f);
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
        if (!topDown)
        {
            Quaternion rot = Quaternion.Euler(pitch, player.transform.eulerAngles.y, 0);
            Vector3 pivot = player.transform.position + Vector3.up * height;
            transform.SetPositionAndRotation(pivot - rot * Vector3.forward * topDistanceFix(distance), rot);
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