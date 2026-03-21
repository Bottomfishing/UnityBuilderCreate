using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class TextEffects : BaseMeshEffect
{
    [Header("渐变设置")]
    public bool useGradient = false;
    public GradientType gradientType = GradientType.Vertical;
    public Color topColor = Color.white;
    public Color bottomColor = Color.black;
    public Color leftColor = Color.white;
    public Color rightColor = Color.black;

    [Header("描边设置")]
    public bool useOutline = false;
    public Color outlineColor = Color.black;
    [Range(0.1f, 5f)]
    public float outlineWidth = 1f;

    public enum GradientType
    {
        Horizontal,
        Vertical,
        DiagonalLeftTop,
        DiagonalLeftBottom
    }

    private List<UIVertex> m_Verts = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        vh.GetUIVertexStream(m_Verts);

        if (m_Verts.Count == 0)
        {
            vh.Clear();
            vh.AddUIVertexTriangleStream(m_Verts);
            return;
        }

        if (useGradient)
        {
            ApplyGradient(m_Verts);
        }

        if (useOutline)
        {
            ApplyOutline(m_Verts);
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_Verts);
        m_Verts.Clear();
    }

    private void ApplyGradient(List<UIVertex> verts)
    {
        float minX = verts[0].position.x;
        float maxX = verts[0].position.x;
        float minY = verts[0].position.y;
        float maxY = verts[0].position.y;

        for (int i = 1; i < verts.Count; i++)
        {
            minX = Mathf.Min(minX, verts[i].position.x);
            maxX = Mathf.Max(maxX, verts[i].position.x);
            minY = Mathf.Min(minY, verts[i].position.y);
            maxY = Mathf.Max(maxY, verts[i].position.y);
        }

        float width = maxX - minX;
        float height = maxY - minY;

        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex vertex = verts[i];
            Color color = vertex.color;

            float xPercent = (vertex.position.x - minX) / width;
            float yPercent = (vertex.position.y - minY) / height;

            switch (gradientType)
            {
                case GradientType.Horizontal:
                    color = Color.Lerp(leftColor, rightColor, xPercent);
                    break;
                case GradientType.Vertical:
                    color = Color.Lerp(bottomColor, topColor, yPercent);
                    break;
                case GradientType.DiagonalLeftTop:
                    float diagonalPercent1 = (xPercent + yPercent) / 2f;
                    color = Color.Lerp(bottomColor, topColor, diagonalPercent1);
                    break;
                case GradientType.DiagonalLeftBottom:
                    float diagonalPercent2 = (xPercent + (1 - yPercent)) / 2f;
                    color = Color.Lerp(topColor, bottomColor, diagonalPercent2);
                    break;
            }

            vertex.color = color;
            verts[i] = vertex;
        }
    }

    private void ApplyOutline(List<UIVertex> verts)
    {
        List<UIVertex> tempVerts = new List<UIVertex>(verts);
        verts.Clear();

        Vector2[] offsets = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        for (int o = 0; o < offsets.Length; o++)
        {
            Vector2 offset = offsets[o] * outlineWidth;
            for (int i = 0; i < tempVerts.Count; i++)
            {
                UIVertex v = tempVerts[i];
                v.position += new Vector3(offset.x, offset.y, 0);
                v.color = outlineColor;
                verts.Add(v);
            }
        }

        for (int i = 0; i < tempVerts.Count; i++)
        {
            verts.Add(tempVerts[i]);
        }
    }
}
