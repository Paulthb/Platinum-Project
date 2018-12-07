using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour {

    [SerializeField]
    private Image bossBar;

    public float bossPoint = 1000f;
    private float bossMaxPoint = 1000f;

    public float currentBossPoint;

    private float m_ratio;
    public float speed = 40;
    public float damage = 50;

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
        currentBossPoint = bossPoint;
    }

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("v"))
        {
            TakeDamage(damage);
        }

        if (currentBossPoint != bossPoint)
        {
            currentBossPoint = currentBossPoint + Mathf.Sign(bossPoint - currentBossPoint) * speed * Time.deltaTime;
            m_ratio = currentBossPoint / bossMaxPoint;
            bossBar.fillAmount = m_ratio;
        }
    }

    private void UpdateBar()
    {
        float ratio;
        ratio = bossPoint / bossMaxPoint;
        bossBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }

    public void TakeDamage(float damage)
    {

        bossPoint -= damage;
        if (bossPoint <= 0)
        {
            bossPoint = 0;
            Debug.Log("le boss est mort !");
            BarManager.Instance.WinGame();
        }
        //UpdateBar();
    }
}
