using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObjEx
{
    public static object TryCastToRelatedObjectOfElementType(object listObj, object elementObj)
    {
        if (listObj == null || elementObj == null)
            return null;

        Type elementType = elementObj.GetType();                    
        Type expectedListType = typeof(RelatedObject<>).MakeGenericType(elementType); 

        if (listObj.GetType() == expectedListType)
        {
            return listObj; 
        }

        return null; 
    }
}
