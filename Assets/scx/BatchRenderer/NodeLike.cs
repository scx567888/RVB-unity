using System;
using UnityEngine;

// 移植完成
public interface NodeLike {
    
    void setParent(GameObject parent);

    GameObject getParent();

    void setPosition(float x, float y, float z);

    Vector3 getPosition();

    void setRotation(float x, float y, float z, float w);

    void setRotationFromEuler(float x, float y, float z);

    Quaternion getRotation();

    void setScale(float x, float y, float z);

    Vector3 getScale();

    void setActive(bool active);

    bool getActive();

    void setLayer(String name);
    
    String getLayer();

    void destroy();
    
}