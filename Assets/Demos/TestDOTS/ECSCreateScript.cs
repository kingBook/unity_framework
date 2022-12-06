using Unity.Entities;
using UnityEngine;

public class ECSCreateScript : MonoBehaviour {

    public Mesh mesh;
    public Material material;
    
    public int MaxCount = 10000;
    public float RandomNum = 10f;

    void Start() {
        World world = World.DefaultGameObjectInjectionWorld;
        ECSMoveSystem moveSystem = world.GetOrCreateSystem<ECSMoveSystem>();
        moveSystem.CreateAllEntity(MaxCount, RandomNum, mesh, material);
        SimulationSystemGroup systemGroup = world.GetOrCreateSystem<SimulationSystemGroup>();
        systemGroup.AddSystemToUpdateList(moveSystem);
    }
}