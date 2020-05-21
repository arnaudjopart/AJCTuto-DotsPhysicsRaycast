using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EntityDestroyEventListener : MonoBehaviour
{
    private EntityManager m_entityManager;

    // Start is called before the first frame update
    void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void Update()
    {
        ListenToDestroyEvents();
    }

    private void ListenToDestroyEvents()
    {
        
        var job = new DestroyEventListenerJob();
        
    }

    public static void DetectEvent(ExplosionData _data)
    {
        print(_data.m_position);
    }
    
}

public struct DestroyEventListenerJob : IJobParallelFor
{
    private NativeArray<Entity> m_entities;
    public void Execute(int index)
    {
        
    }
}
