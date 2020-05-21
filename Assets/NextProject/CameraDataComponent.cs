using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CameraDataComponent :IComponentData
{
    public float3 m_position;
}