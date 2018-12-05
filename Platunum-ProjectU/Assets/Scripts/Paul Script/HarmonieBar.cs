using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : MonoBehaviour {

    [SerializeField]
    private Image harmonieBar;

    private float harmoniePoint = 0f;
    private float harmonieMaxPoint = 100f;

    private float currentHarmoniePoint;
    private float  m_ratio;

    public float speed = 40;


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
        currentHarmoniePoint = harmoniePoint;
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

        if (currentHarmoniePoint != harmoniePoint)
        {

            currentHarmoniePoint = currentHarmoniePoint + Mathf.Sign(harmoniePoint - currentHarmoniePoint) * speed * Time.deltaTime;
            m_ratio = currentHarmoniePoint / harmonieMaxPoint;
            harmonieBar.fillAmount = m_ratio;
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
