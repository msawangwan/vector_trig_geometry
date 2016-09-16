using UnityEngine;
using System.Collections.Generic;

public static class ExtensionsGameObject {

    /* TODO: research how to constarin by gameobject -- is it even possible? */
    public static GameObject InstantiateAtPosition<T> (this GameObject prefab, Vector3 position, Quaternion rotation, bool isActive = true) 
            where T : Object {
        GameObject instance = MonoBehaviour.Instantiate<GameObject>(prefab);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(isActive);
        return instance;
    }

    /* TODO: research how to constarin by gameobject -- is it even possible? */
    public static GameObject InstantiateAtPositionWithParent<T> ( this GameObject prefab, Transform parentTransform, Vector3 position, bool isActive = true ) 
            where T : Object {
        GameObject instance = MonoBehaviour.Instantiate<GameObject>(prefab);
        instance.transform.parent = parentTransform;
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;
        instance.SetActive(isActive);
        return instance;
    }

    /* instantiates a pool of gameobjects -- is it even possible? */
    public static void InstantiatePool<T> ( this GameObject prefab, Transform poolTransform, int poolSize, bool isActive = true ) 
            where T : Object {
        for (int i = 0; i < poolSize; i++) {
            Transform instance = MonoBehaviour.Instantiate<GameObject> ( prefab ).transform;
            instance.gameObject.SetActive ( isActive );
            instance.SetParent ( poolTransform );
            instance.position = Vector3.zero;
            instance.rotation = Quaternion.identity;
        }
    }

    /* returns an allocated reference array of the pooled objects */
    public static GameObject[] InstantiatePoolAllocArr<T> ( this GameObject prefab, Transform poolTransform, int poolSize, bool isActive = true ) 
            where T : Object {
        GameObject[] alloc = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++) {
            GameObject instance = MonoBehaviour.Instantiate<GameObject> ( prefab );
            instance.SetActive ( isActive );
            instance.transform.SetParent ( poolTransform );
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            alloc[i] = instance;
        }
        return alloc;
    }

    public static GameObject Closest ( this GameObject self, List<GameObject> entities ) {
        GameObject closest = null;
        float dMin = Mathf.Infinity;
        int i = 0;
        while (i < entities.Count) {
            GameObject curr = entities[i];
            if (curr == self) {
                Debug.Log("skip self");
            } else {
                float dToCurrSqr = (curr.transform.position - self.transform.position).sqrMagnitude;
                if (dToCurrSqr < dMin) {
                    dMin = dToCurrSqr;
                    closest = curr;
                }
            }
            i++;
        }
        return closest;
    }
}
