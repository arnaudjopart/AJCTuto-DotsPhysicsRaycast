using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawningSystem : SystemBase
{

    private Unity.Mathematics.Random m_random;
    
    private float m_timer = 1f;
    private EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;
    private EntityCommandBuffer m_entityCommandBuffer;

    protected override void OnCreate()
    {
        m_endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_random = new Unity.Mathematics.Random(546);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        m_timer -= Time.DeltaTime;
        if (m_timer < 0f)
        {
            var entityQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabEntityComponent>());

            var entityArray = entityQuery.ToEntityArray(Allocator.TempJob);
            var arrayLength = entityArray.Length;
            
            var ecb = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            //Spawn
            m_timer = 1f;
            var randomVelocityArray = new NativeArray<float3>(arrayLength,Allocator.TempJob);
            var randomXPositionArray = new NativeArray<float>(arrayLength,Allocator.TempJob);

            for (int i = 0; i < randomVelocityArray.Length; i++)
            {
                var randomVelocity = new float3(m_random.NextFloat(-2,2),m_random.NextFloat(10,12),0);
                randomVelocityArray[i] = randomVelocity;
                var randomX = m_random.NextFloat(-5, 5);
                randomXPositionArray[i] = randomX;
            }

            var spawnThreshold = m_random.NextFloat();
            
            Entities.ForEach((Entity _entity, int entityInQueryIndex, ref PrefabEntityComponent _prefabEntityComponent) =>
                {
                    if (!_prefabEntityComponent.m_isActive) return;
                    if (!(spawnThreshold > _prefabEntityComponent.m_spawnThreshold)) return;
                    
                    var newEntity = ecb.Instantiate(entityInQueryIndex,_prefabEntityComponent.m_entityPrefab);
                        
                    ecb.SetComponent(entityInQueryIndex,newEntity,new Translation
                    {
                        Value = new float3(randomXPositionArray[entityInQueryIndex],0,0)
                    });
                    ecb.SetComponent(entityInQueryIndex,newEntity,new PhysicsVelocity()
                    {
                        Linear = randomVelocityArray[entityInQueryIndex],
                        Angular = new float3(0,0,3)
                    });

                })
                .WithDeallocateOnJobCompletion(randomVelocityArray)
                .WithDeallocateOnJobCompletion(randomXPositionArray)
                .ScheduleParallel();
            
            /*CompleteDependency();
            
            randomVelocityArray.Dispose();
            randomXPositionArray.Dispose();*/
            entityArray.Dispose();
            
            m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
            
            

        }
        
    }
}


public class EntitySpawnerSystem : ComponentSystem
{
    private float m_timer =1 ;
    private Unity.Mathematics.Random m_random;

    protected override void OnCreate()
    {
        m_random = new Unity.Mathematics.Random(546);
    }

    protected override void OnUpdate()
    {
        /*m_timer -= Time.DeltaTime;
        if (m_timer < 0f)
        {
            m_timer = 1f;

            Entities.ForEach((ref PrefabEntityComponent _prefabEntityComponent) =>
                {
                    var entity = EntityManager.Instantiate(_prefabEntityComponent.m_entityPrefab);
                    
                    EntityManager.SetComponentData(entity, new Translation
                    {
                        Value = new float3(m_random.NextFloat(-5f,5), -5, 0)
                    });
                    EntityManager.SetComponentData(entity, new PhysicsVelocity
                    {
                        Linear = new float3(m_random.NextFloat(-3f,3f),m_random.NextFloat(2f,6f),0)
                    });
                });
        }*/
    }
}