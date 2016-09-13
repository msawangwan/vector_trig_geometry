using UnityEngine;

public static class ExtensionTransform {

    public static GameObject GetNext ( this Transform t ) {
        return t.parent.GetChild ( t.GetSiblingIndex () + 1 ).gameObject;
    }
}
