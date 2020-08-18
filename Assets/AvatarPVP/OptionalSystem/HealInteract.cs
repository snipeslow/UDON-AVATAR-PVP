
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HealInteract : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public float Amount = 75;
    public override void Interact()
    {
        if (hitboxManager)
        {
            //hitboxManager.Health = hitboxManager.MaxHealth;
            hitboxManager.OnHeal(Amount);
        }
    }
}
