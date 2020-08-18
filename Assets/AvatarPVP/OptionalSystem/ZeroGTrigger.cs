
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
public class ZeroGTrigger : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public float Strength = 0.01f;
    public bool BehaveAsWater = false;
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
            if ((Vector3.Distance(Networking.LocalPlayer.GetPosition(), MyCollider.ClosestPoint(Networking.LocalPlayer.GetPosition())) <= 0.1f) && !Networking.LocalPlayer.IsPlayerGrounded())
            {
                hitboxManager.isInGZone = true;
                hitboxManager.BehaveAsWater = BehaveAsWater;
                hitboxManager.CurrentGravityZoneStrength = Mathf.Max(hitboxManager.CurrentGravityZoneStrength, Strength);
            }

        }
        
        
    }
}
