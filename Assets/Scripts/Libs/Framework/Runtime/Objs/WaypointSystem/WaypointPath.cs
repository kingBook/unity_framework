using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath {

    public enum OrderType {
        /// <summary> 双向 </summary>
        TwoWay = 1,
        /// <summary> 小->大 (只能由最小索引向最大索引的方向行走)</summary>
        OneWayAZ = 2,
        /// <summary> 大->小 (只能由最大索引向最小索引的方向行走)</summary>
        OneWayZA = 4
    }

    private List<WaypointObject> m_waypoints;
    private bool m_isClose;
    private OrderType m_orderType;

    public List<WaypointObject> waypoints => m_waypoints;
    public bool isClose => m_isClose;
    public OrderType orderType => m_orderType;

    public WaypointPath(List<WaypointObject> waypoints, bool isClose, OrderType waypointOrder) {
        m_waypoints = waypoints;
        m_isClose = isClose;
        m_orderType = waypointOrder;
    }


}
