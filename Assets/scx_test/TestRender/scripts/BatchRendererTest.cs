using System.Diagnostics;
using UnityEngine;

public class BatchRendererTest : MonoBehaviour {

    // 预制体
    public GameObject cube;

    private MeshMergeBatchRenderer batchRenderer;

    // 显示/隐藏
    public void Show() {
        gameObject.SetActive(true);
    }

    public void DisShow() {
        gameObject.SetActive(false);
    }

    void Start() {
        batchRenderer = BatchRendererBuilder.createByPrefab(10000*3, cube);
        batchRenderer.setParent(gameObject);
        for (int j = 0; j < batchRenderer.capacity(); j++) {
            batchRenderer.setUnitVisible(j, true);
            batchRenderer.setUnitPosition(j, RandomFloat(-100, 100), RandomFloat(-100, 100), RandomFloat(-100, 100));
        }
        batchRenderer.update();
    }

    void Update() {
        // 绕 Y 轴旋转整个节点
        var euler = transform.eulerAngles;
        euler.y += 10f * Time.deltaTime;
        transform.eulerAngles = euler;

        for (int j = 0; j < batchRenderer.capacity(); j++) {
            batchRenderer.setUnitPosition(j, RandomFloat(-30,30), RandomFloat(-30,30), RandomFloat(-30,30));
        }

        batchRenderer.update();
    }

    public static int RandomFloat(int min, int max) {
        return Random.Range(min, max);
    }
    
}