﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour {
    //Value
    private float CooldownTimer;
    private float TempValue;
    private float OldValue;
    protected float Value;

    //UI
    private Image Temp;
    private Image Current;

    //Settings
    [Header("Settings")]
    public bool StartEmpty;
    public bool ResetCooldown;
    public float PercentagePerSecond;
    public float CooldownBar;
    public float MaxValue;


    // Use this for initialization
    protected void BarStart () {
        if (!StartEmpty)
        {
            Value = MaxValue;
            OldValue = MaxValue;
            TempValue = MaxValue;
        }
        else
        {
            Value = 0;
            OldValue = 0;
            TempValue = 0;
        }
        CooldownTimer = 0;
        Temp = transform.GetChild(0).GetComponent<Image>();
        Current = transform.GetChild(1).GetComponent<Image>();
    }
	
	// Update is called once per frame
	protected void BarUpdate () {
        Current.fillAmount = Value / MaxValue;

        //Si current
        if (Value < OldValue)
        {
            if (CooldownTimer >= CooldownBar)
            {
                if (OldValue > Value)
                {
                    //speed -= TimeToApply * Time.deltaTime;
                    //TempValue = Mathf.Lerp(Value, OldValue, speed);
                    //Debug.Log(TempValue - buffer - Value);
                    TempValue -= (PercentagePerSecond/100*MaxValue) * Time.deltaTime;
                    if (TempValue < Value)
                        TempValue = Value;
                }
            }
            else
            {
                CooldownTimer += Time.deltaTime;
            }
        }
        else if (Value > OldValue)
        {
            /*speed += (TimeToApply* 3) * Time.deltaTime;
            float fillAmount = Mathf.Lerp(OldValue, Value, speed - 1);
            TempValue = fillAmount;
            Current.fillAmount = TempValue / MaxValue;*/
            TempValue += (PercentagePerSecond / 100 * MaxValue) * Time.deltaTime;
            if (TempValue > Value)
                TempValue = Value;

            Current.fillAmount = TempValue / MaxValue;
        }
        Temp.fillAmount = TempValue / MaxValue;
        if (Value != OldValue)
        {
            if (Mathf.Abs(Value - TempValue) < 0.1f)
            {
                OldValue = Value;
                TempValue = Value;
                CooldownTimer = 0;
                //buffer = 0;
            }
        }
    }

    protected void ResetCooldownTimer()
    {
        if(CooldownTimer < CooldownBar)
            CooldownTimer = 0;
    }

    protected void AddToValue(float value)
    {
        this.Value += value;
        if (ResetCooldown)
            ResetCooldownTimer();
    }

    protected void SoustractToValue(float value)
    {
        this.Value -= value;
        if (ResetCooldown)
            ResetCooldownTimer();
    }

    protected void SetValue(float value)
    {
        this.Value = value;
    }

    public float GetValue()
    {
        return Value;
    }
    /*
    protected void AddToBuffer(float value)
    {
        if (Value != TempValue)
        {
            Debug.Log("addtobuffer");
            if(TempValue + buffer + value > MaxValue)
            {
                buffer += Mathf.Abs((TempValue + buffer + value) - MaxValue);
            }
            else
            {
                if (buffer + value > 0)
                    buffer += value;
                else
                    buffer = 0;
            }
        }
        else if(TempValue < Value)
        {
            buffer = MaxValue - TempValue;
        }
    }
    */
}
