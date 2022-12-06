using Unity.Entities;
using Unity.Mathematics;

public struct ECSMoveData : IComponentData {
    
    public float3 rotateSpeed;
    public float3 moveSpeed;
    public float moveDistance;

    public float3 origin;
    public float distanceSqr;
}