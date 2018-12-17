﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : BarUI {

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("c"))
        {
            ResetCooldownTimer();
            WinMana(-50);
        }

        //pour les tests
        if (Input.GetKeyDown("d"))
        {
            WinMana(50);
        }
        BarUpdate();
    }

    private static ManaBar instance;
    public static ManaBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ManaBar>();
            if (instance == null)
                Debug.Log("No ManaBar found");
            return instance;
        }
    }


    void Start()
    {
        instance = ManaBar.Instance;
        BarStart();
    }

    public void WinMana(int manaPt)
    {
        base.Value += manaPt;
        if (Value > MaxValue) //récuperer la somme de mana groupe max
        {
            Value = MaxValue;
        }
    }

    public void Attack()
    {
        
        ResetCooldownTimer();
        Value -= 50;
        if (base.Value < 0)
        {
            Value = 0;
        }

        //UpdateBar();
    }
}
