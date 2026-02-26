using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 道具飞行方式枚举
public enum ItemFlightMode
{
    DropToGround,           // 直接掉在地面
    ParabolicThrow,         // 抛物线投掷
    StraightFlight,         // 直线飞行
    CircularOrbit,          // 环绕挂点
    RotateAroundPoint,      // 围绕轴旋转
    TrackTarget,            // 跟踪目标
    CurveTrackTarget,       // 曲线跟踪目标
    AttachToTarget          // 附着到目标
}

// 道具交互类型枚举
public enum ItemInteractionType
{
    OnSpawn,                // 生成时
    OnHitUnit,              // 碰到单位
    OnHitSpecificUnit,      // 碰到特定单位
    OnDestroy               // 消失时
}

// 道具配置类
[System.Serializable]
public class ItemConfig
{
    public string itemName = "Default Item";
    public ItemFlightMode flightMode = ItemFlightMode.DropToGround;
    public float lifeTime = 5f;                    // 存活时间
    public int maxHitCount = 5;                    // 最大碰撞次数
    public float flightSpeed = 5f;                 // 飞行速度
    public float rotationSpeed = 30f;              // 旋转速度
    public float orbitRadius = 2f;                 // 环绕半径
    public Vector3 offset = Vector3.zero;          // 偏移位置
    public Vector3 rotationOffset = Vector3.zero;  // 旋转偏移
    public bool useGravity = false;                // 是否使用重力
    public float gravityStrength = 9.81f;          // 重力强度
    public float trackDuration = 3f;               // 跟踪持续时间
    public float curveTrackSpeed = 2f;             // 曲线跟踪速度
    public LayerMask interactionLayers = -1;       // 交互层
    public string[] specificTags = new string[0];  // 特定标签
    public bool enableSelfRotation = false;        // 是否启用自转
    public Vector3 selfRotationAxis = Vector3.up;  // 自转轴
    public float selfRotationSpeed = 60f;          // 自转速度
    public float detectionRadius = 0.5f;           // 检测半径
}

// 道具对象类
public class ItemObject : MonoBehaviour
{

    public ItemConfig config;
    public Transform spawnPoint;
    public int currentHitCount= 0;
    public float currentLifeTime= 0f;
    public bool isDestroyed= false;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Transform targetTransform;
    private Vector3 orbitCenter;
    private float orbitAngle = 0f;
    private float rotationAngle = 0f;
    private float trackTimer = 0f;
    private Vector3 velocity;
    private Vector3 gravityVelocity;
    private bool isOnGround = false;

    // 事件委托定义
    public Action<ItemObject> onSpawn;
    public Action<ItemObject, Collider> onHitUnit;
    public Action<ItemObject, Collider> onHitSpecificUnit;
    public Action<ItemObject> onDestroy;

    public void Initialize(ItemConfig itemConfig, Transform spawnTransform)
    {
        config = itemConfig;
        currentHitCount = 0;
        currentLifeTime = 0f;
        isDestroyed = false;
        isOnGround = false;
        if(spawnTransform != null)
        {
            spawnPoint = spawnTransform;
            // 设置初始位置和旋转
            transform.position = spawnTransform.position + spawnTransform.TransformDirection(config.offset);
            transform.rotation = spawnTransform.rotation * Quaternion.Euler(config.rotationOffset);
            initialPosition = transform.position;

        }
        else
        {
            Debug.LogError("没有挂点");
        }

        // 根据飞行模式初始化
        InitializeFlightMode();

        // 调用生成时的交互逻辑
        onSpawn?.Invoke(this);
    }

    // 初始化飞行模式
    private void InitializeFlightMode()
    {
        switch (config.flightMode)
        {
            case ItemFlightMode.DropToGround:
                velocity = Vector3.zero;
                gravityVelocity = Vector3.zero;
                break;

            case ItemFlightMode.ParabolicThrow:
                velocity = spawnPoint.forward * config.flightSpeed;
                gravityVelocity = Vector3.zero;
                break;

            case ItemFlightMode.StraightFlight:
                velocity = spawnPoint.forward * config.flightSpeed;
                break;

            case ItemFlightMode.CircularOrbit:
                orbitCenter = spawnPoint.position;
                orbitAngle = 0f;
                break;

            case ItemFlightMode.RotateAroundPoint:
                orbitCenter = spawnPoint.position;
                orbitAngle = 0f;
                rotationAngle = 0f;
                break;

            case ItemFlightMode.TrackTarget:
                trackTimer = 0f;
                break;

            case ItemFlightMode.CurveTrackTarget:
                trackTimer = 0f;
                break;

            case ItemFlightMode.AttachToTarget:
                // 附着模式下，位置会在Update中更新
                break;
        }
    }

    // 设置跟踪目标
    public void SetTarget(Transform target)
    {
        targetTransform = target;
        targetPosition = target.position;
    }

    private void Update()
    {
        if (isDestroyed) return;

        // 更新存活时间
        currentLifeTime += Time.deltaTime;

        // 检查是否超过存活时间
        if (currentLifeTime >= config.lifeTime)
        {
            DestroyItem();
            return;
        }

        // 执行飞行逻辑
        ExecuteFlightLogic();

        // 执行自转逻辑
        if (config.enableSelfRotation)
        {
            transform.Rotate(config.selfRotationAxis, config.selfRotationSpeed * Time.deltaTime);
        }

        // 检测碰撞
        DetectCollisions();
    }

    // 执行飞行逻辑
    private void ExecuteFlightLogic()
    {
        switch (config.flightMode)
        {
            case ItemFlightMode.DropToGround:
                ExecuteDropToGround();
                break;

            case ItemFlightMode.ParabolicThrow:
                ExecuteParabolicThrow();
                break;

            case ItemFlightMode.StraightFlight:
                ExecuteStraightFlight();
                break;

            case ItemFlightMode.CircularOrbit:
                ExecuteCircularOrbit();
                break;

            case ItemFlightMode.RotateAroundPoint:
                ExecuteRotateAroundPoint();
                break;

            case ItemFlightMode.TrackTarget:
                ExecuteTrackTarget();
                break;

            case ItemFlightMode.CurveTrackTarget:
                ExecuteCurveTrackTarget();
                break;

            case ItemFlightMode.AttachToTarget:
                ExecuteAttachToTarget();
                break;
        }
    }

    // 执行掉落逻辑
    private void ExecuteDropToGround()
    {
        if (isOnGround) return;

        if (config.useGravity)
        {
            gravityVelocity.y -= config.gravityStrength * Time.deltaTime;
            transform.position += gravityVelocity * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * config.flightSpeed * Time.deltaTime;
        }

        // 检测是否着地
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f))
        {
            isOnGround = true;
        }
    }

    // 执行抛物线投掷逻辑
    private void ExecuteParabolicThrow()
    {
        if (config.useGravity)
        {
            gravityVelocity.y -= config.gravityStrength * Time.deltaTime;
            transform.position += velocity * Time.deltaTime + gravityVelocity * Time.deltaTime;
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    // 执行直线飞行逻辑
    private void ExecuteStraightFlight()
    {
        transform.position += spawnPoint.forward * config.flightSpeed * Time.deltaTime;
    }

    // 执行环绕逻辑
    private void ExecuteCircularOrbit()
    {
        orbitAngle += config.flightSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(
            Mathf.Cos(orbitAngle) * config.orbitRadius,
            0,
            Mathf.Sin(orbitAngle) * config.orbitRadius
        );
        transform.position = orbitCenter + offset;
    }

    // 执行围绕轴旋转逻辑
    private void ExecuteRotateAroundPoint()
    {
        orbitAngle += config.flightSpeed * Time.deltaTime;
        rotationAngle += config.rotationSpeed * Time.deltaTime;

        Vector3 orbitOffset = new Vector3(
            Mathf.Cos(orbitAngle) * config.orbitRadius,
            Mathf.Sin(rotationAngle) * config.orbitRadius,
            Mathf.Sin(orbitAngle) * config.orbitRadius
        );
        transform.position = orbitCenter + orbitOffset;
    }

    // 执行目标跟踪逻辑
    private void ExecuteTrackTarget()
    {
        if (targetTransform == null) return;

        trackTimer += Time.deltaTime;
        if (trackTimer >= config.trackDuration) return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * config.flightSpeed * Time.deltaTime;
    }

    // 执行曲线跟踪逻辑
    private void ExecuteCurveTrackTarget()
    {
        if (targetTransform == null) return;

        trackTimer += Time.deltaTime;
        if (trackTimer >= config.trackDuration) return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;
        // 添加曲线效果
        Vector3 curveOffset = new Vector3(
            Mathf.Sin(trackTimer * config.curveTrackSpeed) * 0.5f,
            Mathf.Cos(trackTimer * config.curveTrackSpeed) * 0.3f,
            0
        );
        direction += curveOffset.normalized * 0.2f;
        transform.position += direction.normalized * config.flightSpeed * Time.deltaTime;
    }

    // 执行附着到目标逻辑
    private void ExecuteAttachToTarget()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position + targetTransform.TransformDirection(config.offset);
        transform.rotation = targetTransform.rotation * Quaternion.Euler(config.rotationOffset);
    }

    // 检测碰撞
    private void DetectCollisions()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, config.detectionRadius, config.interactionLayers);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == gameObject) continue; // 忽略自身

            // 检查是否是特定标签的单位
            bool isSpecificUnit = false;
            foreach (string tag in config.specificTags)
            {
                if (collider.CompareTag(tag))
                {
                    isSpecificUnit = true;
                    break;
                }
            }

            // 增加碰撞计数
            currentHitCount++;

            // 调用碰撞回调
            onHitUnit?.Invoke(this, collider);
            if (isSpecificUnit)
            {
                onHitSpecificUnit?.Invoke(this, collider);
            }

            // 检查是否达到最大碰撞次数
            if (currentHitCount >= config.maxHitCount)
            {
                DestroyItem();
                return;
            }
        }
    }

    // 销毁道具
    public void DestroyItem()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // 调用销毁时的交互逻辑
        onDestroy?.Invoke(this);

        // 销毁游戏对象
        Destroy(gameObject);
    }

    // 绘制Gizmos用于调试
    private void OnDrawGizmos()
    {
        if (config == null) return;

        // 绘制检测半径
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, config.detectionRadius);

        // 绘制环绕半径
        if (config.flightMode == ItemFlightMode.CircularOrbit ||
            config.flightMode == ItemFlightMode.RotateAroundPoint)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(orbitCenter, config.orbitRadius);
        }

        // 绘制初始位置
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(initialPosition, 0.1f);
        }
    }
}
