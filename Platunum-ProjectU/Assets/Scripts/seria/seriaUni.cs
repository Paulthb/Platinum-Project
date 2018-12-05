using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class seriaUni : MonoBehaviour {

    public static seriaUni Instance;
    SerialPort Serial = new SerialPort("COM3", 9600);

    public int bt1;
    public int bt2;
    public int bt3;
    public int bt4;
    public int bt5;

    void Start()
    {
        StartCoroutine(delais());
    }

    // Update is called once per frame
    void Update()
    {
        if (!Serial.IsOpen)
        {
            Serial.Open();
        }
        string[] val = Serial.ReadLine().Split(',');
        for (int i = 0; i < val.Length; i++)
        {
             Debug.Log("i "+i +"  val "+ val[i]);
        }
        
        bt1 = int.Parse(val[0]);
        bt2 = int.Parse(val[1]);
        bt3 = int.Parse(val[2]);
        bt4 = int.Parse(val[3]);
        bt5 = int.Parse(val[4]);

        if (bt5 == 0)
        {

        }
        if (bt4 == 0)
        {
            
        }
        if (bt3 == 0)
        {
            
        }
        if (bt2 == 0)
        {
           
        }
        if (bt1 == 0)
        {
         
        }
        Debug.Log( " " + bt1 + " " + bt2 + " " + bt3 + " " + bt4);
    
    }

    private void FixedUpdate()
    {

    }

    IEnumerator delais()
    {
        yield return new WaitForSeconds(1);
    }
   
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}

