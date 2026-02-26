using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VariableBuffExample : MonoBehaviour
{
    int speedname;
    float speed=0f;
    Vector3 currenttransform;
    CharacterController controller;
    IEnumerator enumerator()
    {
        while (true)
        {
            Debug.Log($"{speed}");
            yield return new WaitForSeconds(0.2f);
           
        }
    }
    private void Start()
    {
        //controller=GetComponent<CharacterController>();

        //s = BuffMgr.Instance.AddValue(Vector3.zero );
        ////BuffMgr.Instance.ApplyBuffWithDIY(s, 10f, new List<(float, Vector3, BuffCurveTypeDIY)>() {(1f,new Vector3 (3,0,1), BuffCurveTypeDIY.LineAve ), (3f, new Vector3 (-1,0,1), BuffCurveTypeDIY.LineAve)
        // //   ,(5f, new Vector3 (0,0,1), BuffCurveTypeDIY.LineAve),(7f, new Vector3 (-2,0,1), BuffCurveTypeDIY.LineOut), (11f, new Vector3 (3,0,1), BuffCurveTypeDIY.LineIn)});

        // BuffMgr.Instance.AddBuffWithDIY(s, 5f, new List<(float, Vector3, BuffCurveTypeDIY)>() {(3f,new Vector3 (-5,0,1), BuffCurveTypeDIY.LineOut  ), (5f, new Vector3 (0,0,1), BuffCurveTypeDIY.LineAve)});
        StartCoroutine(enumerator());
        speedname = BuffMgr.Instance.AddValue(0f, (a) => { speed = a; });
       // BuffMgr.Instance.AddBuffWithDIY <float>(speedname,)

    }
    int s;
    float x;
    Vector3 move;

    private void Update()
    {



        if (Input.GetKeyDown(KeyCode.Space))
        {
            //  BuffMgr.Instance.ApplyBuff( "av", new Vector3 (2,2,2), 2f,BuffCurveType.LinearDecay,BuffStackType.Basic ,BuffOperationType.Additive) ;
            BuffMgr.Instance.ApplyBuff<float>(speedname,10f,5f, BuffCurveType.LinearGrowth);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           // BuffMgr.Instance.ReMoveAllBuff<Vector3>("av");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
    }
}