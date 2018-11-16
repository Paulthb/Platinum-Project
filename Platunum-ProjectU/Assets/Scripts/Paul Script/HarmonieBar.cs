using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : MonoBehaviour {

    [SerializeField]
    private Image harmonieBar;

    private float harmoniePoint = 0f;
    private float harmonieMaxPoint = 100f;

    // Use this for initialization
    void Start ()
    {
        UpdateBar();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("x"))
        {
            TakeHarmonie();
        }

    }

    private void TakeHarmonie()
    {

        harmoniePoint += 10;
        if (harmoniePoint >= 100)
        {
            harmoniePoint = 100;
            Debug.Log("on passe en fois 2 !");
        }

        UpdateBar();
    }

    private void UpdateBar()
    {
        float ratio;
        ratio = harmoniePoint / harmonieMaxPoint;
        harmonieBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
    }
}
