using UnityEngine;
using System.Collections;
using System;
using VRTK;

public class InteractController : MonoBehaviour {

    
    public AudioSource bgmSource;
    public AudioSource buttonCueSource;
    public AudioSource sliderCueSource;
    public AudioSource throttleCueSource;
    public AudioSource buttonSuccessSource;
    public AudioSource sliderSuccessSource;
    public AudioSource throttleSuccessSource;
    public AudioSource failureSource;

    public AudioClip bgmClip = new AudioClip();
    public AudioClip buttonCueClip = new AudioClip();
    public AudioClip sliderCueClip = new AudioClip();
    public AudioClip throttleCueClip = new AudioClip();
    public AudioClip buttonSuccessClip = new AudioClip();
    public AudioClip sliderSuccessClip = new AudioClip();
    public AudioClip throttleSuccessClip = new AudioClip();
    public AudioClip failureClip = new AudioClip();

    public GameObject button;
    public GameObject slider;
    public GameObject throttle;

    Metronome metro;

    public int[] sequencer1 = new int[16];
    public int[] sequencer2 = new int[16];
    public int[] sequencer3 = new int[16];
    int seqPosition = 0;

    float beatTime;
    bool checkButtonInput = false;
    bool checkThrottleInput = false;
    bool checkSliderInput = false;
    float buttonInputTimer, throttleInputTimer, sliderInputTimer;
    float lowerHitBound, upperHitBound;

    int health = 100;

    // Use this for initialization
    void Start () {
        bgmSource = AddAudio(bgmClip, true, false, .7f);
        buttonCueSource = AddAudio(buttonCueClip, false, false, 1f);
        sliderCueSource = AddAudio(sliderCueClip, false, false, 1f); ;
        throttleCueSource = AddAudio(throttleCueClip, false, false, 1f); ;
        buttonSuccessSource = AddAudio(buttonSuccessClip, false, false, 1f); ;
        sliderSuccessSource = AddAudio(sliderSuccessClip, false, false, 1f); ;
        throttleSuccessSource = AddAudio(throttleSuccessClip, false, false, 1f); ;
        failureSource = AddAudio(failureClip, false, false, 1f); ;

        metro = gameObject.GetComponent<Metronome>();
        metro.OnTick += Tick;
        bgmSource.Play();
        beatTime = 60 / metro.BPM;

        lowerHitBound = beatTime / 2f;
        upperHitBound = beatTime * 3f;

        button.GetComponent<VRTK_Button>().events.OnPush.AddListener(ButtonInput);
        throttle.GetComponent<VRTK_Lever>().defaultEvents.OnValueChanged.AddListener(ThrottleInput);
        slider.GetComponent<VRTK_Slider>().defaultEvents.OnValueChanged.AddListener(SliderInput);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ButtonInput();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SliderInput(0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ThrottleInput(0f, 0f);
        }

        if (checkButtonInput) {
            Debug.Log("Checking Button...");
            buttonInputTimer += Time.deltaTime;
            if (buttonInputTimer > upperHitBound)
            {
                buttonFailure();
            }
        } else {
            buttonInputTimer = 0f;
        }

        if (checkSliderInput) {
            Debug.Log("Checking Slider...");
            sliderInputTimer += Time.deltaTime;
            if (sliderInputTimer > upperHitBound)
            {
                sliderFailure();
            }
        } else {
            sliderInputTimer = 0f;
        }

        if (checkThrottleInput) {
            Debug.Log("Checking Throttle...");
            throttleInputTimer += Time.deltaTime;
            if (throttleInputTimer > upperHitBound)
            {
                throttleFailure();
            }
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
            checkButtonInput = false;
        } else if (sequencer1[seqPosition] == 2) {
            buttonCueSource.Play();
            checkButtonInput = true;
        }

        if (sequencer2[seqPosition] == 1) {
            sliderCueSource.Play();
            checkSliderInput = false;
        } else if (sequencer2[seqPosition] == 2){
            sliderCueSource.Play();
            checkSliderInput = true;
        }

        if (sequencer3[seqPosition] == 1) {
            throttleCueSource.Play();
            checkThrottleInput = false;
        } else if (sequencer3[seqPosition] == 2){
            throttleCueSource.Play();
            checkThrottleInput = true;
        }

        seqPosition++;
        if (seqPosition >= sequencer1.Length){
            seqPosition = 0;
        }
    }

    public void ButtonInput()
    {
        Debug.Log("Button pressed.");
        if (buttonInputTimer >= lowerHitBound && buttonInputTimer <= upperHitBound) {
            buttonSuccess();
        } else {
            buttonFailure();
        }
    }

    public void SliderInput(float val, float normalizedVal)
    {
        Debug.Log("Slider flipped.");
        if (sliderInputTimer >= lowerHitBound && sliderInputTimer <= upperHitBound) {
            sliderSuccess();
        } else {
            sliderFailure();
        }
    }

    public void ThrottleInput(float val, float normalizedVal)
    {
        Debug.Log("Throttle engaged.");
        if (throttleInputTimer >= lowerHitBound && throttleInputTimer <= upperHitBound) {
            throttleSuccess();
        } else {
            throttleFailure();
        }
    }

    private void buttonSuccess()
    {
        Debug.Log("Button Success.");
        checkButtonInput = false;
        buttonInputTimer = 0f;
        buttonSuccessSource.PlayOneShot(buttonSuccessClip, 1f);
        // success animation
    }

    private void sliderSuccess()
    {
        Debug.Log("Slider Success.");
        checkSliderInput = false;
        sliderInputTimer = 0f;
        sliderSuccessSource.PlayOneShot(sliderSuccessClip, 1f);

        // success animation
    }
    private void throttleSuccess()
    {
        Debug.Log("Throttle Success.");
        checkThrottleInput = false;
        throttleInputTimer = 0f;
        sliderSuccessSource.PlayOneShot(throttleSuccessClip, 1f);
        // success animation
    }

    void buttonFailure()
    {
        health -= 10;
        failureSource.PlayOneShot(failureClip, 1f);
        checkButtonInput = false;
        buttonInputTimer = 0f;
        Debug.Log("Button Failure.");
        // failure animations
    }

    void sliderFailure()
    {
        health -= 10;
        failureSource.PlayOneShot(failureClip, 1f);
        checkSliderInput = false;
        sliderInputTimer = 0f;
        Debug.Log("Slider Failure.");
        // failure animations
    }

        void throttleFailure()
    {
        health -= 10;
        failureSource.PlayOneShot(failureClip, 1f);
        checkThrottleInput = false;
        throttleInputTimer = 0f;
        Debug.Log("Throttle Failure.");
        // failure animations
    }
}
