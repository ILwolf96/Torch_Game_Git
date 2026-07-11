using UnityEngine;

public class Torch : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer torchRenderer;
    [SerializeField] private Light torchLight;

    [Header("Colors")]
    [SerializeField] private Color unlitColor = new Color(0.35f, 0.18f, 0.05f);
    [SerializeField] private Color litColor = Color.yellow;

    [Header("Light Settings")]
    [SerializeField] private float litIntensity = 3f;

    public bool IsLit { get; private set; }

    private Material runtimeMaterial;

    private void Awake()
    {
        if (torchRenderer == null)
            torchRenderer = GetComponentInChildren<Renderer>();

        if (torchRenderer != null)
            runtimeMaterial = torchRenderer.material;

        ApplyVisualState();
    }

    public void LightTorch()
    {
        if (IsLit)
            return;

        IsLit = true;
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        if (runtimeMaterial != null)
            runtimeMaterial.color = IsLit ? litColor : unlitColor;

        if (torchLight != null)
        {
            torchLight.enabled = IsLit;
            torchLight.intensity = IsLit ? litIntensity : 0f;
            torchLight.color = litColor;
        }
    }
}