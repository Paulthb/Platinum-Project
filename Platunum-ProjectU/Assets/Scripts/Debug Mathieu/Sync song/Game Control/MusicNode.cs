using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicNode : MonoBehaviour
{
	public TextMesh timesText;
	public SpriteRenderer ringSprite;
	[NonSerialized] public float startY;
	[NonSerialized] public float endY;
	[NonSerialized] public float removeLineY;
	[NonSerialized] public float beat;
	[NonSerialized] public int times;
	[NonSerialized] public bool paused;
    [NonSerialized] public bool isStone;

    public Sprite[] StoneSrpiteTab;
    public SpriteRenderer StoneSprite;

    private Player player;
    private int idPlayer;

    public void Initialize(float posX, float startY, float endY, float removeLineY, float posZ, float targetBeat, int times, Color color, int id)
	{
		this.startY = startY;
		this.endY = endY;
		this.beat = targetBeat;
		this.times = times;
		this.removeLineY = removeLineY;
        isStone = false;
        idPlayer = id;
		paused = false;

		//set position
		transform.position = new Vector3(posX, startY, posZ);
		
		//set color
		ringSprite.color = color;   

		//randomize background
		//GetComponent<SpriteRenderer>().sprite = backgroundSprites[0];

		//set times
		/*if (times > 0)
		{
			timesText.text = times.ToString();
			timesTextBackground.SetActive(true);
		}
		else
		{
			//timesTextBackground.SetActive(false);

			//randomize rotation
			//transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 359f));
		}*/
		
	}

	void Update()
	{
		if (ConductorCustom.pauseTimeStamp > 0f) return; //resume not managed

		if (paused) return; //multi-times notes might be paused on the finish line

		transform.position = new Vector3(transform.position.x, startY + (endY - startY) * (1f - (beat - ConductorCustom.songposition / ConductorCustom.crotchet) / ConductorCustom.BeatsShownOnScreen), transform.position.z);
	
		//remove itself when out of the screen (remove line)
		if (transform.position.y < removeLineY)
		{
			gameObject.SetActive(false);
            //BarManager.Instance.GetImpact(null, PartitionManager.Rank.MISS);
            if (isStone)
            {
                BossManager.Instance.CancelAttackStone();
                ShieldBar.Instance.TakeDamage(50);
            }
            else
            {
                ShieldBar.Instance.TakeDamage(8);
            }
            // Son en fct du perso
            
            player = PlayerManager.Instance.GetPlayer(idPlayer);
            if (player.Personnage.id == 0)
            {
                int i = UnityEngine.Random.Range(1, 5);
                switch (i)
                {
                    case 1:
                        SoundMgr.Instance.PlaySound("FailPiano1");
                        break;
                    case 2:
                        SoundMgr.Instance.PlaySound("FailPiano2");
                        break;
                    case 3:
                        SoundMgr.Instance.PlaySound("FailPiano3");
                        break;
                    case 4:
                        SoundMgr.Instance.PlaySound("FailPiano4");
                        break;
                }
            }
            else if (player.Personnage.id == 1)
            {
                int i = UnityEngine.Random.Range(1, 5);
                switch (i)
                {
                    case 1:
                        SoundMgr.Instance.PlaySound("FailBass1");
                        break;
                    case 2:
                        SoundMgr.Instance.PlaySound("FailBass2");
                        break;
                    case 3:
                        SoundMgr.Instance.PlaySound("FailBass3");
                        break;
                    case 4:
                        SoundMgr.Instance.PlaySound("FailBass4");
                        break;
                }
            }
            else if (player.Personnage.id == 2)
            {
                int i = UnityEngine.Random.Range(1, 5);
                switch (i)
                {
                    case 1:
                        SoundMgr.Instance.PlaySound("FailGuitar1");
                        break;
                    case 2:
                        SoundMgr.Instance.PlaySound("FailGuitar2");
                        break;
                    case 3:
                        SoundMgr.Instance.PlaySound("FailGuitar3");
                        break;
                    case 4:
                        SoundMgr.Instance.PlaySound("FailGuitar4");
                        break;
                }
            }
            else if (player.Personnage.id == 3)
            {
                int i = UnityEngine.Random.Range(1, 5);
                switch (i)
                {
                    case 1:
                        SoundMgr.Instance.PlaySound("FailBell1");
                        break;
                    case 2:
                        SoundMgr.Instance.PlaySound("FailBell2");
                        break;
                    case 3:
                        SoundMgr.Instance.PlaySound("FailBell3");
                        break;
                    case 4:
                        SoundMgr.Instance.PlaySound("FailBell4");
                        break;
                }
            }
        }
        StoneSprite.gameObject.SetActive(isStone);
	}

	//remove (multi-times note failed), might apply some animations later
	public void MultiTimesFailed()
	{
		gameObject.SetActive(false);
	}

	//if the node is removed, return true
	public bool MultiTimesHit()
	{
		//update text
		times--;
		if (times == 0)
		{
			gameObject.SetActive(false);
			return true;
		}

		//timesText.text = times.ToString();
			
		return false;
	}

	public void PerfectHit()
	{
		gameObject.SetActive(false);
	}

	public void GoodHit()
	{
		gameObject.SetActive(false);
	}

	public void BadHit()
	{
		gameObject.SetActive(false);
	}

    public void HitOnBloc()
    {

    }

    public void updateSprite(int idSprite)
    {
        StoneSprite.sprite = StoneSrpiteTab[idSprite];
    }
}
