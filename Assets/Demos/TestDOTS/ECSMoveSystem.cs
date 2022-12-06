using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[DisableAutoCreation]
public class ECSMoveSystem : SystemBase {
    
    private Mesh m_mesh;
    private Material m_material;

    private EntityManager m_entityManager;
    private EntityArchetype m_archetype;

    private Entity[] m_entities;

    protected override void OnCreate() {
        base.OnCreate();

        World world = World.DefaultGameObjectInjectionWorld;
        m_entityManager = world.EntityManager;

        m_archetype = m_entityManager.CreateArchetype(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<RenderMesh>(),
            ComponentType.ReadWrite<RenderBounds>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<Rotation>(),
            ComponentType.ReadOnly<ECSMoveData>()
        );
    }

    public void CreateAllEntity(int maxCount, float randomNum, Mesh mesh, Material material) {
        m_mesh = mesh;
        m_material = material;
        
        
        m_entities = new Entity[maxCount];
        for (int i = 0; i < maxCount; i++) {
            float x = Random.value * randomNum * 2 - randomNum;
            float y = Random.value * randomNum * 2 - randomNum;
            float z = Random.value * randomNum * 2 - randomNum;
            m_entities[i] = CreateEntity(x, y, z);
        }
    }

    private Entity CreateEntity(float x, float y, float z) {
        Entity entity = m_entityManager.CreateEntity(m_archetype);
        m_entityManager.SetComponentData(entity, new LocalToWorld() {
            Value = new float4x4(rotation:quaternion.identity, translation:new float3(0, 0, 0))
        });
        m_entityManager.SetSharedComponentData(entity, new RenderMesh() {
            mesh = m_mesh,
            material = m_material
        });
        m_entityManager.SetComponentData(entity, new RenderBounds() {
            Value = new AABB() {
                Center = new float3(0, 0, 0),
                Extents = new float3(0.5f, 0.5f, 0.5f)
            }
        });
        m_entityManager.SetComponentData(entity, new Translation() {
            Value = new float3(x, y, z)
        });
        m_entityManager.SetComponentData(entity, new Rotation() {
            Value = quaternion.identity
        });
        float moveDistance = Random.Range(0.5f, 1f) * 5;
        m_entityManager.SetComponentData(entity, new ECSMoveData() {
            rotateSpeed = new float3(Random.value * math.PI * 2, Random.value * math.PI * 2, Random.value * math.PI * 2),
            moveSpeed = new float3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f)),
            moveDistance = moveDistance,
            origin = new float3(x, y, z),
            distanceSqr = moveDistance * moveDistance
        });
        return entity;
    }

    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref ECSMoveData moveData, ref Translation translation, ref Rotation rotation) => {
            rotation.Value = math.mul(math.normalize(rotation.Value),
                quaternion.AxisAngle(moveData.rotateSpeed, deltaTime));

            translation.Value += new float3(moveData.moveSpeed.x * deltaTime, moveData.moveSpeed.y * deltaTime,
                moveData.moveSpeed.z * deltaTime);

            if (math.distancesq(translation.Value, moveData.origin) > moveData.distanceSqr) {
                moveData.moveSpeed = -moveData.moveSpeed;
            }
        }).Schedule();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        for (int i = 0; i < m_entities.Length; i++) {
            m_entityManager.DestroyEntity(m_entities[i]);
        }
    }
}