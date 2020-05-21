using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PrefabEntityComponent : IComponentData
{
    public Entity m_entityPrefab;
    public bool m_isActive;
    public float m_spawnThreshold;
}
