using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

    public Animator animatorFireBall;

	// Use this for initialization
	void Start ()
    {
        //StartCoroutine(FireBallExplode());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FireBallExplode()
    {
        yield return new WaitForSeconds(0.05f);
        //animatorFireBall.SetTrigger("FireBallExplode");
    }
}
