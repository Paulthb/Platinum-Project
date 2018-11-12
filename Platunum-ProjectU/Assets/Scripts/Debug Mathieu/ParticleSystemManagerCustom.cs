using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManagerCustom : MonoBehaviour 
{
	public ParticleSet[] particleSet;

	//will be informed by the Conductor after a beat is hit
	public void BeatOnHit(int track, PartitionManager.Rank rank)
	{
		if (rank == PartitionManager.Rank.PERFECT)
		{
			particleSet[track].perfect.Play();
			/*perfectionEffect.Play();
			comboEffect.Play();*/
		}
		else if (rank == PartitionManager.Rank.GOOD)
		{
			particleSet[track].good.Play();
			//comboEffect.Play();
		}
		else if (rank == PartitionManager.Rank.BAD)
		{
			particleSet[track].bad.Play();
		}


		//do nothing if missed
	}

    //Parametrage sur unity editor
	[System.Serializable]
	public class ParticleSet
	{
		public ParticleSystem perfect;
		public ParticleSystem good;
		public ParticleSystem bad;
	}
}
