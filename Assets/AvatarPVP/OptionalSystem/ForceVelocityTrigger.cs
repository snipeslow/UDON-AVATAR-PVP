
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ForceVelocityTrigger : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public Vector3 ForceVelocity;
    Collider MyCollider;
    private void Start()
    {
        MyCollider = GetComponent<Collider>();
    }
    private void FixedUpdate()
    {
        //Currently a workaround
        if (MyCollider)
        {
            if ((Vector3.Distance(Networking.LocalPlayer.GetPosition(), MyCollider.ClosestPoint(Networking.LocalPlayer.GetPosition())) <= 0.1f))
            {
                hitboxManager.isInVZone = true;
                hitboxManager.ForceVelocity = Vector3.Max(hitboxManager.ForceVelocity, transform.rotation * ForceVelocity);
            }

        }


    }
}
