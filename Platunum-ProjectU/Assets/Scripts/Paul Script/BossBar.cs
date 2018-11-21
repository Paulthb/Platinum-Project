using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour {

    [SerializeField]
    private Image bossBar;

    public float bossPoint = 1000f;
    private float bossMaxPoint = 1000f;

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

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("v"))
        {
            TakeDamage(50);
        }
    }

    private void UpdateBar()
    {
        float ratio;
        ratio = bossPoint / bossMaxPoint;
        bossBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }

    public void TakeDamage(int damage)
    {

        bossPoint -= damage;
        if (bossPoint <= 0)
        {
            bossPoint = 0;
            Debug.Log("le boss est mort !");
            BarManager.Instance.WinGame();
        }
        UpdateBar();
    }
}
