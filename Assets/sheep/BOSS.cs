using UnityEngine;

public class BOSS : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    [Header("活动范围")]
    public float minX = -50f;
    public float maxX = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;

    private Vector3 targetPosition;
    private float fixedY;

    void Start()
    {
        // 保持方块开始时的高度不变
        fixedY = transform.position.y;

        SetNewTarget();
    }

    void Update()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        // 平滑移动到目标位置
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 朝移动方向旋转
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        // 接近目标点后，重新选择一个随机目标
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            SetNewTarget();
        }
    }

    public float edgePadding = 2f;

    void SetNewTarget()
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0:
                randomX = minX + edgePadding;
                break;

            case 1:
                randomX = maxX - edgePadding;
                break;

            case 2:
                randomZ = minZ + edgePadding;
                break;

            case 3:
                randomZ = maxZ - edgePadding;
                break;
        }

        targetPosition = new Vector3(randomX, fixedY, randomZ);
    }
}