using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowMouseMono : MonoBehaviour
{
    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_mouseEntity = m_entityManager.CreateEntity(typeof(MouseData));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = GetMouseWorldPositionFromJob();
        transform.position = new Vector3(mousePosition.x,mousePosition.y,0);
    }

    public struct MousePositionJob : IJobParallelFor
    {
        public NativeArray<MouseData> m_mouseDatas;
        public NativeArray<float3> result;
        public void Execute(int index)
        {
            result[index] = m_mouseDatas[index].m_mousePositionInWorldSpace;
        }
    }

    private float3 GetMouseWorldPositionFromJob()
    {
        var mouseDatas = new NativeArray<MouseData>(1, Allocator.TempJob)
        {
            [0] = m_entityManager.GetComponentData<MouseData>(m_mouseEntity)
        };
        var result = new NativeArray<float3>(1,Allocator.TempJob);
        var job = new MousePositionJob
        {
            m_mouseDatas = mouseDatas,
            result = result 
        };

        var jobHandle = job.Schedule(1,5);
        jobHandle.Complete();
        
        var position = result[0];
        
        mouseDatas.Dispose();
        result.Dispose();
        
        return position;

    }

    private Entity m_mouseEntity;
    private EntityManager m_entityManager;

    private Vector3 m_lastMousePosition;
}


