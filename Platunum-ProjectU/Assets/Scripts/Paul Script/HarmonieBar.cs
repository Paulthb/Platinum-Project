﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : BarUI {
    private int multiplier = 1;
    private bool emptying = false;
    private float Timer = 0f;
    public float multiplierDuration = 20f;
    public Text multiplierText;
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
            GiveHarmonie(10f);
        }

        if (Input.GetKeyDown("x"))
        {
            GiveHarmonie(10f);
        }
        if (emptying)
        {
            SoustractToValue((MaxValue / multiplierDuration) * Time.deltaTime);
            if (Value <= 0)
            {
                emptying = false;
                multiplier = 1;
                multiplierText.enabled = false;
            }
        }
        BarUpdate();
    }

    public void GiveHarmonie(float harmoniePt)
    {
        if (!emptying)
        {
            AddToValue(harmoniePt);
            if (Value >= MaxValue)
            {
                SetValue(MaxValue);
                TriggerMultiplier();
            }
        }
    }

    private void TriggerMultiplier()
    {
        multiplier = 2;
        emptying = true;
        multiplierText.enabled = true;
    }

    public int GetMultiplier()
    {
        return multiplier;
    }
}
