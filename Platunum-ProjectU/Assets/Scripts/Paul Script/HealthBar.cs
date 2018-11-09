using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image armorBar;

    [SerializeField]
    private Text rationText;

    private float healthPoint = 150f;
    private float armorPoint = 150f;
    private float maxHealthPoint = 150f;
    private float maxArmorPoint = 150f;

    private bool isArmor = true;


	// Use this for initialization
	void Start ()
    {
        UpdateIsArmor();
    }
	
	// Update is called once per frame
	void Update () {
		
        //pour les tests
        if (Input.GetKeyDown("space"))
        {
            TakeDamage();
        }
    }

    private void UpdateBar()
    {
        float ratio;

        if (!isArmor)
        {
            ratio = healthPoint / maxHealthPoint;
            healthBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        }
        else
        {
            ratio = armorPoint / maxArmorPoint;
            armorBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        }
        rationText.text = (ratio * 100).ToString() + '%';

        UpdateIsArmor();
    }

    private void TakeDamage()
    {
        if (!isArmor)
        {
            healthPoint -= 10;
            if (healthPoint < 0)
            {
                healthPoint = 0;
                Debug.Log("on est mort !");
            }
        }
        else
        {
            armorPoint -= 10;
            if (armorPoint < 0)
            {
                armorPoint = 0;
                Debug.Log("plus d'armure");
            }
        }

        UpdateBar();
        UpdateIsArmor();
    }

    private void TakeArmor()
    {
    }

    private void UpdateIsArmor()
    {
        if (armorPoint <= 0)
            isArmor = false;
        else
            isArmor = true;
    }
}
