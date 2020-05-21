using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MouseData : IComponentData
{
    public float m_delta;
    public float2 m_previousPosition;
    public float2 m_currentPosition;
    public float3 m_mousePositionInWorldSpace;
}