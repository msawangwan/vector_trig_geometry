using UnityEngine;

public static class ExtensionTransform {

    public static GameObject GetNextSibling ( this Transform t ) {
        return t.parent.GetChild ( t.GetSiblingIndex () + 1 ).gameObject;
    }

    public static GameObject GetPreviousSibling ( this Transform t ) {
        return t.parent.GetChild ( t.GetSiblingIndex () - 1 ).gameObject;
    }

    public static GameObject GetNextActiveSibling ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            GameObject curr = t.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy ) {
                return curr;
            }
        }
        return null;
    }

    public static GameObject GetNextInactiveSibling ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            GameObject curr = t.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy == false ) {
                return curr;
            }
        }
        return null;
    }

    public static int GetNextSiblingIndex ( this Transform t ) {
        return t.GetSiblingIndex () + 1;
    }

    public static int GetPreviousSiblingIndex ( this Transform t ) {
        return t.GetSiblingIndex () - 1; // warning: caller must verify index > 0
    }

    public static int GetNextActiveSiblingIndex ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            Transform curr = t.GetChild ( i );
            if ( curr.gameObject.activeInHierarchy ) {
                return t.GetSiblingIndex ();
            }
        }
        return -1; // warning: caller must verify index > 0
    }

    public static int GetNextInactiveSiblingIndex ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            Transform curr = t.GetChild ( i );
            if ( curr.gameObject.activeInHierarchy == false ) {
                return curr.GetSiblingIndex ();
            }
        }
        return -1; // warning: caller must verify index > 0
    }

    /* returns a Transform child count of either active (default) or inactive objects */
    public static int ChildCountActive ( this Transform t, bool countActiveOnly = true ) {
        int index = 0;
        int count = 0;
        int totalNumChildren = t.childCount;
        switch ( countActiveOnly ) {
        case false:
            while ( index < totalNumChildren ) {
                if ( t.GetChild ( index ).gameObject.activeSelf == false ) {
                    count++;
                }
                index++;
            }
            break;
        default:
            while ( index < totalNumChildren ) {
                if ( t.GetChild ( index ).gameObject.activeSelf ) {
                    count++;
                }
                index++;
            }
            break;
        }
        return count;
    }

    /* wrapper method -- usage: Transform t = t.InstantiateTransform() */
    public static Transform InstantiateTransform ( this Transform t, string name = "transform_instance" ) {
        return new GameObject ( name ).transform;
    }

    /* wrapper method -- usage: Transform t = t.InstantiateTransform(pos) */
    public static Transform InstantiateTransformAtPosition ( this Transform t, Vector3 position, string name = "transform_instance" ) {
        Transform instance = new GameObject ( name ).transform;
        instance.position = position;
        instance.rotation = Quaternion.identity;
        return instance;
    }

    /* wrapper method -- usage: Transform t = t.InstantiateTransform(parent) */
    public static Transform InstantiateTransformWithParent ( this Transform t, Transform parentTransform, string name = "transform_instance" ) {
        Transform instance = new GameObject ( name ).transform;
        instance.parent = parentTransform;
        instance.position = Vector3.zero;
        instance.rotation = Quaternion.identity;
        return instance;
    }
}
