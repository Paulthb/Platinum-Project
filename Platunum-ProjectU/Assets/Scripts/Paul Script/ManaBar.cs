using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {

    [SerializeField]
    private Image manaBar;

    // valeurs temporaires, il faut récup en fontion des stats des joueurs
    public float manaPoint = 900f;
    private float manaMaxPoint = 900f;

    private float currentManaPoint;
    private float m_ratio;

    public float speed = 80;


    void Start()
    {
        currentManaPoint = manaPoint;
    }

    void Update()
    {
        //pour les tests
        if (Input.GetKeyDown("c"))
        {
            WinMana(-10);
        }

        if (currentManaPoint != manaPoint)
        {

            currentManaPoint = currentManaPoint + Mathf.Sign(manaPoint - currentManaPoint) * speed * Time.deltaTime;
            m_ratio = currentManaPoint / manaMaxPoint;
            manaBar.fillAmount = m_ratio;
        }
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

    private void UpdateBar()
    {
        float ratio;
        ratio = manaPoint / manaMaxPoint;
        manaBar.fillAmount = ratio;
    }
    
    public void WinMana(int manaPt)
    {
        manaPoint += manaPt;
        if (manaPoint > manaMaxPoint) //récuperer la somme de mana groupe max
        {
            manaPoint = manaMaxPoint;
            Debug.Log("full mana");
        }
        UpdateBar();
    }

    public void Attack()
    {
        manaPoint -= 100;
        if (manaPoint < 0)
        {
            manaPoint = 0;
            Debug.Log("on ne peut plus attaquer");
        }

        UpdateBar();
    }
}
