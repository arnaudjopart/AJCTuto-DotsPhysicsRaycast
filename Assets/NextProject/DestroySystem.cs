using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class DestroySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;
    private Random m_random;

    protected override void OnCreate()
    {
        m_endSimulationEntityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_random = new Random(56);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var ecb = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var randomLinearVelocityArray = new NativeArray<float3>(2,Allocator.TempJob);
        var randomAngularVelocityArray = new NativeArray<float3>(2,Allocator.TempJob);

        for (var i = 0; i < randomAngularVelocityArray.Length; i++)
        {
            randomLinearVelocityArray[i] = new float3(m_random.NextFloat(-2, 2),m_random.NextFloat(3, 5),0);
            randomAngularVelocityArray[i] = new float3(m_random.NextFloat(-2, 2),m_random.NextFloat(5, 10),m_random.NextFloat(6, 10));
        }

        Entities.WithoutBurst().ForEach((Entity _entity, int entityInQueryIndex, ref DestroyComponent _sliceComponent,
                ref
                    Translation _translation, ref Cutable _cutable) =>
            {

                var explosionData = new ExplosionData
                {
                    m_position = _translation.Value,
                    m_type = 0
                };

                EntityDestroyEventListener.DetectEvent(explosionData);

                for (int i = 0; i < _cutable.m_numberOfParts; i++)
                {
                    var halfFruit = ecb.Instantiate(entityInQueryIndex, _cutable.m_cutEntity);
                    ecb.SetComponent(entityInQueryIndex, halfFruit, new Translation
                    {
                        Value = _translation.Value
                    });

                    ecb.SetComponent(entityInQueryIndex, halfFruit, new Rotation()
                    {
                        Value = Quaternion.Euler(0, i * 180, 0)
                    });

                    ecb.SetComponent(entityInQueryIndex, halfFruit, new PhysicsVelocity()
                    {
                        Linear = randomLinearVelocityArray[i],
                        Angular = randomAngularVelocityArray[i]
                    });
                }
                ecb.DestroyEntity(entityInQueryIndex, _entity);

            }).WithDeallocateOnJobCompletion(randomLinearVelocityArray).WithDeallocateOnJobCompletion(randomAngularVelocityArray)
            .ScheduleParallel();

        m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}