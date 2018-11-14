﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class PartitionManager : MonoBehaviour {
    public float startLineY;
    public float finishLineY;

    public float removeLineY;

    public float badOffsetY;
    public float goodOffsetY;
    public float perfectOffsetY;

    public float PartitionSize;
    public float PartitionSpace;


    public enum Rank { PERFECT, GOOD, BAD, MISS };
    public static readonly Color[] trackColor = { Color.green, Color.red, Color.blue, Color.yellow };

    //Get instance
    private static PartitionManager instance;
    public static PartitionManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PartitionManager>();
            if (instance == null)
                Debug.Log("No PartitionManager found");
            return instance;
        }
    }
    private void Start()
    {
        LoadPlayer();
    }

    //Load Players
    public void LoadPlayer()
    {
        //PlayerManager.GetPlayer();
    }
}
