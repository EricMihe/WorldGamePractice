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

        BuffMgr.Instance.AddVariable<float>("a", 0);
        BuffMgr.Instance.AddVariable<Vector3>("a", Vector3 .zero );

    }

    IEnumerator enumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds (0.5f);
            Debug.Log($"{BuffMgr.Instance.GetValue<float>("a")}");
            Debug.Log($"{BuffMgr.Instance.GetValue<Vector3>("a")}");
        }
    }


    private void CheckAfterBuff()
    {

    }


    private void Update()
    {
        //speed = BuffMgr.Instance.GetFloatValue("AttackPower");
        speed = BuffMgr.Instance.GetValue<float>("a");
        currenttransform = BuffMgr.Instance.GetValue<Vector3>("a");
        transform.position += Vector3.left * speed * Time.deltaTime;
        transform.position += currenttransform * Time.deltaTime;
        if (Input.GetKeyDown (KeyCode.A))
        {
            BuffMgr.Instance.ApplyBuff("a",10f, 2f);

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BuffMgr.Instance.ApplyBuff( "a",Vector3.forward * 5, 2f);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {

        }
        if (Input.GetKeyDown(KeyCode.S))
        {

        }
        if (Input.GetKeyDown(KeyCode.D))
        {

        }
    }
}