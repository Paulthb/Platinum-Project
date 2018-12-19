using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : BarUI {
    public float damage = 50;
    private BossManager bossManager;
    public Transform back;

    private static BossBar instance;
    public static BossBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<BossBar>();
            if (instance == null)
                Debug.Log("No BossBar found");
            return instance;
        }
    }

    void Start()
    {
        bossManager = BossManager.Instance;
        BarStart();
    }

    void Update()
    {
        //pour les tests
        if (Input.GetKey("v"))
        {
            TakeDamage(damage * 10);
        }
        if (Input.GetKeyDown("f"))
        {
            TakeDamage(-damage);
        }
        BarUpdate();
    }

    public void TakeDamage(float damage)
    {
        SoustractToValue(damage * HarmonieBar.Instance.GetMultiplier());
        if (bossManager.goUltralaser)
            bossManager.StackDmg += damage;
        if (Value <= 0)
        {
            SetValue(0);
            Debug.Log("le boss est mort !");
            BarManager.Instance.WinGame();
        }
    }

    public void hide()
    {
        back.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
