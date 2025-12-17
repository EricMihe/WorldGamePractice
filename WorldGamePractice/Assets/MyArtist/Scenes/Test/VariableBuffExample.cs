using System.Collections;
using TreeEditor;
using UnityEngine;

public class VariableBuffExample : MonoBehaviour
{
    float speed;
    Vector3 currenttransform;
    IEnumerator enumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log($"{BuffMgr.Instance.GetValue<float>("a")}");
            Debug.Log($"{BuffMgr.Instance.GetValue<Vector3>("a")}");
        }
    }
    private void Start()
    {
        StartCoroutine(enumerator());

        BuffMgr.Instance.AddValue("a", 0);
        BuffMgr.Instance.AddValue("a", Vector3 .zero );

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