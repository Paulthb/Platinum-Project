﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class gamepads
{
    public int portNum;
    SerialPort Serial;
    public int[] btn;
    public bool[] btnReleased;

    public gamepads(int portNum, int speed = 9600)
    {
        btn = new int[5];
        for (int i = 0; i < btn.Length; i++)
            btn[i] = 1;
        btnReleased = new bool[5];
        for (int i = 0; i < btnReleased.Length; i++)
            btnReleased[i] = true;
        Serial = new SerialPort("COM"+portNum, speed);
        //Serial.ReadTimeout = 10;//50
        this.portNum = portNum; 
        //StartCoroutine(delais());
    }

    // Update is called once per frame
    public void Update()
    {
        if (!Serial.IsOpen)
        {
            Serial.Open();
        }
        try
        {
            String[] val = Serial.ReadLine().Split(',');
            
            for (int i = 1; i < val.Length; i++)
            {
                //if (i == 4)
                //    Debug.Log("pads n°" + portNum + "/ Value:" + val[i]);
                btn[i-1] = int.Parse(val[i]);
                Debug.Log((val[i]));
            }
        }
        catch(TimeoutException e)
        {
            for (int i = 0; i < btn.Length; i++)
            {
                btn[i] = 1;
            }
        }
    }

    public bool GetKeyDown(int id)
    {
        Debug.Log(btn[id]);
        if(btn[id] == 0)
        {
            if(btnReleased[id] == true)
            {
                btnReleased[id] = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            btnReleased[id] = true;
            return false;
        }
    }

    public bool GetKey(int id)
    {
        if (btn[id] == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetLed(int ledId)
    {
        Serial.Write(ledId.ToString());
        
    }
}