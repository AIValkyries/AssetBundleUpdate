#if !NO_RUNTIME
using System;
using ProtoBuf.Meta;

#if FEAT_IKVM
using Type = IKVM.Reflection.Type;
using IKVM.Reflection;
#else
using System.Reflection;
#endif

public class PropertyGetMethod
{
    public static object GetValueNew(PropertyInfo prop, object attr)
    {
        MethodInfo methodInfo = prop.GetGetMethod();
        if (methodInfo != null)
        {
            return methodInfo.Invoke(attr, null);
        }
        else
        {
            if (prop.PropertyType.Name != "Boolean")
            {
                UnityEngine.Debug.LogError("GetGetMethod property type is:" + prop.GetType().Name);
            }
            return false;
        }
    }
}
#endif