﻿using UnityEngine;
using System.Collections.Generic;

public static class ExtensionsGameObject {
    public static GameObject InstantiateAtPosition ( this GameObject prefab, Vector3 position, bool isActive = true ) {
        GameObject marker = MonoBehaviour.Instantiate<GameObject>(prefab);
        marker.transform.position = position;
        marker.transform.rotation = Quaternion.identity;
        marker.SetActive(isActive);
        return marker;
    }

    public static GameObject InstantiateAtPositionToParent ( this GameObject prefab, Transform parent, Vector3 position, bool isActive = true ) {
        GameObject marker = MonoBehaviour.Instantiate<GameObject>(prefab);
        marker.transform.parent = parent;
        marker.transform.position = position;
        marker.transform.rotation = Quaternion.identity;
        marker.SetActive(isActive);
        return marker;
    }

    public static GameObject Closest ( this GameObject self, Vector3 pos, List<GameObject> entities ) {
        GameObject closest = null;
        float dMin = Mathf.Infinity;
        int i = 0;
        while (i < entities.Count) {
            GameObject curr = entities[i];
            Debug.Log("checking for closest: ", curr);
            if (curr == self) continue;
            float dSqr = (curr.transform.position - pos).sqrMagnitude;
            if (dSqr < dMin) {
                dMin = dSqr;
                closest = curr;
                Debug.Log("updating closest: ", curr);
            }
            i++;
        }
        return closest;
    }
}