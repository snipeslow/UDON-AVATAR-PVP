
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
public class HitboxLayer : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    Collider myCollider;
    public bool IsHead = false;
    bool ToggleCollision = false;
    private void Start()
    {
        myCollider = GetComponent<Collider>();
    }
    private void FixedUpdate()
    {
        if(myCollider)
        {
            if (gameObject.layer != 17)
            {
                if (!ToggleCollision)
                {
                    myCollider.enabled = Networking.LocalPlayer.IsPlayerGrounded();
                    //myCollider.enabled = true;
                }
                else
                {
                    myCollider.enabled = false;
                }
                ToggleCollision = !ToggleCollision;
            }
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if(hitboxManager)
        {
            switch(gameObject.layer)
            {
                case 17: // Damage
                    if (IsHead)
                    {
                        hitboxManager.OnDamageHead();

                    } 
                    else
                    {
                        hitboxManager.OnDamageChest();

                    }
                    break;
                case 29: //Heal over time
                    hitboxManager.OnHOT();
                    break;
                case 30: //Heal
                    hitboxManager.OnHealChest();
                    break; 
                case 31: //Damage over time
                    hitboxManager.OnDOT();
                    break;
            }
        }
    }
}
