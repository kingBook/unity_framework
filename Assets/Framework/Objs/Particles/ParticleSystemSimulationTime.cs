#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定一个时间，让粒子系统从该时间的状态开始模拟
/// </summary>
public class ParticleSystemSimulationTime : MonoBehaviour {

    [Tooltip("模拟粒子的时间（秒）")] public float time = 5;

    private ParticleSystem m_particleSystem;

    private void Awake () {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start () {
        m_particleSystem.Simulate(time, true, true);
        m_particleSystem.Play();
    }
}
