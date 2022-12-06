using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaypointSystem : MonoBehaviour {

    private CinemachineSmoothPath[] m_cinemachinePaths;
    private WaypointSystem m_waypointSystem;
    private WaypointObject[] m_results;


    private void Awake() {
        m_cinemachinePaths = GetComponentsInChildren<CinemachineSmoothPath>();
        m_results = new WaypointObject[2048];
    }

    private void Start() {
        WaypointPath[] paths = WaypointSystemHelper.ConvertCinemachinePaths(m_cinemachinePaths);
        m_waypointSystem = new WaypointSystem(paths);

    }

    private void Update() {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        int count = m_waypointSystem.FindPathNonAlloc((2, 15), (9, 0), m_results);
        WaypointSystemDebug.DrawFindPathResults(count, m_results, Color.red, 0.1f);

        sw.Stop();
        Debug.Log($"FindPath time:{sw.ElapsedMilliseconds}, count:{count}");
    }
}
