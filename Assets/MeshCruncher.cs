using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeshCruncher : MonoBehaviour
{
    public Material opaqueMaterial;
    public Mesh m;
    // Start is called before the first frame update
    List<Material> materials;
    public AnimationCurve ac;
    void Start()
    {
        materials = new List<Material>();
        List<CombineInstance> combines = new List<CombineInstance>();
        List<Transform> transforms = new List<Transform>();
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            var mr = mf.GetComponent<MeshRenderer>();

            for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
            {
                transforms.Add(mf.transform);
                var sharedMat = mr.sharedMaterials[i];
                var mat = Instantiate(sharedMat);
                Color c = mat.color;
                mr.sharedMaterials[i] = mat;

                //mat.EnableKeyword("_ALPHABLEND_ON");
                if (sharedMat == opaqueMaterial)
                {
                    c.a = 1;
                    mat.color = c;
                    mat.SetFloat("_Mode", 0);
                    mat.SetInt("_Zwrite", 1);
                    mat.renderQueue = 3000;
                }
                else
                {
                    //c.a = 0.4f;
                    mat.color = c;
                    mat.SetFloat("_Mode", 2);
                    mat.SetInt("_Zwrite", 1);
                    mat.renderQueue = 2000;
                }
                materials.Add(mat);
                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.transform = mf.transform.localToWorldMatrix;
                ci.subMeshIndex = i;
                combines.Add(ci);
            }
            //mr.enabled = false;
        }

        m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.CombineMeshes(combines.ToArray(), false);
        BoneWeight[] boneWeights = new BoneWeight[m.vertexCount];
        for (int i = 0; i < m.subMeshCount; i++)
        {
            var ind = m.GetIndices(i);
            for (int j = 0; j < ind.Length; j++)
            {
                boneWeights[ind[j]].boneIndex0 = i;
                boneWeights[ind[j]].weight0 = 1;
            }
        }
        m.boneWeights = boneWeights;
        Matrix4x4[] bindPoses = new Matrix4x4[transforms.Count];
        for (int i = 0; i < transforms.Count; i++)
        {
            bindPoses[i] = transforms[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }
        m.bindposes = bindPoses;
        var go = new GameObject("thing");
        go.AddComponent<MeshFilter>().sharedMesh = m;
        var smr = go.AddComponent<SkinnedMeshRenderer>();
        smr.bones = transforms.ToArray();
        smr.rootBone = transform;
        smr.sharedMaterials = materials.ToArray();
        smr.sharedMesh = m;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var mat in materials)
        {
            Color c = mat.color;
            c.a = ac.Evaluate(Mathf.PingPong(Time.time/2, 1));
            mat.color = c;
        }
    }

    public bool z;


}
