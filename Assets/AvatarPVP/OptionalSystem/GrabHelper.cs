
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
 
public class GrabHelper : UdonSharpBehaviour
{
    public override void OnDrop()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }
    public override void OnPickup()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
    private void FixedUpdate()
    {
        VRCPlayerApi vrcpapi = Networking.LocalPlayer;

        if (vrcpapi != null)
        {
            VRCPlayerApi.TrackingData trackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                transform.position = trackingData.position;
                transform.position += trackingData.rotation * Vector3.forward;
            }
        }
    }
}
