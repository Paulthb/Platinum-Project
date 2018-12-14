﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : BarUI {


    private static HarmonieBar instance;
    public static HarmonieBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<HarmonieBar>();
            if (instance == null)
                Debug.Log("No HarmonieBar found");
            return instance;
        }
    }


    // Use this for initialization
    void Start ()
    {
        BarStart();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("x"))
        {
            TakeHarmonie(10f);
        }

        if (Input.GetKeyDown("x"))
        {
            TakeHarmonie(10f);
        }
        BarUpdate();
    }

    public void TakeHarmonie(float harmoniePt)
    {

        Value += harmoniePt;
        if (Value >= 100)
        {
            Value = 100;
        }
    }
}
