using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour {

    [SerializeField]
    private Image manaBar;

    // valeurs temporaires, il faut récup en fontion des stats des joueurs
    private float manaPoint = 900f;
    private float manaMaxPoint = 900f;

    // Update is called once per frame
    /*
    void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("c"))
        {
            TakeDamage();
        }
    }*/

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
        manaBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
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

    private void Attack()
    {
        manaPoint -= 10;
        if (manaPoint < 0)
        {
            manaPoint = 0;
            Debug.Log("on ne peut plus attaquer");
        }

        UpdateBar();
    }
}
