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

    // valeurs temporaires, il faut récup en fontion des stats des joueurs
    public float healthPoint = 950f;
    private float armorPoint = 300f;
    private float maxHealthPoint = 950f;
    private float maxArmorPoint = 300f;

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
        UpdateIsArmor();
    }

    // Update is called once per frame
    /*
    void Update () {
		
        //pour les tests
        if (Input.GetKeyDown("space"))
        {
            TakeDamage();
        }
    }*/

    public void WinArmor(int armorPt)
    {
        armorPoint += armorPt;
        if (armorPoint > maxArmorPoint) //récuperer la somme de armor groupe max
        {
            armorPoint = maxArmorPoint;
            Debug.Log("full armor");
        }
        UpdateBar();
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

    public void TakeDamage(int damagePt)
    {
        if (!isArmor)
        {
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
            armorPoint -= damagePt;
            if (armorPoint < 0)
            {
                armorPoint = 0;
                isArmor = false;
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
