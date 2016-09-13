using UnityEngine;

public class PlayerAssetContainer : MonoBehaviour {

    public static PlayerAssetContainer s = null;

    void Awake () {
        s = this;
        DontDestroyOnLoad(gameObject);
    }
}
