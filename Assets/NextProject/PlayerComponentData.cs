using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlayerComponentData : IComponentData
{
    public int m_score;
}