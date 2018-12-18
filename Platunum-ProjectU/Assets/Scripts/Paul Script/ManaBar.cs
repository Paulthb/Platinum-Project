using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : BarUI {
    public Text debugText;
    public bool debug;
    public float ManaPerAttack = 50f;
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
        debugText.text = Value + "/" + MaxValue;
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
        debugText.gameObject.SetActive(debug);
    }

    public void WinMana(int manaPt)
    {
        AddToValue(manaPt * HarmonieBar.Instance.GetMultiplier());
        if (Value > MaxValue) //récuperer la somme de mana groupe max
        {
            Value = MaxValue;
        }
    }

    public void Attack()
    {
        SoustractToValue(ManaPerAttack);
        if (base.Value < 0)
        {
            SetValue(0);
        }

        //UpdateBar();
    }
}
