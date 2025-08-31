using System.Collections;
using UnityEngine;
using OrderType = WaypointPath.OrderType;

public class AdditionalPathData : MonoBehaviour {

    [SerializeField] private OrderType m_orderType;

    public OrderType orderType => m_orderType;

}