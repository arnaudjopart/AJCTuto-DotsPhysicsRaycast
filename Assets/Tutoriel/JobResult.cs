using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class JobResult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = 10;
        var b = 15;

        NativeArray<float> result = new NativeArray<float>(1,Allocator.TempJob);
        var job = new AddJob
        {
            m_a = a,
            m_b = b,
            m_result = result
        };

        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();
        
        print(result[0]);
        result.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct AddJob : IJob
{
    public float m_a;
    public float m_b;

    public NativeArray<float> m_result;
    public void Execute()
    {
        var result = m_a + m_b;
        m_result[0] = result;
    }
}

