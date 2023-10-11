using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ClothingData
{
    public GameObject obj;
    public Material mat;
    [Range (0, 1)] public float altMaterialChance = 0;
    public List<Material> altMaterials = new List<Material>();
    public bool useGradient;
    [ConditionalHide(nameof(useGradient))] public Gradient colorGradient;
    public bool useColorList;
    public List<Color> colorList = new List<Color>();
}

public class CitizenModelTemplate : MonoBehaviour
{
    [SerializeField] List<ClothingData> heads = new List<ClothingData>();
    [SerializeField] List<ClothingData> tops = new List<ClothingData>();
    [SerializeField] List<ClothingData> bottoms = new List<ClothingData>();

    [SerializeField] Vector3 scaleMin = Vector3.one, scaleMax = Vector3.one;
    [SerializeField, Range(0, 1)] float headchance = 0.5f;

    [SerializeField] Gradient skinColorGradient;
    [SerializeField] Renderer skinRenderer, blushRenderer;
    [SerializeField] Color blushColorAddition;
    [SerializeField] float blushLerp = 0.5f;

    private void Start()
    {
        HideAll();
        Generate();
    }

    void HideAll()
    {
        foreach (var h in heads) h.obj.SetActive(false);
        foreach (var t in tops) t.obj.SetActive(false);
        foreach (var b in bottoms) b.obj.SetActive(false);
    }

    void Generate()
    {
        transform.localScale = Vector3.Lerp(scaleMin, scaleMax, Random.Range(0.0f, 1));

        if (heads.Count > 0 && Random.Range(0.0f, 1) < headchance) Configure(heads[Random.Range(0, heads.Count)]);
        if (tops.Count > 0) Configure(tops[Random.Range(0, tops.Count)]);
        if (bottoms.Count > 0) Configure(bottoms[Random.Range(0, bottoms.Count)]);

        var skinColor = skinColorGradient.Evaluate(Random.Range(0.0f, 1));
        skinRenderer.material.color = skinColor;
        blushRenderer.material.color = Color.Lerp(skinColor, blushColorAddition, blushLerp);
    }

    void Configure(ClothingData data)
    {
        var renderer = data.obj.GetComponent<Renderer>();

        if (Random.Range(0.0f, 1) < data.altMaterialChance) renderer.material = data.altMaterials[Random.Range(0, data.altMaterials.Count)];
        else renderer.material = data.mat;

        if (data.useGradient) renderer.material.color = data.colorGradient.Evaluate(Random.Range(0.0f, 1));
        else if (data.useColorList) renderer.material.color = data.colorList[Random.Range(0, data.colorList.Count)];

        data.obj.SetActive(true);
    }
}
