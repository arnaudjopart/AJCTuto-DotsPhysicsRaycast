
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine;
using Material = UnityEngine.Material;
using RaycastHit = Unity.Physics.RaycastHit;

public class MouseRaycast : MonoBehaviour
{
    public Camera m_camera;
    public float m_raycastMaxDistance;
    private EntityManager m_entityManager;
    public int m_belongsTo;
    public int m_collidesWith;
    public Material m_material;
    public Material m_defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = m_camera.ScreenPointToRay(Input.mousePosition);

        var hitEntity = Raycast(ray.origin, ray.direction*m_raycastMaxDistance, m_belongsTo, m_collidesWith);

        if (!m_entityManager.Exists(hitEntity)) return;
        Debug.Log(hitEntity.Index);
        if (!m_entityManager.HasComponent<HighlightComponent>(hitEntity)) return;
        
        var highlightComponent = m_entityManager.GetComponentData<HighlightComponent>(hitEntity);
        var mesh = m_entityManager.GetSharedComponentData<RenderMesh>(hitEntity);
        highlightComponent.m_isHighlighted = !highlightComponent.m_isHighlighted;
        var material = highlightComponent.m_isHighlighted ? m_material: m_defaultMaterial;
        mesh.material = material;
        m_entityManager.SetSharedComponentData(hitEntity,mesh);
        m_entityManager.SetComponentData(hitEntity,highlightComponent);
        
        /*
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = m_camera.ScreenPointToRay(Input.mousePosition);
        
        var physicWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorldSystem = physicWorldSystem.PhysicsWorld.CollisionWorld;
        
        
        var raycastInput = new RaycastInput
        {
            Start = ray.origin,
            End = ray.direction*m_raycastMaxDistance,
            Filter = new CollisionFilter
            {
                BelongsTo = (uint)(1<<m_belongsTo),
                CollidesWith = (uint)(1<<m_collidesWith),
                GroupIndex = 0
            }
        };

        
        var singleRaycastHit = new RaycastHit( );
        RaycastManager.SingleRaycast(collisionWorldSystem, raycastInput, ref singleRaycastHit);
        if (m_entityManager.Exists(singleRaycastHit.Entity))
        {
            if (m_entityManager.HasComponent<HighlightComponent>(singleRaycastHit.Entity))
            {
                var highlightComponent = m_entityManager.GetComponentData<HighlightComponent>(singleRaycastHit.Entity);
                var mesh = m_entityManager.GetSharedComponentData<RenderMesh>(singleRaycastHit.Entity);
                highlightComponent.m_isHighlighted = !highlightComponent.m_isHighlighted;
                var material = highlightComponent.m_isHighlighted ? m_material: m_defaultMaterial;
                mesh.material = material;
                m_entityManager.SetSharedComponentData(singleRaycastHit.Entity,mesh);
                m_entityManager.SetComponentData(singleRaycastHit.Entity,highlightComponent);
                
            }
            
        }*/

    }

    private Entity Raycast(Vector3 _from, Vector3 _to, int _belongsTo, int _collidesWith)
    {
        var physicWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        var collisionWorldSystem = physicWorldSystem.PhysicsWorld.CollisionWorld;
            
        var input = new RaycastInput
        {
            Start = _from,
            End = _to,
            Filter = new CollisionFilter
            {
                BelongsTo = (uint)(1<<_belongsTo),
                CollidesWith = (uint)(1<<_collidesWith),
                GroupIndex = 0
            }
        };
        
        RaycastHit hit = new RaycastHit();
        bool haveHit = collisionWorldSystem.CastRay(input, out hit);

        if (haveHit)
        {
            Entity hitEntity = physicWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            return hitEntity;
        }
        return Entity.Null;
    }
}