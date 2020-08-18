using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SetPlayerClass : UdonSharpBehaviour
{
    public int MaxHealth = 300;
    public float ChestDMG = 25;
    public float HeadDMG = 300;
    public float DOTAmount = 50;
    public float HOTAmount = 40;
    public float HealAmount = 75;
    public float JumpSpeed = 5;
    public float RunSpeed = 5;
    public float WalkSpeed = 3;
    public bool AllowJump = true;
    //public float GravityStrength = 1f;
    public float MinimumSize = 1f;
    public string ClassText = "Assault";
    public HitboxManager hitboxManager;
    public override void Interact()
    {
        if(hitboxManager)
        {
            hitboxManager.ChestDMG = ChestDMG;
            hitboxManager.HeadDMG = HeadDMG;
            hitboxManager.HealAmount = HealAmount;
            hitboxManager.MaxHealth = MaxHealth;
            hitboxManager.DOTAmount = DOTAmount;
            hitboxManager.HOTAmount = HOTAmount;
            hitboxManager.ClassText = ClassText;
            if (AllowJump)
            {
                Networking.LocalPlayer.SetJumpImpulse(JumpSpeed);

            }
            else
            {
                Networking.LocalPlayer.SetJumpImpulse(0);

            }
            Networking.LocalPlayer.SetRunSpeed(RunSpeed);
            Networking.LocalPlayer.SetWalkSpeed(WalkSpeed);
            //Networking.LocalPlayer.SetGravityStrength(GravityStrength);
            hitboxManager.MinimumSize = MinimumSize;
        }
    }
}
