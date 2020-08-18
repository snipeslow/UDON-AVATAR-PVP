
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SetPlayerSpawn : UdonSharpBehaviour
{
    public HitboxManager hitboxManager;
    public Transform RespawnPoint;
    public override void Interact()
    {
        if (hitboxManager)
        {
            if (RespawnPoint)
            {
                hitboxManager.SetRespawn(RespawnPoint);

            }
            else
            {
                hitboxManager.ResetRespawn();
            }
        }
    }
}
