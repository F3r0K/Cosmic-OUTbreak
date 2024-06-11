using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnDamage : MonoBehaviour
{
    public Color DamageColor = Color.red;

    public float DamageBorderPulseAmount = 4f;

    public float DamageFadeDuration = 0.15f;
    private float _fadeTime = 0f;

    public Renderer[] BorderRenderers;

    private Material _borderBaseMaterial;

    public void StartDamageFlash()
    {
        _fadeTime = DamageFadeDuration;
    }

    private void Awake()
    {
        if (BorderRenderers.Length > 0)
        {
            _borderBaseMaterial = BorderRenderers[0].sharedMaterial;
        }
    }

    private void Update()
    {
        if (_fadeTime > 0f)
        {
            _fadeTime -= Time.deltaTime;

            foreach (var t in BorderRenderers)
            {
                t.material.color =
                    Color.Lerp(Color.black, DamageColor, _fadeTime / DamageFadeDuration);

                t.material.SetFloat("_OutlineWidth",
                    Mathf.Lerp(0, DamageBorderPulseAmount,
                        _fadeTime / DamageFadeDuration));
            }

            if (_fadeTime <= 0f)
            {
                foreach (var t in BorderRenderers)
                {
                    Destroy(t.material);
                    t.material = _borderBaseMaterial;
                }
            }
        }
    }
}
