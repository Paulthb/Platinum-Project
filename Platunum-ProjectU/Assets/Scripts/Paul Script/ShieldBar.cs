using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : BarUI {
    private bool isArmor = true;

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

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("space"))
        {
            TakeDamage(10);
        }
        //pour les tests
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            TakeDamage(-10);
        }
        BarUpdate();
    }

    public void WinArmor(int armorPt)
    {
        Value += armorPt;
        if (Value > MaxValue) //récuperer la somme de armor groupe max
        {
            Value = MaxValue;
        }
    }

    public void TakeDamage(int damagePt)
    {
        Value -= damagePt;
        if (Value < 0)
        {
            HealthBar.Instance.TakeDamage(Mathf.Abs(Value));
            Value = 0;
        }
        else if(Value > MaxValue)
        {
            Value = MaxValue;
        }
        UpdateIsArmor();
    }



    private void UpdateIsArmor()
    {
        if (Value <= 0)
            isArmor = false;
        else
            isArmor = true;
    }
}
