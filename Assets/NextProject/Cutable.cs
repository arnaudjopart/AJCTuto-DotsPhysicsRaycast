using Unity.Entities;

[GenerateAuthoringComponent]
internal struct Cutable : IComponentData
{
    public int m_meshIndex;
    public float m_points;
    public Entity m_cutEntity;
    public float m_numberOfParts;
}