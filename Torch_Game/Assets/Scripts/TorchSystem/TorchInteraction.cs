using UnityEngine;
using UnityEngine.InputSystem;

public class TorchInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Transform interactionOrigin;
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private LayerMask torchLayerMask = ~0;

    private void Reset()
    {
        interactionOrigin = transform;
    }

    private void Awake()
    {
        if (interactionOrigin == null)
            interactionOrigin = transform;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryLightNearbyTorch();
        }
    }

    public bool TryLightNearbyTorch()
    {
        Collider[] hits = Physics.OverlapSphere(
            interactionOrigin.position,
            interactionRadius,
            torchLayerMask,
            QueryTriggerInteraction.Ignore
        );

        Torch closestTorch = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Torch torch = hit.GetComponentInParent<Torch>();
            if (torch == null || torch.IsLit)
                continue;

            float distance = Vector3.Distance(interactionOrigin.position, torch.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTorch = torch;
            }
        }

        if (closestTorch == null)
            return false;

        closestTorch.LightTorch();
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = interactionOrigin != null ? interactionOrigin : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, interactionRadius);
    }
}