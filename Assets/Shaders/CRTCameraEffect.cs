using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CRTCameraEffect : MonoBehaviour
{
    public Material crtMaterial;
    
    [SerializeField]
    private Color displayColor = Color.green;

    [SerializeField, Range(0f, 100f)]
    private float flickerFrequency = 10.0f;

    [SerializeField, Range(0f, 0.1f)]
    private float ghostingAmount = 0.005f;

    [SerializeField, Range(0f, 50f)]
    private float scanlineSpeed = 1.0f;

    [SerializeField, Range(0f, 200f)]
    private float scanlineAmount = 100.0f;

    [SerializeField, Range(0f, 0.2f)]
    private float scanlineIntensity = 0.1f;


    
    void Start()
    {
        if (crtMaterial != null)
        {
            UpdateMaterialProperties();
        }
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (crtMaterial != null)
        {
            crtMaterial.SetColor("_Color", displayColor);
            crtMaterial.SetFloat("_FlickerFrequency", flickerFrequency);
            crtMaterial.SetFloat("_GhostingAmount", ghostingAmount);
            crtMaterial.SetFloat("_ScanlineSpeed", scanlineSpeed);
            crtMaterial.SetFloat("_ScanlineAmount", scanlineAmount);
            crtMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (crtMaterial != null)
        {
            Graphics.Blit(src, dest, crtMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
