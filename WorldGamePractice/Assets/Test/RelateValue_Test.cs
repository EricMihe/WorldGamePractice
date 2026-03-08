using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelateValue_Test : IRelatedData<RelateValue_Test>
{
    public int a = 0;
    public string s = "000";

    public bool Compare(RelateValue_Test t)
    {
        if(a == t.a
            && s == t.s)
            return true;
        else return false;
    }

    public void CopyData(RelateValue_Test t)
    {
        a = t.a;
        s = t.s;
    }
}
