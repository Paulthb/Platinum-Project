using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour {

    [SerializeField]
    private Image bossBar;

    private float bossPoint = 1000f;
    public float bossMaxPoint = 1000f;

    public float currentBossPoint;

    private float m_ratio;
    public float speed = 40;
    public float damage = 50;

    private BossManager bossManager;

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
        bossPoint = bossMaxPoint;
        currentBossPoint = bossPoint;
        bossManager = BossManager.Instance;
    }

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("v"))
        {
            TakeDamage(damage);
        }
        if (Input.GetKeyDown("f"))
        {
            TakeDamage(-damage);
        }

        if (currentBossPoint != bossPoint)
        {
            float ToAdd = Mathf.Sign(bossPoint - currentBossPoint) * speed * Time.deltaTime;

            if(ToAdd > Mathf.Abs(currentBossPoint - bossPoint))
                currentBossPoint = bossPoint;
            else
                currentBossPoint = currentBossPoint + ToAdd;

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
        if (bossManager.goUltralaser)
            bossManager.StackDmg += damage;
        if (bossPoint <= 0)
        {
            bossPoint = 0;
            BarManager.Instance.WinGame();
        }
    }
}
