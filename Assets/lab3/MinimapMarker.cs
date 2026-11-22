using UnityEngine;

public enum MarkerType { Gracz, Interaktywny, Specjalny }

public class MinimapMarker : MonoBehaviour
{
    public MarkerType type = MarkerType.Interaktywny;
    public float size = 3f, height = 40f;
    Transform icon;

    Color GetColor() => type switch
    {
        MarkerType.Gracz => Color.green,
        MarkerType.Interaktywny => Color.yellow,
        _ => Color.red                      // Specjalny
    };

    void Start()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(go.GetComponent<Collider>());
        go.name = "MinimapIcon";
        go.layer = LayerMask.NameToLayer("Minimap");
        var r = go.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        r.material.color = GetColor();
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        icon = go.transform;
        icon.localScale = Vector3.one * size;
    }

    void LateUpdate()
    {
        icon.position = transform.position + Vector3.up * height;
        float yaw = type == MarkerType.Gracz ? transform.eulerAngles.y : 0f;
        icon.rotation = Quaternion.Euler(90f, yaw, 0f);
    }

    void OnDestroy() { if (icon) Destroy(icon.gameObject); }
}