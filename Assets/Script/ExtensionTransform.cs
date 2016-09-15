using UnityEngine;

public static class ExtensionTransform {

    public static GameObject GetNext ( this Transform t ) {
        return t.parent.GetChild ( t.GetSiblingIndex () + 1 ).gameObject;
    }

    public static GameObject GetPrevious ( this Transform t ) {
        return t.parent.GetChild ( t.GetSiblingIndex () - 1 ).gameObject;
    }

    public static GameObject GetNextActive ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            GameObject curr = t.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy ) {
                return curr;
            }
        }
        return null;
    }

    public static GameObject GetNextInactive ( this Transform t ) {
        for ( int i = 0; i < t.childCount; i++ ) {
            GameObject curr = t.GetChild ( i ).gameObject;
            if ( curr.activeInHierarchy == false ) {
                return curr;
            }
        }
        return null;
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
}
