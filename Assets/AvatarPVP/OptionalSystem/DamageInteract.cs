
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DamageInteract : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public float Amount = 25;
    public override void Interact()
    {
        if (hitboxManager)
        {
            hitboxManager.OnDamage(Amount);
        }
    }
}
