
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
public class HitboxManager : UdonSharpBehaviour
{
    public int MaxHealth = 300;
    public float Health = 300;
    public float ChestDMG = 25;
    public float HeadDMG = 300;
    public float DOTAmount = 50;
    public float DOTTime = 0;
    public int DOTPulses = 0;
    public float HOTAmount = 40;
    public float HOTTime = 0;
    public int HOTPulses = 0;
    public float HealAmount = 75;
    public float JumpSpeed = 5;
    public float RunSpeed = 5;
    public float WalkSpeed = 3;

    public float MinimumSize = 1f;
    public Text TextUI;
    public Text TextUI2;
    public MeshRenderer vrcPVDMR;
    public string ClassText = "Assault";
    [UdonSynced(UdonSyncMode.None)]
    bool UseClassicHitbox = true;
    public bool AllowJump = true;

    public bool LimitHitbox = false;
    //public bool JumpEnabled = false;
    GameObject HeadProxy;
    public Transform DefaultRespawnPoint;

    public GameObject ClassicHitbox;

    public GameObject AdvancedHitbox;
    Transform ActiveRespawnPoint;
    bool RemovedDefaultVisual = false;
    public bool SetHitboxMode(bool targetMode)
    {
        if(Networking.LocalPlayer.isMaster)
        {
            UseClassicHitbox = targetMode;
        }
        return UseClassicHitbox;
    }
    public void SetHitboxRender(bool targetState)
    {
        foreach(MeshRenderer meshr in transform.GetComponents<MeshRenderer>())
        {
            meshr.enabled = targetState;
        }
    }
    public bool ResetRespawn()
    {
        if (DefaultRespawnPoint)
        {
            Networking.LocalPlayer.CombatSetRespawn(true, 5, DefaultRespawnPoint);
            ActiveRespawnPoint = DefaultRespawnPoint;
            return true;
        }
        return false;
    }
    public bool SetRespawn(Transform newRespawnPoint)
    {
        if(newRespawnPoint)
        {
            Networking.LocalPlayer.CombatSetRespawn(true, 5, newRespawnPoint);
            ActiveRespawnPoint = newRespawnPoint;
            return true;
        }
        return false;
    }
    public Transform GetRespawn()
    {
        return ActiveRespawnPoint;
    }
    public GameObject GetHeadProxy()
    {
        return HeadProxy;
    }
    private void Start()
    {
        Health = MaxHealth;
        if(Networking.LocalPlayer != null)
        {
            Networking.LocalPlayer.CombatSetMaxHitpoints(100);
            Networking.LocalPlayer.CombatSetCurrentHitpoints(100);
            Networking.LocalPlayer.CombatSetup();
            if(DefaultRespawnPoint)
            {
                Networking.LocalPlayer.CombatSetRespawn(true, 5, DefaultRespawnPoint);
                ActiveRespawnPoint = DefaultRespawnPoint;

            }
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
             
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi targetPlayerAPI)
    {
        targetPlayerAPI.CombatSetup();
    }
    public void OnHOT()
    {
        if (Networking.LocalPlayer.CombatGetCurrentHitpoints() > 0)//Prevent extra damage when ragdolled
        {
            if (HOTPulses <= 0)
            {
                HOTPulses = 4;
                HOTTime = 1f;
                OnHeal(HOTAmount);
            }

        }
    }
    public void OnHealHead()
    {
        OnHeal(HealAmount);
    }
    public void OnHealChest() 
    {
        OnHeal(HealAmount);
    }
    public void OnHeal(float heal)
    {
        if (Networking.LocalPlayer.CombatGetCurrentHitpoints() > 0)//Prevent extra damage when ragdolled
        {
            if (vrcPVDMR)
            {
                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                float GreyScale = (float)Health / (float)MaxHealth;
                materialPropertyBlock.SetColor("_Color", new Color(1, 1, 1, 1f - GreyScale));
                vrcPVDMR.SetPropertyBlock(materialPropertyBlock);
            }
            Health += heal;
            Health = Mathf.Min(Health, MaxHealth);
        }
            
    }
    public void OnDOT()
    {
        if (Networking.LocalPlayer.CombatGetCurrentHitpoints() > 0)//Prevent extra damage when ragdolled
        {
            if (DOTPulses <= 0)
            {
                DOTPulses = 4;
                DOTTime = 1f;
                OnDamage(DOTAmount);
            }
        }
    }
    public void OnDamageHead()
    {
        OnDamage(HeadDMG);
    }
    public void OnDamageChest()
    {
        OnDamage(ChestDMG);
    }
    public void OnDamage(float damage)
    {
        if(Networking.LocalPlayer.CombatGetCurrentHitpoints() > 0)//Prevent extra damage when ragdolled
        {
            Health -= damage;
            //Our screen effect
            if (vrcPVDMR)
            {
                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                float GreyScale = (float)Health / (float)MaxHealth;
                materialPropertyBlock.SetColor("_Color", new Color(1, 1, 1, 1f - GreyScale));
                vrcPVDMR.SetPropertyBlock(materialPropertyBlock);
            }
            if (Health <= 0)
            {

                OnDeath();
            }

        }
    }

    public void OnDeath()
    {

        Health = MaxHealth;
        DOTPulses = 0;
        DOTTime = 0f;
        HOTPulses = 0;
        HOTTime = 0f;
        //Our screen effect
        if (vrcPVDMR)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            float GreyScale = (float)Health / (float)MaxHealth;
            materialPropertyBlock.SetColor("_Color", new Color(1, 1, 1, 1f - GreyScale));
            vrcPVDMR.SetPropertyBlock(materialPropertyBlock);
        }
        Networking.LocalPlayer.CombatSetCurrentHitpoints(0);
    }
    
    private void FixedUpdate()
    {
        VRCPlayerApi vrcpapi = Networking.LocalPlayer;
        
        if (vrcpapi != null)
        {
            VRCPlayerApi.TrackingData trackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            RunGravityMechanic(trackingData);
            RunForceVelocityMechanic();
            transform.position = trackingData.position;
            transform.rotation = Networking.LocalPlayer.GetRotation();
            if (!HeadProxy)
            {
                HeadProxy = GameObject.Find("Head Proxy");
            }
            //Please remind me to make this better later
            if (HeadProxy)
            {
                if(UseClassicHitbox)
                {
                    if(ClassicHitbox)
                    {
                        ClassicHitbox.SetActive(true);
                    }
                    if (AdvancedHitbox)
                    {
                        AdvancedHitbox.SetActive(false);
                    }
                    if(LimitHitbox)
                    {
                        transform.localScale = Vector3.one * Mathf.Max(MinimumSize, Vector3.Magnitude(HeadProxy.transform.localScale));
                    }
                    else
                    {
                        transform.localScale = Vector3.one * Vector3.Magnitude(HeadProxy.transform.localScale);

                    }
                }
                else
                {
                    if (ClassicHitbox)
                    {
                        ClassicHitbox.SetActive(false);
                    }
                    if (AdvancedHitbox)
                    {
                        AdvancedHitbox.SetActive(true);
                    }
                    transform.localScale = Vector3.one;

                }
                if (!RemovedDefaultVisual)
                {
                    Transform VRC_PlayerVisualDamage = HeadProxy.transform.Find("VRC_PlayerVisualDamage(Clone)");
                    if (VRC_PlayerVisualDamage)
                    {
                        MeshRenderer vrcPVDMR_Old = VRC_PlayerVisualDamage.GetComponent<MeshRenderer>();

                        if (vrcPVDMR_Old)
                        {
                            if (vrcPVDMR)
                            {
                                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                                materialPropertyBlock.SetColor("_Color", new Color(1, 1, 1, 0));
                                vrcPVDMR.SetPropertyBlock(materialPropertyBlock);
                                RemovedDefaultVisual = true;
                            }
                            vrcPVDMR_Old.enabled = false;
                        }
                    }
                }
                if (vrcPVDMR)
                {
                    vrcPVDMR.transform.position = trackingData.position;
                    if (Networking.LocalPlayer.IsUserInVR())
                    {
                        vrcPVDMR.transform.position += trackingData.rotation * (Vector3.forward * 0.25f * Vector3.Magnitude(HeadProxy.transform.localScale));
                    }
                    else
                    {
                        vrcPVDMR.transform.position += trackingData.rotation * (Vector3.forward * 0.5f * Vector3.Magnitude(HeadProxy.transform.localScale));

                    }
                    vrcPVDMR.transform.rotation = trackingData.rotation;
                }
            }
            RunHOTMechanic();
            RunDOTMechanic();

            //Used for Debugging, but can be used to display your class on a board
            if (TextUI)
            {
                TextUI.text = "Class: "+ ClassText+ "\nHealth: " + Health;
                TextUI.text += "\nMax Health: " + MaxHealth;
                TextUI.text += "\nBody Damage: " + ChestDMG;
                TextUI.text += "\nHead Damage: " + HeadDMG;
                TextUI.text += "\nDOT Amount: " + DOTAmount;
                TextUI.text += "\nHOT Amount: " + HOTAmount;
                TextUI.text += "\nHeal Amount: " + HealAmount;
                TextUI.text += "\nIs Grounded: " + Networking.LocalPlayer.IsPlayerGrounded();
                TextUI.text += "\nGravity: " + Networking.LocalPlayer.GetGravityStrength();
                if(TextUI2)
                {
                    TextUI2.text = TextUI.text;
                }
            }
        }
    }
    void RunHOTMechanic()
    {
        if (HOTPulses > 0)
        {
            if (HOTTime > 0)
            {
                HOTTime -= Time.fixedDeltaTime;
                if (HOTTime <= 0)
                {
                    HOTPulses--;
                    if (HOTPulses > 0)
                    {
                        //HOTPulses = 4;
                        HOTTime = 1f;
                    }
                    OnHeal(HOTAmount);
                }
            }

        }
    }
    void RunDOTMechanic()
    {
        if (DOTPulses > 0)
        {
            if (DOTTime > 0)
            {
                DOTTime -= Time.fixedDeltaTime;
                if (DOTTime <= 0)
                {
                    DOTPulses--;
                    if (DOTPulses > 0)
                    {
                        //DOTPulses = 4;
                        DOTTime = 1f;
                    }
                    OnDamage(DOTAmount);
                }
            }

        }

    }
    public bool isInVZone = false;
    public Vector3 ForceVelocity = Vector3.zero;
    void RunForceVelocityMechanic()
    {
        if(isInVZone)
        {
            ForceVelocity *= Time.fixedDeltaTime;
            ForceVelocity += Networking.LocalPlayer.GetVelocity();
            Networking.LocalPlayer.SetVelocity(ForceVelocity);
            ForceVelocity = Vector3.zero;
            isInVZone = false;
        }
    }
    public float CurrentGravityZoneStrength = 1f;
    public bool BehaveAsWater = false;
    public bool isInGZone = false;
    void RunGravityMechanic(VRCPlayerApi.TrackingData trackingData)
    {
        if (isInGZone)
        {

            Vector3 newVel = new Vector3(Networking.LocalPlayer.GetVelocity().x * CurrentGravityZoneStrength * Time.fixedDeltaTime, Networking.LocalPlayer.GetVelocity().y * CurrentGravityZoneStrength * Time.fixedDeltaTime, Networking.LocalPlayer.GetVelocity().z * CurrentGravityZoneStrength * Time.fixedDeltaTime);
            if (BehaveAsWater)
            {
                newVel.y += 1f;
            }
            newVel += (trackingData.rotation * Vector3.forward) * Input.GetAxis("Vertical");
            newVel += (trackingData.rotation * Vector3.right) * Input.GetAxis("Horizontal");
            if (Networking.LocalPlayer.IsUserInVR())
            {

                newVel += (trackingData.rotation * Vector3.up) * Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical");
            }
            else
            {
                newVel += (trackingData.rotation * Vector3.up) * (Input.GetKey(KeyCode.Space) ? 1f : 0f);
                newVel += (trackingData.rotation * Vector3.down) * (Input.GetKey(KeyCode.LeftControl) ? 1f : 0f);

            }
            newVel = Vector3.ClampMagnitude(newVel, 1f) * Networking.LocalPlayer.GetRunSpeed();
            Networking.LocalPlayer.SetVelocity(newVel);
            isInGZone = false;
            BehaveAsWater = false;
            CurrentGravityZoneStrength = 1f;
        }
    }
}
