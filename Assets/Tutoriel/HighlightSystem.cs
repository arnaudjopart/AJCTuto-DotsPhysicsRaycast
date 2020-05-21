using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using UnityEngine;

public class HighlightSystem : ComponentSystem
{
    
    protected override void OnUpdate()
    {
        Entities.WithAll<HighlightComponent>().ForEach((Entity _entity, ref HighlightComponent _highlight) =>
        {
            if (_highlight.m_isHighlighted)
            {
                
            }
            
        });
        
    }
}

