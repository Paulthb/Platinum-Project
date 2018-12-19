using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : BarUI {
    // valeurs temporaires, il faut récup en fontion des stats des joueurs
    private static HealthBar instance;
    public static HealthBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<HealthBar>();
            if (instance == null)
                Debug.Log("No health found");
            return instance;
        }
    }

	// Use this for initialization
	void Start ()
    {
        BarStart();
    }

    // Update is called once per frame

    void Update()
    {
        BarUpdate();
    }

    public void TakeDamage(float damagePt)
    {
        SoustractToValue(damagePt);
        if (Value < 0 && !BarManager.Instance.endGame)
        {
            SetValue(0);
            BarManager.Instance.EndGame();
        }
        else if(Value > MaxValue)
        {
            SetValue(MaxValue);
        }
    }
}
