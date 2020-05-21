using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MouseDataSystem : SystemBase
{
    private Camera m_camera;
    protected override void OnStartRunning()
    {
        m_camera = Camera.main;
    }
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((ref MouseData _mouseData) =>
        {
            Vector2 mousePosition = Input.mousePosition;
            
            _mouseData.m_currentPosition = mousePosition;
            var mousePositionInWorldSpace = m_camera.ScreenToWorldPoint(new Vector3(mousePosition.x,
                mousePosition.y, m_camera.nearClipPlane));
            
            var delta = math.distancesq(mousePositionInWorldSpace, _mouseData.m_mousePositionInWorldSpace);
            _mouseData.m_delta = delta;
            _mouseData.m_mousePositionInWorldSpace = mousePositionInWorldSpace;
            
        }).Run();

    }
}

