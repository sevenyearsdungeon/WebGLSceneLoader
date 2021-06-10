using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleMaterialFade : MonoBehaviour
{
    [Range(0, 1)]
    public float alpha;
    float lastAlpha = 0;
    Material myMat;
    // Start is called before the first frame update
    void Start()
    {
        var mr = GetComponent<MeshRenderer>();
        myMat = mr.material;
        mr.material = myMat;

    }

    // Update is called once per frame
    void Update()
    {
        if (lastAlpha != alpha)
        {
            UpdateMaterial();
        }
        lastAlpha = alpha;

    }

    private void UpdateMaterial()
    {
        SetFade(alpha < 1);
        Color c = myMat.color;
        c.a = alpha;
        myMat.color = c;
    }

    void SetFade(bool fade)
    {
        myMat.SetFloat("_Mode", fade ? 2 : 0);
        //myMat.SetInt("_Zwrite", fade ? 1 : 0);
    }
}
