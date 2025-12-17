using System.Collections;
using TreeEditor;
using UnityEngine;

public class VariableBuffExample : MonoBehaviour
{
    float speed;
    Vector3 currenttransform;
    private void Start()
    {


        //// 示例1：使用整数变量
        //ExampleIntVariables();

        //// 示例2：使用浮点数变量
        //ExampleFloatVariables();

        //// 示例3：使用Vector3变量
        //ExampleVector3Variables();

        //// 示例4：展示不同效果曲线
        //ExampleEffectCurves();

        //// 示例5：展示不同堆叠类型
        //ExampleStackTypes();

        StartCoroutine(enumerator());

        BuffMgr.Instance.AddFloatVariable("AttackPower", 0f);
        BuffMgr.Instance.AddIntVariable("currenttransform", 10);
    }

    IEnumerator enumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds (0.5f);
            Debug.Log($"{BuffMgr.Instance.GetFloatValue("AttackPower")}");
            Debug.Log($"{BuffMgr.Instance.GetIntValue("currenttransform")}");
        }
    }

    private void ExampleIntVariables()
    {
        Debug.Log("=== 整数变量示例 ===");


        // 添加一个+50的攻击加成，持续3秒，使用加法
        BuffMgr.Instance.ApplyIntBuff(
            "AttackPower",           // 变量ID
            "PowerBoost",            // 效果ID
            50,                      // 增值
            3.0f,                    // 持续时间
            BuffOperationType.Additive, // 加法运算
            EffectCurveType.Instant,    // 瞬间生效
            StackType.Basic           // 基础叠加
        );

        Debug.Log($"添加加成后攻击力: {BuffMgr.Instance.GetIntValue("AttackPower")}");

        // 3秒后查看数值
        Invoke("CheckAfterBuff", 3.5f);
    }

    private void CheckAfterBuff()
    {
        Debug.Log($"效果结束后攻击力: {BuffMgr.Instance.GetIntValue("AttackPower")}");
    }

    private void ExampleFloatVariables()
    {
        Debug.Log("=== 浮点数变量示例 ===");

        // 注册一个移动速度变量，初始值为5.0
        BuffMgr.Instance.AddFloatVariable("MoveSpeed", 5.0f);

        Debug.Log($"初始移动速度: {BuffMgr.Instance.GetFloatValue("MoveSpeed")}");

        // 添加一个+2.0的速度加成，持续2秒
        BuffMgr.Instance.ApplyFloatBuff(
            "MoveSpeed",
            "SpeedBoost",
            2.0f,
            2.0f,
            BuffOperationType.Additive,
            EffectCurveType.Instant,
            StackType.Basic,
            (effect) => Debug.Log("速度加成效果结束")
        );

        Debug.Log($"加速后移动速度: {BuffMgr.Instance.GetFloatValue("MoveSpeed")}");

        // 修改基础值
        BuffMgr.Instance.SetFloatValue("MoveSpeed", 6.0f);
        Debug.Log($"修改基础值后速度: {BuffMgr.Instance.GetFloatValue("MoveSpeed")}");
    }

    private void ExampleVector3Variables()
    {
        Debug.Log("=== Vector3变量示例 ===");

        // 注册一个位置偏移变量
        BuffMgr.Instance.AddVector3Variable("PositionOffset", Vector3.zero);

        Debug.Log($"初始位置偏移: {BuffMgr.Instance.GetVector3Value("PositionOffset")}");

        // 添加一个位置偏移效果
        BuffMgr.Instance.ApplyVector3Buff(
            "PositionOffset",
            "Knockback",
            new Vector3(0, 0, 5),
            3f,
            BuffOperationType.Additive ,
            EffectCurveType.Instant,
            StackType.Basic
        );

        Debug.Log($"添加偏移后: {BuffMgr.Instance.GetVector3Value("PositionOffset")}");
    }

    private void ExampleEffectCurves()
    {
        Debug.Log("=== 效果曲线示例 ===");

        // 注册一个测试变量
        BuffMgr.Instance.AddFloatVariable("TestCurve", 10.0f);

        // 瞬间效果
        BuffMgr.Instance.ApplyFloatBuff(
            "TestCurve",
            "InstantEffect",
            5.0f,
            2.0f,
            BuffOperationType.Additive,
            EffectCurveType.Instant
        );

        Debug.Log($"瞬间效果: {BuffMgr.Instance.GetFloatValue("TestCurve")}");

        // 线性衰减效果
        BuffMgr.Instance.ApplyFloatBuff(
            "TestCurve",
            "DecayEffect",
            8.0f,
            3.0f,
            BuffOperationType.Additive,
            EffectCurveType.LinearDecay
        );

        Debug.Log($"衰减效果开始: {BuffMgr.Instance.GetFloatValue("TestCurve")}");

        // 缓入缓出效果
        BuffMgr.Instance.ApplyFloatBuff(
            "TestCurve",
            "EaseInOutEffect",
            6.0f,
            4.0f,
            BuffOperationType.Additive,
            EffectCurveType.EaseInEaseOut
        );

        Debug.Log($"缓入缓出效果开始: {BuffMgr.Instance.GetFloatValue("TestCurve")}");
    }

    private void ExampleStackTypes()
    {
        Debug.Log("=== 堆叠类型示例 ===");

        // 基础堆叠 - 多个效果会累加
        BuffMgr.Instance.AddFloatVariable("BasicStack", 100.0f);
        BuffMgr.Instance.ApplyFloatBuff(
            "BasicStack", "Boost1", 10.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.Basic
        );
        BuffMgr.Instance.ApplyFloatBuff(
            "BasicStack", "Boost2", 20.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.Basic
        );
        Debug.Log($"基础堆叠 (100+10+20): {BuffMgr.Instance.GetFloatValue("BasicStack")}");

        // 覆盖类型 - 后来的效果会覆盖前面的
        BuffMgr.Instance.AddFloatVariable("OverrideStack", 100.0f);
        BuffMgr.Instance.ApplyFloatBuff(
            "OverrideStack", "Boost1", 10.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.Override
        );
        BuffMgr.Instance.ApplyFloatBuff(
            "OverrideStack", "Boost2", 30.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.Override
        );
        Debug.Log($"覆盖堆叠 (应为130): {BuffMgr.Instance.GetFloatValue("OverrideStack")}");

        // 最大值类型 - 只保留最大的效果
        BuffMgr.Instance.AddFloatVariable("MaxStack", 100.0f);
        BuffMgr.Instance.ApplyFloatBuff(
            "MaxStack", "SmallBoost", 10.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.MaxValue
        );
        BuffMgr.Instance.ApplyFloatBuff(
            "MaxStack", "BigBoost", 50.0f, 5.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.MaxValue
        );
        Debug.Log($"最大值堆叠 (应为150): {BuffMgr.Instance.GetFloatValue("MaxStack")}");

        // 最长时间类型 - 只保留持续时间最长的效果
        BuffMgr.Instance.AddFloatVariable("MaxDurationStack", 100.0f);
        BuffMgr.Instance.ApplyFloatBuff(
            "MaxDurationStack", "ShortEffect", 20.0f, 2.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.MaxDuration
        );
        BuffMgr.Instance.ApplyFloatBuff(
            "MaxDurationStack", "LongEffect", 15.0f, 8.0f,
            BuffOperationType.Additive, EffectCurveType.Instant, StackType.MaxDuration
        );
        Debug.Log($"最长时间堆叠 (应为115): {BuffMgr.Instance.GetFloatValue("MaxDurationStack")}");
    }

    private void Update()
    {
        //speed = BuffMgr.Instance.GetFloatValue("AttackPower");

        //transform.position += Vector3.left * speed * Time.deltaTime;
        //transform.position += currenttransform * Time.deltaTime;
        if (Input.GetKeyDown (KeyCode.A))
        {
            BuffMgr.Instance.ApplyFloatBuff(
                "AttackPower",        
                "PowerBoost",         
                20f,       
                1f,                   
                BuffOperationType.Additive ,
                EffectCurveType.LinearDecay ,  
                StackType.Basic
            );

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BuffMgr.Instance.ApplyIntBuff("currenttransform", "aa",5, 5, BuffOperationType.Additive,
                EffectCurveType.LinearDecay,
                StackType.Basic);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BuffMgr.Instance.SetFloatValue("AttackPower", 20);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            BuffMgr.Instance.ApplyIntBuff(
                "AttackPower",           // 变量ID
                "PowerBoost2",            // 效果ID
                20,                      // 增值
                15f,                    // 持续时间
                BuffOperationType.Additive, // 加法运算
                EffectCurveType.Instant,    // 瞬间生效
                StackType.Basic         // 基础叠加
            );

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            BuffMgr.Instance.ApplyIntBuff(
                "AttackPower",           // 变量ID
                "PowerBoost2",            // 效果ID
                80,                      // 增值
                3.0f,                    // 持续时间
                BuffOperationType.Additive, // 加法运算
                EffectCurveType.Instant,    // 瞬间生效
                StackType.Override          // 基础叠加
            );

        }
    }
}