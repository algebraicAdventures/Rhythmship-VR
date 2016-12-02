using UnityEngine;
using System.Collections;

public class InteractController : MonoBehaviour {

    
    public AudioSource bgmSource;
    public AudioSource buttonCueSource;
    public AudioSource switchCueSource;
    public AudioSource throttleCueSource;
    public AudioSource buttonSuccessSource;
    public AudioSource switchSuccessSource;
    public AudioSource throttleSuccessSource;
    public AudioSource failureSource;

    public AudioClip bgmClip = new AudioClip();
    public AudioClip buttonCueClip = new AudioClip();
    public AudioClip switchCueClip = new AudioClip();
    public AudioClip throttleCueClip = new AudioClip();
    public AudioClip buttonSuccessClip = new AudioClip();
    public AudioClip switchSuccessClip = new AudioClip();
    public AudioClip throttleSuccessClip = new AudioClip();
    public AudioClip failureClip = new AudioClip();

    Metronome metro;

    public int[] sequencer1 = new int[16];
    public int[] sequencer2 = new int[16];
    public int[] sequencer3 = new int[16];
    int seqPosition = 0;

    float beatTime;
    bool checkButtonInput = false;
    bool checkThrottleInput = false;
    bool checkSwitchInput = false;
    float buttonInputTimer, throttleInputTimer, switchInputTimer;

    // Use this for initialization
    void Start () {
        bgmSource = AddAudio(bgmClip, true, false, .7f);
        buttonCueSource = AddAudio(buttonCueClip, false, false, 1f);
        switchCueSource = AddAudio(switchCueClip, false, false, 1f); ;
        throttleCueSource = AddAudio(throttleCueClip, false, false, 1f); ;
        buttonSuccessSource = AddAudio(buttonSuccessClip, false, false, 1f); ;
        switchSuccessSource = AddAudio(switchSuccessClip, false, false, 1f); ;
        throttleSuccessSource = AddAudio(throttleSuccessClip, false, false, 1f); ;
        failureSource = AddAudio(failureClip, false, false, 1f); ;

        metro = gameObject.GetComponent<Metronome>();
        metro.OnTick += Tick;
        bgmSource.Play();
        beatTime = 60 / metro.BPM;
    }
	
	// Update is called once per frame
	void Update () {
	    if (checkButtonInput) {
            buttonInputTimer += Time.deltaTime;
        } else {
            buttonInputTimer = 0f;
        }

        if (checkSwitchInput) {
            switchInputTimer += Time.deltaTime;
        } else {
            switchInputTimer = 0f;
        }

        if (checkThrottleInput) {
            throttleInputTimer += Time.deltaTime;
        } else {
            throttleInputTimer = 0;
        }
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.loop = loop;
        audio.playOnAwake = playAwake;
        audio.volume = vol;
        return audio;
    }

    private void Tick(Metronome metronome)
    {
        if (sequencer1[seqPosition] == 1) {
            buttonCueSource.Play();
        } else if (sequencer1[seqPosition] == 2) {
            checkButtonInput = true;
        } else {
            checkButtonInput = false;
        }

        if (sequencer2[seqPosition] == 1) {
            switchCueSource.Play();
        } else if (sequencer2[seqPosition] == 2)
        {
            checkSwitchInput = true;
        }
        else
        {
            checkSwitchInput = false;
        }

        if (sequencer3[seqPosition] == 1) {
            throttleCueSource.Play();
        } else if (sequencer3[seqPosition] == 2)
        {
            checkThrottleInput = true;
        }
        else
        {
            checkThrottleInput = false;
        }

        seqPosition++;
        if (seqPosition >= sequencer1.Length)
        {
            seqPosition = 0;
        }
    }

    public void ButtonInput()
    {
        Debug.Log("Button pressed.");
        if (buttonInputTimer >= beatTime / 2 && buttonInputTimer <= beatTime + beatTime / 2) {
            // success
        } else {
            failure();
        }
    }

    public void SwitchInput()
    {
        Debug.Log("Switch flipped.");
        // check for timing
    }

    public void ThrottleInput()
    {
        Debug.Log("Throttle engaged.");
        // check for timing
    }

    void failure()
    {
        // subtract ship health
        // play failure sound
        // failure animations
    }
}
