using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public float speed = 5f, crouchSpeed = 2.5f;
    public float jumpHeight = 1.2f, gravity = -20f;
    public float mouseSens = 2f;
    public KeyCode jumpKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [HideInInspector] public bool topDown;   // ustawia CameraManager

    CharacterController cc;
    float velY;
    Vector3? target;                         // cel dla ruchu myszą (pkt 8)
    float standHeight, standCenterY;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        standHeight = cc.height;
        standCenterY = cc.center.y;
    }

    public void SetTarget(Vector3 p) => target = p;

    void Update()
    {
        bool grounded = cc.isGrounded;
        if (grounded && velY < 0) velY = -2f;

        bool crouching = Input.GetKey(crouchKey);
        float spd = crouching ? crouchSpeed : speed;
        Vector3 move = Vector3.zero;

        if (!topDown)
        {
            // pkt 5: mysz obraca postać
            transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSens, 0);
            move = transform.right * Input.GetAxis("Horizontal")
                 + transform.forward * Input.GetAxis("Vertical");
            if (move.sqrMagnitude > 1f) move.Normalize();
            target = null;
        }
        else if (target.HasValue)
        {
            // pkt 8: idź do klikniętego punktu
            Vector3 d = target.Value - transform.position;
            d.y = 0;
            if (d.magnitude < 0.15f) target = null;
            else
            {
                move = d.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(move), 10f * Time.deltaTime);
            }
        }

        // skok
        if (grounded && Input.GetKeyDown(jumpKey) && !crouching)
        {
            velY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator?.SetTrigger("Jump");
        }
        velY += gravity * Time.deltaTime;

        cc.Move((move * spd + Vector3.up * velY) * Time.deltaTime);

        // kucanie: zmniejszamy kapsułę kolizji
        float h = crouching ? standHeight * 0.6f : standHeight;
        cc.height = Mathf.Lerp(cc.height, h, 10f * Time.deltaTime);
        cc.center = new Vector3(cc.center.x, standCenterY - (standHeight - cc.height) / 2f, cc.center.z);

        if (animator)
        {
            animator.SetFloat("Speed", move.magnitude , 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", crouching);
        }
    }
}