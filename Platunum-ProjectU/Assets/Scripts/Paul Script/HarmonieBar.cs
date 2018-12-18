using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarmonieBar : BarUI {
    private int multiplier = 1;
    private bool emptying = false;
    public float multiplierDuration = 20f;
    public Text multiplierText;

    //UI
    private float TimerUI = 0;
    private float targetScale = 1.25f;
    public float minScale = 1;
    public float maxScale = 1.5f;

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
        multiplierText.transform.localScale = Vector3.one * minScale;
        BarStart();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //pour les tests
        if (Input.GetKeyDown("x"))
        {
            GiveHarmonie(10f);
        }

        if (Input.GetKeyDown("x"))
        {
            GiveHarmonie(10f);
        }
        if (emptying)
        {
            SoustractToValue((MaxValue / multiplierDuration) * Time.deltaTime);
            if (Value <= 0)
            {
                emptying = false;
                multiplier = 1;

                //Reset UI
                multiplierText.enabled = false;
                multiplierText.transform.localScale = Vector3.one * minScale;
                targetScale = maxScale;
                TimerUI = 0;
            }

            TimerUI += Time.deltaTime;
            float scale = Mathf.Lerp(multiplierText.transform.localScale.x, targetScale, TimerUI);
            multiplierText.transform.localScale = new Vector3(scale, scale, scale);
            if(Mathf.Abs(targetScale - scale) < 0.1f)
            {
                if(targetScale == maxScale)
                {
                    targetScale = minScale;
                }
                else
                {
                    targetScale = maxScale;
                }
                TimerUI = 0;
            }
        }
        BarUpdate();
    }

    public void GiveHarmonie(float harmoniePt)
    {
        if (!emptying)
        {
            AddToValue(harmoniePt);
            if (Value >= MaxValue)
            {
                SetValue(MaxValue);
                TriggerMultiplier();
            }
        }
    }

    private void TriggerMultiplier()
    {
        multiplier = 2;
        emptying = true;
        multiplierText.enabled = true;
    }

    public int GetMultiplier()
    {
        return multiplier;
    }
}
