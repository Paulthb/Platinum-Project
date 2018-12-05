using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : MonoBehaviour {

    [SerializeField]
    private Image harmonieBar;

    private float harmoniePoint = 0f;
    private float harmonieMaxPoint = 100f;

    private static HarmonieBar instance;
    public static HarmonieBar Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<HarmonieBar>();
            if (instance == null)
                Debug.Log("No HarmonieBar found");
            return instance;
        }
    }


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
            TakeHarmonie(harmoniePoint);
        }

    }

    public void TakeHarmonie(float harmoniePt)
    {

        harmoniePoint += harmoniePt;
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
        harmonieBar.fillAmount = ratio;
    }
}
