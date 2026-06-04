using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    private int chosenSkinId;
    public static SkinManager instance;

    private void Awake() {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void setSkinId(int id) => chosenSkinId = id;
    public int getSkinId() => chosenSkinId;

}
