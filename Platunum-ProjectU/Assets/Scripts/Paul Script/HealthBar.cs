﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image armorBar;

    // valeurs temporaires, il faut récup en fontion des stats des joueurs
    public float healthPoint = 300f;
    private float oldHealthPoint;
    private float armorPoint = 100f;
    private float maxHealthPoint = 300f;
    private float maxArmorPoint = 100f;
    private float oldArmorPoint;
    private float currentHealthPoint;
    private float currentArmorPoint;
    private float m_ratio;

    public float speed = 40;


    private bool isArmor = true;

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
        currentHealthPoint = healthPoint;
        currentArmorPoint = armorPoint;
        UpdateIsArmor();
    }

    // Update is called once per frame

    void Update()
    {

        if (currentHealthPoint != healthPoint || currentArmorPoint != armorPoint)
        { 
            if (!isArmor)
            {
                //Fill Gap
                float ToAdd = Mathf.Sign(healthPoint - currentHealthPoint) * speed * Time.deltaTime;

                //If we overfill
                if(ToAdd > Mathf.Abs(currentHealthPoint - healthPoint))
                    currentHealthPoint = healthPoint;//Get Current value
                else
                    currentHealthPoint = currentHealthPoint + ToAdd;//Fill the amout toadd

                //Display ratio
                m_ratio = currentHealthPoint / maxHealthPoint;
                healthBar.fillAmount = m_ratio;
            }
            else
            {
                float ToAdd = Mathf.Sign(armorPoint - currentArmorPoint) * speed * Time.deltaTime;

                if(ToAdd > Mathf.Abs(currentArmorPoint - armorPoint))
                    currentArmorPoint = armorPoint;
                else
                    currentArmorPoint = currentArmorPoint + ToAdd;

                m_ratio = currentArmorPoint / maxArmorPoint;
                armorBar.fillAmount = m_ratio;
            }
        }
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
    }

    public void WinArmor(int armorPt)
    {
        armorPoint += armorPt;
        if (armorPoint > maxArmorPoint) //récuperer la somme de armor groupe max
        {
            armorPoint = maxArmorPoint;
            Debug.Log("full armor");
        }
    }

    public void TakeDamage(int damagePt)
    {
        if (!isArmor)
        {
            oldHealthPoint = healthPoint;
            healthPoint -= damagePt;
            if (healthPoint < 0)
            {
                healthPoint = 0;
                Debug.Log("on est mort !");
                BarManager.Instance.EndGame();
            }
        }
        else
        {
            oldArmorPoint = armorPoint;
            armorPoint -= damagePt;
            if (armorPoint <= 0)
            {
                armorBar.fillAmount = 0;//////////////////////////////////////////
                armorPoint = 0;
                Debug.Log("plus d'armure");
            }
        }

        UpdateIsArmor();
    }

    private void UpdateIsArmor()
    {
        if (armorPoint <= 0)
            isArmor = false;
        else
            isArmor = true;
    }
}
