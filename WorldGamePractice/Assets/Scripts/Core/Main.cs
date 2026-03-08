using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Transform player;
    public Transform mainCamera;
    public bool isOpenInputMgr=true;

    //BaseCharacter character;
    //AddVector3Temp addVector3Temp=new AddVector3Temp();

    RelatedObject<RelateValue_Test> r1;
    RelateTrigger relateTrigger = new RelateTrigger();
    //RelatedObject<RelateValue_Bool> relatedObject = new RelatedObject<RelateValue_Bool>(new RelateValue_Bool() { isopen = false });
    // Start is called before the first frame update
    void Start()
    {
        // 锁定鼠标到屏幕中心，并隐藏光标
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        InputMgr.Instance.StartOrCloseInputMgr(isOpenInputMgr);

        var s = new RelateTrigger();
        relateTrigger.AddRelate(s, (a, b) =>
        {
            Debug.Log("111111111");
        });

        relateTrigger.AddRelate(s, (a, b) =>
        {
            Debug.Log("222222222");
        });

        relateTrigger.AddProceed((a) =>
        {
            Debug.Log("aaaaaaaaaaa");
        });

        //relateTrigger.AddRelate(relatedObject, new RelateValue_Bool() { isopen = false }, (a, b) =>
        //{
        //    Debug.Log("333333333");
        //    b.newValue .isopen = true;
        //    b.UpdateLate();
        //});

        ////relatedObject.AddRelate(new RelateValue_Bool() { isopen = true }, relateTrigger, (a, b) =>
        ////{
        ////    Debug.Log("44444444");
        ////    a.newValue.isopen = false;
        ////    a.UpdateLate();
        ////    b.UpdateLate();
        ////});

        //relatedObject.AddProceed(new RelateValue_Bool() { isopen = true }, (a) =>
        //{
        //    Debug.Log("44444444");
        //    a.newValue.isopen = false;
        //    a.UpdateLate();
        //});

        r1 = new RelatedObject<RelateValue_Test>(new RelateValue_Test(), ICompare .Any, relateTrigger, (a, b) =>
        {
            if (a is RelatedObject<RelateValue_Test> _a)
            {
                Debug.Log("5555555555");
                _a.newData.a = 0;
                _a.newData.s = "000";
                _a.RefreshLate();
            }
            b.RefreshLate();
        });

        //r1.AddProceed(new RelateValue_Test() { a = 0, s = "000" }, (a) =>
        //{
        //    Debug.Log("666666666");
        //    a.newValue.a = 1;
        //    a.newValue.s = "111";
        //    a.UpdateLate();
        //});



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            relateTrigger.Refresh();
        }
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    relateTrigger.RemoveRelate(relatedObject);
        //}
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            r1.newData.a = 1;
            r1.newData.s = "111";
            r1.Refresh();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            r1.newData.a = 2;
            r1.newData.s = "111";
            r1.Refresh();
        }


    }

    private void OnDisable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }
}
