﻿using UnityEngine;

public class PlayerInputControl : MonoBehaviour 
{

    public delegate void InputtedAction(int partitionId, int trackNumber);
    public static event InputtedAction inputtedEvent;

    public CircleCollider2D[] tappingSpheres;
	
    //in unity editor & standalone, input by keyboard
    #if UNITY_EDITOR || UNITY_STANDALONE

    private KeyCode[] keybindings;
    private KeyCode pauseKey;

    #endif

    private AudioSource[] audioSources;
    public int partitionId;
    //cache the number of tracks
    private int trackLength;

    //animation
    public SpriteRenderer[] innerCircles;
    private SpriteColorLerp[] innerAnimations;
    private Coroutine[] previousInnerAnimations;
    private const float StartAlphaForInner = 0.7f;

    void Start()
    {
        trackLength = tappingSpheres.Length;

        //init audio sources (cache them), and configure the recorded clips & tap animation
        audioSources = new AudioSource[trackLength];
        previousInnerAnimations = new Coroutine[trackLength];
        innerAnimations = new SpriteColorLerp[trackLength];
        for (int i = 0; i < trackLength; i++)
        {
            audioSources[i] = tappingSpheres[i].GetComponent<AudioSource>();

            //audioSources[i].clip = SongInfoCustom.Instance.recordedBeats[i];

            //init inner circle animation
            //Color startColor = new Color(innerCircles[i].color.r, innerCircles[i].color.g, innerCircles[i].color.b, StartAlphaForInner);
            //innerAnimations[i] = new SpriteColorLerp(innerCircles[i], startColor, innerCircles[i].color, audioSources[i].clip.length);
            previousInnerAnimations[i] = null;
        }

        //just for debugging
        #if UNITY_EDITOR || UNITY_STANDALONE
        keybindings = new KeyCode[4];

        /*keybindings[1] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track2);
        keybindings[2] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track3);
        keybindings[3] = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Track4);
        pauseKey = KeyboardInputManager.instance.GetKeyCode(KeyboardInputManager.KeyBindings.Pause);*/

        keybindings[0] = inputManager.instance.GetKeyCode(partitionId, 0);
        keybindings[1] = inputManager.instance.GetKeyCode(partitionId, 1);
        keybindings[2] = inputManager.instance.GetKeyCode(partitionId, 2);
        keybindings[3] = inputManager.instance.GetKeyCode(partitionId, 3);
        pauseKey = inputManager.instance.GetKeyCode(partitionId, 4);

        KeyboardInputManager.keyChangedEvent += KeyChanged;
        #endif

    }

    #if UNITY_EDITOR || UNITY_STANDALONE
    void KeyChanged(KeyboardInputManager.KeyBindings keyBinding, KeyCode keyCode)
    {
        if (keyBinding == KeyboardInputManager.KeyBindings.Pause)
        {
            pauseKey = keyCode;
        }
        else
        {
            keybindings[(int) keyBinding] = keyCode;
        }
    }

    void OnDestroy()
    {
        KeyboardInputManager.keyChangedEvent -= KeyChanged;
    }
    #endif
    

    void Update()
    {
        if (ConductorCustom.paused) return;

        //keyboard input
#if UNITY_EDITOR || UNITY_STANDALONE
        for (int i = 0; i < trackLength; i++)
        {
            if (Input.GetKeyDown(keybindings[i]))
            {
                Inputted(i);
            }
        }

        if (Input.GetKeyDown(pauseKey))
        {
            FindObjectOfType<PlayingUIController>().PauseButtonOnClick();
        }
        #endif

        //touch input
        #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR

        //check touch input
        foreach (Touch touch in Input.touches)
        {
            
            //tap down
            if (touch.phase == TouchPhase.Began)
            {
                //check if on a tapping sphere
                for (int i = 0; i < trackLength; i++)
                {
                    if (tappingSpheres[i].OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)))
                    {
                        Inputted(i);
                    }
                }
            }
        }
        #endif
    }

    void Inputted(int i)
    {
        //inform Conductor and other interested classes
        if (inputtedEvent != null) inputtedEvent(partitionId,i);

        //play audio clip
        audioSources[i].Play();

        //start inner circle animation
        /*if (previousInnerAnimations[i] != null)
        {
            StopCoroutine(previousInnerAnimations[i]);
        }
        previousInnerAnimations[i] = StartCoroutine(innerAnimations[i].AnimationCoroutine());*/
    }
}