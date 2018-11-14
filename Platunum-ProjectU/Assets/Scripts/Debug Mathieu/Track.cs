using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {
    [SerializeField]
    private TrackParticle trackParticle;
    public float offsetX;

    public void PlayParticle(PartitionManager.Rank rank)
    {
        switch (rank)
        {
            case PartitionManager.Rank.PERFECT:
                trackParticle.Perfect.Play();
                break;
            case PartitionManager.Rank.GOOD:
                trackParticle.Good.Play();
                break;
            case PartitionManager.Rank.BAD:
                trackParticle.Bad.Play();
                break;
        }
    }

    public void SetTrackWidth(float size)
    {
        Transform track = transform.Find("track");
        track.localScale = new Vector3(size, track.localScale.y, track.localScale.z);
    }
}

[System.Serializable]
public class TrackParticle
{
    public ParticleSystem Perfect;
    public ParticleSystem Good;
    public ParticleSystem Bad;
    public ParticleSystem Fail;
}
