using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MagicFlameVFX{

public class TestDemo : MonoBehaviour
{
    string[] vfxInfo = null;

    private int i = 0;
    private GameObject now;
    public Transform vfxParent;

    public void Awake()
    {
        vfxInfo = new string[vfxParent.childCount];
        for (int j = 0; j < vfxParent.childCount; j++)
        {
            vfxInfo[j] = vfxParent.GetChild(j).gameObject.name;
        }

        now = GameObject.Instantiate(vfxParent.transform.Find(vfxInfo[i]).gameObject);
        now.transform.parent = null;
        now.transform.localPosition = Vector3.zero;
    }

    public void OnLeftClick()
    {
        i--;
        if (i < 0)
        {
            i = vfxInfo.Length - 1;
        }
        if (now != null)
        {
            GameObject.DestroyImmediate(now);
        }
        now = GameObject.Instantiate(vfxParent.transform.Find(vfxInfo[i]).gameObject);
        now.transform.parent = null;
        now.transform.localPosition = Vector3.zero;
    }

    public void OnRightClick()
    {
        i++;
        if (i >= vfxInfo.Length)
        {
            i = 0;
        }
        if (now != null)
        {
            GameObject.DestroyImmediate(now);
        }
        now = GameObject.Instantiate(vfxParent.transform.Find(vfxInfo[i]).gameObject);
        now.transform.parent = null;
        now.transform.localPosition = Vector3.zero;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnLeftClick();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnRightClick();
        }
    }

    public void RePlay()
    {
        if (now != null)
        {
            now.SetActive(false);
            now.SetActive(true);
        }
    }


}

}