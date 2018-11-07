using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Image healthbar;
    [SerializeField]
    private Text rationText;

    private float HitPoint = 150f;
    private float MaxHitPoint = 150f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        //pour les tests
        if (Input.GetKeyDown("space"))
        {
            TakeDamage();
        }

    }

    private void UpdateHeatlBar()
    {
        float ratio = HitPoint / MaxHitPoint;
        healthbar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        rationText.text = (ratio * 100).ToString() + '%'; 
    }

    private void TakeDamage()
    {
        HitPoint -= 10;
        if (HitPoint < 0)
        {
            HitPoint = 0;
            Debug.Log("on est mort !");
        }

        UpdateHeatlBar();
    }

    private void TakeArmor()
    {
    }
}
