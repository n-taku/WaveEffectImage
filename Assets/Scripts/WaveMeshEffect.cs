using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(Subdivide))]
public class WaveMeshEffect : BaseMeshEffect
{
    [Serializable]
    public class WaveInfo
    {
        public float waveSpeed;
        public float frequency;
        public float amplitude;
    }
    public List<WaveInfo> waveList = new List<WaveInfo>();

    [SerializeField]
    bool isWaveTop = true;

    [SerializeField]
    bool isWaveBottom = true;

    [System.NonSerialized]
    private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    Subdivide m_Subdivide;
    private Subdivide subdivide
    {
        get
        {
            if (m_Subdivide == null)
                m_Subdivide = GetComponent<Subdivide>();
            return m_Subdivide;
        }
    }

    void Update()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        var output = ListPool<UIVertex>.Get();
        vh.GetUIVertexStream(output);
        ApplyVerts(output);
        vh.Clear();
        vh.AddUIVertexTriangleStream(output);
        ListPool<UIVertex>.Release(output);
    }

    protected void ApplyVerts(List<UIVertex> verts)
    {
        UIVertex vt;

        for (int i = 0; i < verts.Count; ++i)
        {
            int num = i % 6;
            var isBottom = (num == 0 || num == 4 || num == 5);

            if (isBottom && !isWaveBottom)
            {
                continue;
            }

            if (!isBottom && !isWaveTop)
            {
                continue;
            }

            vt = verts[i];
            vt.position.y += CalcOffset(vt.position.x);
            verts[i] = vt;
        }
    }

    float CalcOffset(float x)
    {
        float ret = 0;
        foreach (var wave in waveList)
        {
            ret += wave.amplitude * Mathf.Sin((Time.time * wave.waveSpeed + x * wave.frequency) * Mathf.Deg2Rad);
        }
        return ret;
    }
}
