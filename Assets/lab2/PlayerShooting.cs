using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerShooting : MonoBehaviour
{
    public Transform muzzle;                 // puste dziecko Playera przy lufie
    public GameObject weapon;                // model broni, widoczny przy celowaniu
    public GameObject crosshair;
    public Projectile[] projectilePrefabs;   // 0 = znika, 1 = zostaje 5 s
    public KeyCode switchKey = KeyCode.Q;
    public float fireRate = 0.25f, range = 100f;

    
    PlayerController pc;
    CharacterController cc;
    Camera cam;
    int type;
    float nextShot;

    void Start()
    {
        pc = GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
        if (weapon) weapon.SetActive(false);
        if (crosshair) crosshair.SetActive(false);
    }

    void Update()
    {
        bool aim = !pc.topDown && Input.GetMouseButton(1);
        if (aim != pc.aiming)
        {
            pc.aiming = aim;
            if (weapon) weapon.SetActive(aim);
            if (crosshair) crosshair.SetActive(aim);
        }

        if (pc.topDown) return;

        if (Input.GetKeyDown(switchKey))
        {
            type = (type + 1) % projectilePrefabs.Length;
            Debug.Log("Typ pocisku: " + projectilePrefabs[type].name);
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShot)
        {
            nextShot = Time.time + fireRate;
            Shoot();
        }
        
    }

    void Shoot()
    {
        // promień ze środka ekranu, startujący za graczem, żeby nie trafił w jego kapsułę
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin += ray.direction * Vector3.Distance(cam.transform.position, transform.position);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore)
            ? hit.point : ray.GetPoint(range);

        Vector3 dir = (targetPoint - muzzle.position).normalized;
        Projectile p = Instantiate(projectilePrefabs[type], muzzle.position, Quaternion.LookRotation(dir));
        p.Launch(dir, cc);
    }
}