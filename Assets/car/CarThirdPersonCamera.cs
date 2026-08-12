using UnityEngine;

public class CarThirdPersonCamera : MonoBehaviour {

    public Transform target;

    public float distance = 6f;
    public float height = 3f;

    public float positionSpeed = 8f;
    public float rotationSpeed = 8f;

    void LateUpdate() {

        if (target == null) {
            return;
        }

        // 只取汽车在水平面上的朝向
        // 避免车上坡/翻滚时摄像机也跟着翻
        var forward = target.forward;
        forward.y = 0;

        if (forward.sqrMagnitude < 0.001f) {
            forward = Vector3.forward;
        }

        forward.Normalize();

        // 摄像机目标位置：
        // 汽车后面 distance
        // 上面 height
        var targetPosition =
            target.position
            - forward * distance
            + Vector3.up * height;

        // 平滑跟随位置
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            positionSpeed * Time.deltaTime
        );

        // 看向汽车稍微靠上的位置
        var lookTarget =
            target.position + Vector3.up * 1f;

        var targetRotation =
            Quaternion.LookRotation(
                lookTarget - transform.position,
                Vector3.up
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}