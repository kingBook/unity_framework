using UnityEngine;
using System.Collections;

public static class PhysicsUtil {

    public static readonly RaycastHit[] RaycastHits = new RaycastHit[32];

    /// <summary>
    /// 返回离射线原点最近的 RaycastHit,如果没有找到将返回 new RaycastHit()，使用 RaycastHit.collider==null 来判断是否查询到碰撞器
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="ignoreBodies">忽略检测的刚体列表</param>
    /// <param name="layerMask">用于射线计算的LayerMask，如：LayerMask.GetMask("ItemModel")。</param>
    /// <param name="queryTriggerInteraction">定义是否查询isTrigger的碰撞器</param>
    /// <returns></returns>
    public static RaycastHit GetClosestRayCastHitNonAlloc (Ray ray, Rigidbody[] ignoreBodies = null, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal) {
        RaycastHit result = new RaycastHit();
        int count = Physics.RaycastNonAlloc(ray, RaycastHits, Mathf.Infinity, layerMask, queryTriggerInteraction);
        if (count > 0) {
            float minDistance = float.MaxValue;
            for (int i = 0; i < count; i++) {
                RaycastHit hit = RaycastHits[i];
                if (ignoreBodies != null && System.Array.IndexOf(ignoreBodies, hit.rigidbody) > -1) continue;
                if (hit.distance < minDistance) {
                    minDistance = hit.distance;
                    result = hit;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 返回射线投射到一个世界坐标包围盒的碰撞信息。
    /// <br>RaycastHit.collider!=null表示射线与包围盒发生碰撞。</br>
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="worldBounds">世界坐标包围盒</param>
    /// <returns></returns>
    public static RaycastHit GetRaycastBounds (Ray ray, Bounds worldBounds) {
        var gameObject = new GameObject();
        gameObject.transform.position = worldBounds.center;
        var boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = worldBounds.size;
        RaycastHit hitInfo;
        boxCollider.Raycast(ray, out hitInfo, Mathf.Infinity);
        Object.Destroy(gameObject);
        return hitInfo;
    }

    /// <summary>
    /// 返回包围盒与碰撞器是否相交
    /// </summary>
    /// <param name="bounds">包围盒</param>
    /// <param name="collider">碰撞器</param>
    /// <returns></returns>
    public static bool GetBoundsIntersectsCollider<T> (Bounds bounds, T collider) where T : Collider {
        return bounds.Intersects(collider.bounds);
    }

    /// <summary>
    /// 返回包围盒与碰撞器列表是否有相交
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bounds">包围盒</param>
    /// <param name="colliders">碰撞器列表</param>
    /// <returns></returns>
    public static bool GetBoundsIntersectsColliders<T> (Bounds bounds, T[] colliders) where T : Collider {
        int len = colliders.Length;
        for (int i = 0; i < len; i++) {
            if (GetBoundsIntersectsCollider(bounds, colliders[i])) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取由原点抛物运动至目标点所需要的线性速度（此方法未完善）
    /// </summary>
    /// <param name="targetPosition"> 目标点 </param>
    /// <param name="source"> 原点 </param>
    /// <param name="angle"> 抛物的角度 </param>
    /// <returns></returns>
    public static Vector3 GetBallisticVelocity (Vector3 targetPosition, Transform source, float angle) {
        Quaternion rotationRecord = source.rotation;

        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(source.position.x, 0.0f, source.position.z);
        Vector3 targetXZPos = new Vector3(targetPosition.x, 0.0f, targetPosition.z);

        // rotate the object to face the target
        source.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(angle * Mathf.Deg2Rad);
        float H = targetPosition.y - source.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = source.TransformDirection(localVelocity);

        source.rotation = rotationRecord;

        return globalVelocity;
    }

    /// <summary>
    /// 忽略物理接触列表的所有碰撞，此方法适用于 OnCollisionEnter (Collision collision) 消息函数内
    /// </summary>
    /// <param name="contacts"> 物理接触列表 </param>
    public static void IgnoreContactsCollision (ContactPoint[] contacts) {
        for (int i = 0, len = contacts.Length; i < len; i++) {
            ContactPoint contact = contacts[i];
            Collider thisCollider = contact.thisCollider;
            Collider otherCollider = contact.otherCollider;
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }

}
