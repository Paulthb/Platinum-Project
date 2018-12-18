using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : BarUI {

    private static ShieldBar instance;
    public static ShieldBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ShieldBar>();
            if (instance == null)
                Debug.Log("No health found");
            return instance;
        }
    }

    private void Start()
    {
        BarStart();
    }

    void Update()
    {
        //pour les tests
        if (Input.GetKey(KeyCode.LeftControl))
        {
            TakeDamage(10);
        }
        //pour les tests
        if (Input.GetKey(KeyCode.LeftShift))
        {
            TakeDamage(-10);
        }
        BarUpdate();
    }

    public void WinArmor(int armorPt)
    {
        AddToValue(armorPt * HarmonieBar.Instance.GetMultiplier());
        if (Value > MaxValue) //récuperer la somme de armor groupe max
        {
            SetValue(MaxValue);
        }

    }

    public void TakeDamage(int damagePt)
    {
        SoustractToValue(damagePt);
        if (Value < 0)
        {
            HealthBar.Instance.TakeDamage(Mathf.Abs(Value));
            SetValue(0);
        }
        else if(Value > MaxValue)
        {
            SetValue(MaxValue);
        }
    }
}
