
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HOTInteract : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public override void Interact()
    {
        if (hitboxManager)
        {
            hitboxManager.OnHOT(); 
        }

    }
}
