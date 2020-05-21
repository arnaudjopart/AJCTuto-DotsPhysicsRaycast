using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class TriggerJobSystem : JobComponentSystem
{
    private BuildPhysicsWorld m_physicsWorld;
    private StepPhysicsWorld m_physicsStep;

    private EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_physicsStep = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new TriggerJob
        {
            cuterEntities = GetComponentDataFromEntity<PlayerComponentData>(),
            cutableEntities = GetComponentDataFromEntity<Cutable>(),
            waitingToBeDestroyedEntities = GetComponentDataFromEntity<DestroyComponent>(),
            m_entityCommandBuffer = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer()
        };
        
        var jobHandle = job.Schedule(m_physicsStep.Simulation, ref m_physicsWorld.PhysicsWorld, inputDeps);
        m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        jobHandle.Complete();
        
        return jobHandle;
    }

    [BurstCompile]
    private struct TriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<PlayerComponentData> cuterEntities;
        public ComponentDataFromEntity<Cutable> cutableEntities;
        public ComponentDataFromEntity<DestroyComponent> waitingToBeDestroyedEntities;
        public EntityCommandBuffer m_entityCommandBuffer;
        
        
        public void Execute(TriggerEvent triggerEvent)
        {
            
            if (cuterEntities.HasComponent(triggerEvent.Entities.EntityA))
            {
                if (cutableEntities.HasComponent(triggerEvent.Entities.EntityB)&& !waitingToBeDestroyedEntities.HasComponent(triggerEvent.Entities.EntityB))
                {
                    
                    Debug.Log("EntityA is Slicing EntityB");
                    
                    PlayerComponentData player = cuterEntities[triggerEvent.Entities.EntityA];
                    player.m_score += 1;
                    cuterEntities[triggerEvent.Entities.EntityA] = player;
                    
                    m_entityCommandBuffer.AddComponent(triggerEvent.Entities.EntityB,new DestroyComponent
                    {
                        useless = true
                    });
                }
            }
            
            if (cuterEntities.HasComponent(triggerEvent.Entities.EntityB))
            {
                if (cutableEntities.HasComponent(triggerEvent.Entities.EntityA) && !waitingToBeDestroyedEntities.HasComponent(triggerEvent.Entities.EntityA))
                {
                    Debug.Log("EntityB Player is Slicing EntityA");
                    PlayerComponentData player = cuterEntities[triggerEvent.Entities.EntityB];
                    player.m_score += 1;
                    cuterEntities[triggerEvent.Entities.EntityB] = player;
                    
                    m_entityCommandBuffer.AddComponent(triggerEvent.Entities.EntityA,new DestroyComponent
                    {
                        useless = true
                    });

                }
            }
        }
    }
}

public struct DestroyComponent : IComponentData
{
    public bool useless;
}


public struct ExplosionData
{
    public float3 m_position;
    public int m_type;
}