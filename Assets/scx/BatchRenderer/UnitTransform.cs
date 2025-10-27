using UnityEngine;

// 移植完成
public class UnitTransform {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool visible;

    public UnitTransform() {
        this.position = new Vector3(0, 0, 0);
        this.rotation = new Quaternion(0, 0, 0, 1);
        this.scale = new Vector3(1, 1, 1);
        this.visible = false;
    }

}