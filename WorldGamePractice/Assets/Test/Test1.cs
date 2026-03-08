using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public Transform spawnPoint; // 挂点位置
    //public ItemConfig[] itemConfigs; // 道具配置数组

    //void CreateParabolicItem()
    //{
    //    ItemConfig config = new ItemConfig
    //    {
    //        itemName = "ParabolicItem",
    //        flightMode = ItemFlightMode.ParabolicThrow,
    //        lifeTime = 10f,
    //        maxHitCount = 3,
    //        flightSpeed = 10f,
    //        useGravity = true,
    //        gravityStrength = 9.81f,
    //        detectionRadius = 0.5f,
    //        interactionLayers = LayerMask.GetMask("Default"),
    //        enableSelfRotation = true,
    //        selfRotationAxis = Vector3.up,
    //        selfRotationSpeed = 120f
    //    };

    //    ItemObject item = ItemSystemManager.Instance.CreateItem(config, spawnPoint);

    //    // 设置交互逻辑
    //    item.onSpawn += (itemObj) =>
    //    {
    //        Debug.Log($"道具 {itemObj.config.itemName} 已生成");
    //    };

    //    item.onHitUnit += (itemObj, collider) =>
    //    {
    //        Debug.Log($"道具 {itemObj.config.itemName} 碰到单位: {collider.name}");
    //    };

    //    item.onDestroy += (itemObj) =>
    //    {
    //        Debug.Log($"道具 {itemObj.config.itemName} 已销毁");
    //    };
    //}

    //void CreateOrbitItem()
    //{
    //    ItemConfig config = new ItemConfig
    //    {
    //        itemName = "OrbitItem",
    //        flightMode = ItemFlightMode.CircularOrbit,
    //        lifeTime = 15f,
    //        maxHitCount = 1,
    //        flightSpeed = 2f,
    //        orbitRadius = 3f,
    //        detectionRadius = 0.3f,
    //        interactionLayers = LayerMask.GetMask("Default")
    //    };

    //    ItemObject item = ItemSystemManager.Instance.CreateItem(config, spawnPoint);
    //}

    //void CreateTrackingItem()
    //{
    //    // 假设场景中有一个名为"Target"的游戏对象作为跟踪目标
    //    Transform target = GameObject.Find("Target")?.transform;
    //    if (target == null)
    //    {
    //        // 如果没有找到目标，创建一个示例目标
    //        GameObject targetGO = new GameObject("Target");
    //        targetGO.transform.position = spawnPoint.position + Vector3.right * 5f + Vector3.forward * 3f;
    //        target = targetGO.transform;
    //    }

    //    ItemConfig config = new ItemConfig
    //    {
    //        itemName = "TrackingItem",
    //        flightMode = ItemFlightMode.TrackTarget,
    //        lifeTime = 8f,
    //        maxHitCount = 2,
    //        flightSpeed = 5f,
    //        trackDuration = 5f,
    //        detectionRadius = 0.4f,
    //        interactionLayers = LayerMask.GetMask("Default"),
    //        specificTags = new string[] { "Player", "Enemy" }
    //    };

    //    ItemObject item = ItemSystemManager.Instance.CreateItem(config, spawnPoint);
    //    item.SetTarget(target);
    //}
}

//// 道具系统管理器
//public class ItemSystemManager : MonoBehaviour
//{
//    public static ItemSystemManager Instance { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    // 创建道具
//    public ItemObject CreateItem(ItemConfig config, Transform spawnPoint)
//    {
//        GameObject itemGO = new GameObject(config.itemName);
//        ItemObject item = itemGO.AddComponent<ItemObject>();
//        item.Initialize(config, spawnPoint);
//        return item;
//    }
//}
