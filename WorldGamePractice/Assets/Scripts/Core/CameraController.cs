
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 相机控制模式枚举
/// </summary>
public enum CameraMode
{
    Orbit,
    FreeLook,
    SpecificPoint
}

/// <summary>
/// 高级第三人称相机控制器
/// 支持：环绕 + 阻尼跟随 + 碰撞检测 + 动态 FOV + 镜头震动
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    // ====== 基础设置 ======
    public CameraMode currentMode = CameraMode.Orbit;
    public Transform targetPoint;

    [Header("环绕模式参数")]
    public float orbitFollowSpeed = 50f;
    public float orbitRotateSpeed = 5f;
    public float initialDistance = 10f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("阻尼跟随")]
    public bool useDampedOrbitTarget = true;
    public float orbitTargetDampSmoothTime = 0.15f;

    [Header("自由视角参数")]
    public float freeLookRotateSpeed = 2f;
    public float freeLookMoveSpeed = 10f;

    [Header("碰撞检测")]
    public bool enableCollision = true;
    public LayerMask collisionLayers = ~0; // 默认排除所有层，建议设为环境层
    public float sphereCastRadius = 0.2f;

    [Header("动态 FOV")]
    public bool enableDynamicFOV = false;
    public float baseFOV = 60f;
    public float maxFOV = 80f;
    public float fovSensitivity = 5f; // 距离越远，FOV 越大

    [Header("镜头震动")]
    public float shakeMagnitude = 0.2f;
    public float shakeDuration = 0.3f;
    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    // ====== 内部变量 ======
    private Camera _camera;
    private float currentDistance;
    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 orbitTargetDampVelocity = Vector3.zero;
    private Vector3 dampedTargetPosition;
    private bool isAnimating = false;
    private Transform originalParent;
    private Animation targetAnimation;

    // 输入缓存
    private Vector3 mouseDelta;
    private float scrollInput;

    void Start()
    {
        _camera = GetComponent<Camera>();
        originalLocalPosition = transform.localPosition;

        currentDistance = initialDistance;
        currentRotation = transform.eulerAngles;
        originalParent = transform.parent;

        if (targetPoint != null)
        {
            dampedTargetPosition = targetPoint.position;
            if (enableDynamicFOV) _camera.fieldOfView = baseFOV;
        }
        else
        {
            dampedTargetPosition = transform.position;
        }

        // 初始化旋转
        currentRotation.x = transform.eulerAngles.x;
        currentRotation.y = transform.eulerAngles.y;
    }

    private void HandleOrbitMode()
    {
        if (targetPoint == null)
        {
            Debug.LogWarning("目标挂点为空，请设置targetPoint");
            return;
        }

        // === 1. 更新阻尼目标点 ===
        if (useDampedOrbitTarget)
        {
            dampedTargetPosition = Vector3.SmoothDamp(
                dampedTargetPosition,
                targetPoint.position,
                ref orbitTargetDampVelocity,
                orbitTargetDampSmoothTime
            );
        }
        else
        {
            dampedTargetPosition = targetPoint.position;
        }

        // === 2. 处理输入 ===
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        currentRotation.y += mouseDelta.x * orbitRotateSpeed;
        currentRotation.x -= mouseDelta.y * orbitRotateSpeed;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -89f, 89f);

        initialDistance -= scrollInput * 5f * orbitRotateSpeed;
        initialDistance = Mathf.Clamp(initialDistance, minDistance, maxDistance);

        // === 3. 计算理想位置（无碰撞）===
        Vector3 idealPosition = CalculateOrbitPosition(dampedTargetPosition);

        // === 4. 【碰撞检测】从目标点向理想相机位置发射球形射线 ===
        Vector3 finalPosition = idealPosition;
        if (enableCollision && Physics.SphereCast(
                dampedTargetPosition,
                sphereCastRadius,
                (idealPosition - dampedTargetPosition).normalized,
                out RaycastHit hit,
                Vector3.Distance(dampedTargetPosition, idealPosition),
                collisionLayers))
        {
            // 如果撞到物体，把相机放在碰撞点前方一点
            finalPosition = hit.point + hit.normal * sphereCastRadius * 1.1f;
        }

        // === 5. 平滑移动到最终位置 ===
        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPosition,
            ref smoothVelocity,
            1f / orbitFollowSpeed
        );

        // === 6. 朝向目标点 ===
        transform.LookAt(dampedTargetPosition);

        // === 7. 动态 FOV ===
        if (enableDynamicFOV)
        {
            float distance = Vector3.Distance(transform.position, dampedTargetPosition);
            float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            _camera.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, t * fovSensitivity);
        }

        currentDistance = Vector3.Distance(transform.position, dampedTargetPosition);
    }

    private Vector3 CalculateOrbitPosition(Vector3 center)
    {
        Quaternion rot = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 offset = rot * Vector3.forward * (-initialDistance);
        return center + offset;
    }

    // ====== 自由视角模式 ======
    private void HandleFreeLookMode()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        bool hasMouseInput = Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f;
        if (hasMouseInput)
        {
            currentRotation.y += mouseX * freeLookRotateSpeed;
            currentRotation.x -= mouseY * freeLookRotateSpeed;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -89f, 89f);
            transform.eulerAngles = currentRotation;
        }

        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            transform.Translate(Vector3.forward * scrollInput * freeLookMoveSpeed, Space.Self);
        }
    }

    // ====== 特定挂点模式 ======
    private void HandleSpecificPointMode()
    {
        if (targetPoint == null) return;

        transform.position = targetPoint.position;
        transform.SetParent(targetPoint);

        if (targetAnimation != null && !isAnimating)
        {
            targetAnimation.Play();
            isAnimating = true;
        }
    }

    // ====== 模式切换 ======
    public void SwitchCameraMode(CameraMode newMode)
    {
        if (currentMode == CameraMode.SpecificPoint)
        {
            transform.SetParent(originalParent);
            if (targetAnimation != null && isAnimating)
            {
                targetAnimation.Stop();
                isAnimating = false;
            }
        }

        currentMode = newMode;

        if (newMode == CameraMode.Orbit && targetPoint != null)
        {
            currentDistance = Vector3.Distance(transform.position, targetPoint.position);
        }
        else if (newMode == CameraMode.FreeLook)
        {
            currentRotation = transform.eulerAngles;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetPoint = newTarget;
        if (targetPoint != null)
        {
            targetAnimation = targetPoint.GetComponent<Animation>();
            dampedTargetPosition = targetPoint.position;
        }
    }

    // ====== 镜头震动 ======
    public void ShakeCamera(float magnitude = -1, float duration = -1)
    {
        if (magnitude < 0) magnitude = shakeMagnitude;
        if (duration < 0) duration = shakeDuration;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(magnitude, duration));
    }

    private IEnumerator ShakeRoutine(float magnitude, float duration)
    {
        float elapsed = 0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float strength = Mathf.Lerp(magnitude, 0, progress);

            Vector3 randomOffset = Random.insideUnitSphere * strength;
            randomOffset.z = 0; // 可选：限制2D震动（去掉则3D）

            transform.localPosition = originalPos + randomOffset;
            yield return null;
        }

        transform.localPosition = originalPos;
        shakeCoroutine = null;
    }

    // ====== 生命周期 ======
    void Update()
    {
        initialDistance = Mathf.Clamp(initialDistance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        switch (currentMode)
        {
            case CameraMode.Orbit:
                HandleOrbitMode();
                break;
            case CameraMode.FreeLook:
                HandleFreeLookMode();
                break;
            case CameraMode.SpecificPoint:
                HandleSpecificPointMode();
                break;
        }
    }

    // ====== 可视化 ======
    void OnDrawGizmosSelected()
    {
        if (currentMode == CameraMode.Orbit && targetPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPoint.position);
            if (enableCollision)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
            }
        }
    }

    // ====== 顿帧（保留原功能）=====
    Coroutine coroutine_hitlag;
    public void DOHitlag(int frame, bool lerp)
    {
        if (frame > 0 && Time.timeScale == 1)
        {
            if (coroutine_hitlag != null) StopCoroutine(coroutine_hitlag);
            coroutine_hitlag = StartCoroutine(Hitlag(frame, lerp));
        }
    }

    IEnumerator Hitlag(int frame, bool lerp)
    {
        for (int i = 0; i < frame; i++)
        {
            Time.timeScale = lerp ? Mathf.Lerp(1, 0, (float)i / frame) : 0;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1;
        coroutine_hitlag = null;
    }
}


