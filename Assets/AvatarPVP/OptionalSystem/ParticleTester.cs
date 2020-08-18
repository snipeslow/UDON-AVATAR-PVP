
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.Events;
public class ParticleTester : UdonSharpBehaviour
{
    public float ResetTime = 0.25f;
    public float InternalTimer = 0;
    public Color TargetHitColor = Color.red;
    private void OnParticleCollision(GameObject other)
    {
        if(InternalTimer <= 0)
        {
            MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_Color", TargetHitColor);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
            InternalTimer = ResetTime;
        }
    } 
    public void ImpactTimerEnd()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        meshRenderer.SetPropertyBlock(materialPropertyBlock);

    }
    private void Update()
    {
        if(InternalTimer > 0)
        {
            InternalTimer -= Time.deltaTime;
            if (InternalTimer <= 0 )
            {
                ImpactTimerEnd(); 
            }
        }
    }
}
