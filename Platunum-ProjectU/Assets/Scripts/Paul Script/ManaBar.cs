using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {

    [SerializeField]
    private Image manaBar;

    private float manaPoint = 100f;
    private float manaMaxPoint = 100f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("c"))
        {
            TakeDamage();
        }
    }

    private void UpdateBar()
    {
        float ratio;
        ratio = manaPoint / manaMaxPoint;
        manaBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }

    private void TakeDamage()
    {

        manaPoint -= 10;
        if (manaPoint < 0)
        {
            manaPoint = 0;
            Debug.Log("on est mort !");
        }

        UpdateBar();
    }
}
