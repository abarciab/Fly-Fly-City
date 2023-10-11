using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] RenderTexture camOutput;
    [SerializeField] float targetLuminance;
    [SerializeField] int numSamplePoints;
    [SerializeField] float avgLuminance, exposureChangeSpeed;
    [SerializeField] Volume ppVolume;
    [SerializeField] float targetExposure, deltaThreshold = 0.1f;
    [SerializeField] Camera cam;

    private void Update()
    {
        cam.Render();

        avgLuminance = CalculateAvgLuminance(camOutput);

        float lumDelta = targetLuminance - avgLuminance;
        if (Mathf.Abs(lumDelta) < deltaThreshold) return;

        if (lumDelta < 0) targetExposure -= exposureChangeSpeed;
        if (lumDelta > 0) targetExposure += exposureChangeSpeed;

        ppVolume.profile.TryGet<ColorAdjustments>(out var color);
        color.postExposure.value = targetExposure;
    }

    float CalculateAvgLuminance(RenderTexture renderTex)
    {
        RenderTexture.active = renderTex;
        float totalLuminance = 0;

        var tex = new Texture2D(renderTex.width, renderTex.height);
        tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        tex.Apply();
        Color[] Pixels = tex.GetPixels();

        for (int i = 0; i < numSamplePoints; i++) {
            var pixel = Pixels[Random.Range(0, Pixels.Length)];
            float luminance = 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b;
            totalLuminance += luminance;
        }

        if (Application.isPlaying) Destroy(tex);
        else DestroyImmediate(tex);
        RenderTexture.active = null;

        return totalLuminance / numSamplePoints;
    }
}
