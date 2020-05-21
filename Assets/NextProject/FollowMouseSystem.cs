using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowMouseSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(MouseData)).ToComponentDataArray<MouseData>(Allocator.Temp);

        var isMouseDataAvailable = query.Length > 0;

        if (!isMouseDataAvailable) return;
        
        var mouseData = query[0];
        
        Entities.WithAll<FollowMouseComponent>().
            ForEach((Entity _entity, ref Translation _translation) =>
            {
                _translation.Value = new float3(mouseData.m_mousePositionInWorldSpace.x,mouseData
                .m_mousePositionInWorldSpace.y,0);
            }).Schedule();
        
    }
}