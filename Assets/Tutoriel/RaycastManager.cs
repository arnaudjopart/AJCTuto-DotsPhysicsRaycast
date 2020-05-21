
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;


public static class RaycastManager
{
   [BurstCompile]
   public struct RaycastJob : IJobParallelFor
   {
      [ReadOnly] public CollisionWorld m_collisionWorld;
      [ReadOnly] public NativeArray<RaycastInput> m_inputs;
      public NativeArray<RaycastHit> m_results;
      public void Execute(int _index)
      {
         m_collisionWorld.CastRay(m_inputs[_index], out var hit);
         m_results[_index] = hit;
      }
   }
   
   public static void SingleRaycast(CollisionWorld _collisionWorld, RaycastInput _input, ref RaycastHit _hit)
   {
      var rayCommand = new NativeArray<RaycastInput>(1,Allocator.TempJob);
      var rayResult = new NativeArray<RaycastHit>(1,Allocator.TempJob);

      rayCommand[0] = _input;

      var jobHandle = ScheduleRaycast(_collisionWorld, rayCommand, rayResult);
      jobHandle.Complete();
      _hit = rayResult[0];
      
      rayCommand.Dispose();
      rayResult.Dispose();
      
   }

   public static JobHandle ScheduleRaycast(CollisionWorld _collisionWorld, NativeArray<RaycastInput> _raycastInputs,
      NativeArray<RaycastHit> _hits)
   {
      var rcj = new RaycastJob
      {
         m_collisionWorld = _collisionWorld,
         m_inputs =  _raycastInputs,
         m_results = _hits
      };

      JobHandle jobHandle = rcj.Schedule(_raycastInputs.Length, 5);
      return jobHandle;
   }

  
}


