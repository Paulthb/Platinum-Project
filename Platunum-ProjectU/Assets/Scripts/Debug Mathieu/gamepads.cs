using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class gamepads
{
    public int portNum;
    SerialPort Serial;// = new SerialPort("COM3", 9600);
    public int[] btn;
    public bool[] btnReleased;

    public gamepads(int portNum, int speed = 9600)
    {
        btn = new int[5];
        btnReleased = new bool[5];
        for (int i = 0; i < btnReleased.Length; i++)
            btnReleased[i] = true;
        Serial = new SerialPort("COM"+portNum, speed);
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
        string[] val = Serial.ReadLine().Split(',');
        for (int i = 0; i < val.Length; i++)
        {
            if(i==4)
                Debug.Log("pads n°" + portNum + "/ Value:"+val[i]);
            btn[i] = int.Parse(val[i]);
        }
    }

    public bool GetKeyDown(int id)
    {
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
}