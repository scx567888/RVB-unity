using UnityEngine;

namespace common {
    public class FreeCameraController : MonoBehaviour {
        [Header("移动设置")] [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float fastMoveMultiplier = 3f;
        [SerializeField] private float scrollSpeedStep = 1f;
        [SerializeField] private float minMoveSpeed = 1f;
        [SerializeField] private float maxMoveSpeed = 50f;

        [Header("视角设置")] [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minPitch = -89f;
        [SerializeField] private float maxPitch = 89f;

        private float yaw;
        private float pitch;

        private void Start() {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;

            if (pitch > 180f) {
                pitch -= 360f;
            }
        }

        private void Update() {
            HandleRotation();
            HandleMovement();
            HandleSpeedAdjustment();
        }

        private void HandleRotation() {
            if (Input.GetMouseButtonDown(1)) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetMouseButtonUp(1)) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (!Input.GetMouseButton(1)) {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void HandleMovement() {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            float verticalMovement = 0f;

            if (Input.GetKey(KeyCode.E)) {
                verticalMovement += 1f;
            }

            if (Input.GetKey(KeyCode.Q)) {
                verticalMovement -= 1f;
            }

            Vector3 direction =
                transform.right * horizontal +
                transform.forward * vertical +
                Vector3.up * verticalMovement;

            if (direction.sqrMagnitude > 1f) {
                direction.Normalize();
            }

            float currentSpeed = moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift)) {
                currentSpeed *= fastMoveMultiplier;
            }

            transform.position += direction * currentSpeed * Time.unscaledDeltaTime;
        }

        private void HandleSpeedAdjustment() {
            float scroll = Input.mouseScrollDelta.y;

            if (Mathf.Abs(scroll) > 0.01f) {
                moveSpeed += scroll * scrollSpeedStep;
                moveSpeed = Mathf.Clamp(moveSpeed, minMoveSpeed, maxMoveSpeed);
            }
        }
    }
}