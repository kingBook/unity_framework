using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCinemachineFindClosestPoint : MonoBehaviour {

    public Transform player;
    public Transform redPoint;
    public CinemachineSmoothPath smoothPath;

    void Update() {
        // 查找 CinemachinePath 路径上距离玩家最近的点
        float playerPathUnits = smoothPath.FindClosestPoint(player.position, 0, -1, 0);
        // 将 PathUnits 转换为 DistanceUnits
        float playerDistanceUnits = smoothPath.FromPathNativeUnits(playerPathUnits, CinemachinePathBase.PositionUnits.Distance);

        Debug.Log($"playerPathUnits:{playerPathUnits}, playerDistanceUnits:{playerDistanceUnits}");
        Vector3 playerPosOnPath = smoothPath.EvaluatePosition(playerPathUnits); // 玩家在路径上的世界坐标

        redPoint.position = playerPosOnPath;
    }
}
