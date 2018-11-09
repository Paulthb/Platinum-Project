using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour {

    [SerializeField]
    private Image bossBar;

    private float bossPoint = 1000f;
    private float bossMaxPoint = 1000f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("v"))
        {
            TakeDamage();
        }
    }

    private void UpdateBar()
    {
        float ratio;
        ratio = bossPoint / bossMaxPoint;
        bossBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }

    private void TakeDamage()
    {

        bossPoint -= 10;
        if (bossPoint < 0)
        {
            bossPoint = 0;
            Debug.Log("on est mort !");
        }

        UpdateBar();
    }
}
