using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct HighlightComponent : IComponentData
{
    public bool m_isHighlighted;
}