using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointObject {
    public ushort pathIndex;
    public ushort wayPointIndex;
    public Vector3 position;
    /// <summary> 0 到 65,535</summary>
    public ushort overlapPointsCount;
    /// <summary> 与当前点重叠的点列表（包含当前点），注意：1).所有重叠点必须引用同一个; 2).长度范围 0 到 65,535 </summary>
    public List<WaypointObject> overlapPoints;

    #region 寻路时设置
    public WaypointObject parent;
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    #endregion

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override bool Equals(object other) {
        if (!(other is WaypointObject)) {
            return false;
        }
        return CompareWaypointObject(this, (WaypointObject)other);
    }

    private static bool CompareWaypointObject(WaypointObject a, WaypointObject b) {
        bool flagA = (object)a == null;
        bool flagB = (object)b == null;
        if (flagA && flagB) {
            return true;
        }
        if (flagA || flagB) {
            return false;
        }
        if (a.pathIndex == b.pathIndex && a.wayPointIndex == b.wayPointIndex) return true;
        // 是否共点
        if (a.overlapPointsCount > 0 && b.overlapPointsCount > 0 && a.overlapPointsCount == b.overlapPointsCount && a.overlapPoints == b.overlapPoints) return true;
        return false;
    }

    public override string ToString() {
        return string.Format("WaypointObject{{ pathIndex:{0}, wayPointIndex:{1}, position:{2}, overlapPointsCount:{3} }}", pathIndex, wayPointIndex, position, overlapPointsCount);
    }

    public static bool operator ==(WaypointObject a, WaypointObject b) {
        return CompareWaypointObject(a, b);
    }

    public static bool operator !=(WaypointObject a, WaypointObject b) {
        return !CompareWaypointObject(a, b);
    }

}
