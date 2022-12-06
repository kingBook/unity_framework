using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class SimpleECSCreateScript : MonoBehaviour {

    public Mesh mesh;
    public Material material;

    private void Start() {
        // 获取默认的World和EntityManager
        World world = World.DefaultGameObjectInjectionWorld;
        EntityManager entityManager = world.EntityManager;

        // 创建Cube需要的所有Component对应的类型集合
        EntityArchetype archetype = entityManager.CreateArchetype(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<RenderMesh>(),
            ComponentType.ReadOnly<RenderBounds>()
        );

        // 创建Entity
        Entity entity = entityManager.CreateEntity(archetype);
        // 为Entity设置数据
        entityManager.SetComponentData(entity, new LocalToWorld() {
            Value = new float4x4(rotation:quaternion.identity, translation:new float3(0, 0, 0))
        });
        entityManager.SetSharedComponentData(entity, new RenderMesh() {
            mesh = mesh,
            material = material
        });
        entityManager.SetComponentData(entity, new RenderBounds() {
            Value = new AABB() {
                Center = new float3(0, 0, 0),
                Extents = new float3(0.5f, 0.5f, 0.5f)
            }
        });


    }

}