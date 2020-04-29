using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 WithX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }

    public static Vector3 WithY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }

    private static Transform _findInChildren(Transform trans, string name)
    {
        if (trans.name == name)
            return trans;
        else
        {
            Transform found;

            for (int i = 0; i < trans.childCount; i++)
            {
                found = _findInChildren(trans.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

    public static Transform FindInChildren(this Transform trans, string name)
    {
        return _findInChildren(trans, name);
    }
}
