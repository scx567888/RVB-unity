using System.Collections.Generic;
using UnityEngine;

public class NormalTest : MonoBehaviour {

    // 预制体
    public GameObject cube;

    private List<GameObject> list = new();

    // 显示
    public void Show() {
        gameObject.SetActive(true);
    }

    // 隐藏
    public void DisShow() {
        gameObject.SetActive(false);
    }

    void Start() {
        for (int j = 0; j < 10000*3 ; j++) {
            var obj = Instantiate(cube);
            obj.transform.position = new Vector3(RandomFloat(-10f, 10f), RandomFloat(-10f, 10f), RandomFloat(-10f, 10f));
            obj.transform.SetParent(transform, false); // 保持局部变换
            list.Add(obj);
        }
    }

    void Update() {

        // 绕 Y 轴旋转节点
        var euler = transform.eulerAngles;
        euler.y += 10f * Time.deltaTime;
        transform.eulerAngles = euler;

        // 更新每个实例的位置
        for (int j = 0; j < list.Count; j++) {
            list[j].transform.localPosition = new Vector3(RandomFloat(-30f, 30f), RandomFloat(-30f, 30f), RandomFloat(-30f, 30f));
        }

    }

    private static float RandomFloat(float min, float max) {
        return Random.Range(min, max);
    }
    
}