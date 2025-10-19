using UnityEngine;

public class GauntletMounts : MonoBehaviour
{
    [Header("Assign hand mount points (empty child Transforms on the player)")]
    public Transform LeftHandMount;
    public Transform RightHandMount;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (LeftHandMount) { Gizmos.DrawWireSphere(LeftHandMount.position, 0.05f); Gizmos.DrawLine(transform.position, LeftHandMount.position); }
        if (RightHandMount){ Gizmos.DrawWireSphere(RightHandMount.position, 0.05f); Gizmos.DrawLine(transform.position, RightHandMount.position); }
    }
}