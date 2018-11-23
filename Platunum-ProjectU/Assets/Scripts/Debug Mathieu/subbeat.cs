using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class subbeat : MonoBehaviour {
    private PrimaryBeat parentBeat;
    public int subNode;
    private void Awake()
    {
        parentBeat = transform.parent.GetComponent<PrimaryBeat>();
    }

    public void OnClick(int trackId)
    {
        parentBeat.OnClick(trackId, subNode);
    }
}
