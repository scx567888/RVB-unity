using UnityEngine;

public class WheelColliderTest : MonoBehaviour {

    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL;
    public WheelCollider RR;

    public GameObject FLObject;
    public GameObject FRObject;
    public GameObject RLObject;
    public GameObject RRObject;

    public float motorTorque = 1500f;
    public float steerAngle = 30f;
    public float brakeTorque = 3000f;

    void FixedUpdate() {

        float motor = 0;
        float steer = 0;

        if (Input.GetKey(KeyCode.W))
            motor = motorTorque;

        if (Input.GetKey(KeyCode.S))
            motor = -motorTorque;

        if (Input.GetKey(KeyCode.A))
            steer = -steerAngle;

        if (Input.GetKey(KeyCode.D))
            steer = steerAngle;

        RL.motorTorque = motor;
        RR.motorTorque = motor;

        FL.steerAngle = steer;
        FR.steerAngle = steer;

        float brake =
            Input.GetKey(KeyCode.Space)
                ? brakeTorque
                : 0;

        FL.brakeTorque = brake;
        FR.brakeTorque = brake;
        RL.brakeTorque = brake;
        RR.brakeTorque = brake;
    }

    void Update() {
        syncWheel(FL, FLObject);
        syncWheel(FR, FRObject);
        syncWheel(RL, RLObject);
        syncWheel(RR, RRObject);
    }

    void syncWheel(
        WheelCollider wheel,
        GameObject obj
    ) {
        wheel.GetWorldPose(
            out var position,
            out var rotation
        );

        obj.transform.position = position;
        // 修正 Cylinder 自身轴向
        obj.transform.rotation =
            rotation * Quaternion.Euler(0, 0, 90);
    }
}