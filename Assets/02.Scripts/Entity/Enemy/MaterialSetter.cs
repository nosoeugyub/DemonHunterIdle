using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    [SerializeField]
    private GameObject modelRoot;
    [SerializeField]
    private List<Material> materials;

    private bool isInit = false;
    SkinnedMeshRenderer[] meshRenderers = null;
    private Dictionary<string, Material> materialsDict = new();

    public void SetMaterial(string name)
    {
        if (!isInit)
            Init();
        Material mat = GetMaterial(name);
        if (mat == null)
            Game.Debbug.Debbuger.ErrorDebug($"Material not found : {name}. Check {transform.name} prefab MaterialSetter");

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = mat;
        }
    }
    public List<Material> GetAllMaterialsInModel()
    {
        if (!isInit)
            Init();
        List<Material> tmpMaterials = new List<Material>();
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            tmpMaterials.Add(meshRenderers[i].material);
        }

        return tmpMaterials;
    }

    private void Init()
    {
        if(isInit) return;

        meshRenderers = modelRoot.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        materialsDict.Clear();

        for (int i = 0; i < materials.Count; i++)
        {
            materialsDict.Add(materials[i].name, materials[i]);
        }
        isInit = true;
    }

    private Material GetMaterial(string name)
    {
        if (!isInit)
            Init();
        
        if(materialsDict.ContainsKey(name)) 
            return materialsDict[name];
        
        return null;
    }
}
