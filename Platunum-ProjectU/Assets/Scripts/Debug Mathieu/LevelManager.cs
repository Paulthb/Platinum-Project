using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public float startLineY;
    public float finishLineY;

    public float removeLineY;

    public float badOffsetY;
    public float goodOffsetY;
    public float perfectOffsetY;

    public Color[] trackColor;

    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<LevelManager>();
            if (instance == null)
                Debug.Log("No LevelManager found");
            return instance;
        }
    }
}
