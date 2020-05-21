using Unity.Entities;

[GenerateAuthoringComponent]
public struct SlicingFruitComponent : IComponentData
{
    public bool m_isSlicing;
}