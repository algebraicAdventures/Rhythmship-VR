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

    int[,] difficulty1 = new int[,] {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
        {1, 0, 1, 0, 2, 0, 0, 0, 1, 0, 1, 0, 2, 0, 0 ,0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0}};

    int[,] difficulty2 = new int[,] {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
        {1, 0, 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
        {0, 0, 0, 0, 0, 1, 0, 1, 0, 2, 0, 0, 0, 0, 0 ,0}};

    int[,] difficulty3 = new int[,] {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 2, 0},
        {1, 0, 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0},
        {0, 0, 0, 0, 0, 1, 0, 1, 0, 2, 0, 0, 0, 0, 0 ,0}};

    float beatTime;
    bool checkButtonInput = false;
    bool checkThrottleInput = false;
    bool checkSliderInput = false;
    float buttonInputTimer, throttleInputTimer, sliderInputTimer;
    float lowerHitBound, upperHitBound;
    public float sliderUpperVal = 4.5f, sliderLowerVal = 0.5f;
    float lastSliderVal;
    // true = right
    bool sliderGoal = true;
    public float throttleThresholdVal = 5f;

    int health = 100;

    Color highlightColor = new Color(0, 1f, 0);
    Color normalColor = new Color(1f, 0, 0);

    public Color tvColor1, tvColor2, tvColor3, tvColor4, tvColor5;
    public Color frownForegroundColor, frownBackgroundColor;

    GameObject[] regularTVs;
    GameObject[] frownTVs;
    bool frown = false;
    public int frownTickLength = 4;
    int frownTicker = 0;

    bool running = false;
    public GameObject startButton;
    public GameObject difficultySlider;
    int difficulty;
    public TextMesh difficultyText;

    public TextMesh healthTextmesh;
    Boolean firstStart = true;

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

        regularTVs = GameObject.FindGameObjectsWithTag("TV_regular");
        frownTVs = GameObject.FindGameObjectsWithTag("TV_frown");



        button.GetComponent<VRTK_Button>().events.OnPush.AddListener(ButtonInput);
        throttle.GetComponent<VRTK_Spring_Lever>().defaultEvents.OnValueChanged.AddListener(ThrottleInput);
        slider.GetComponent<VRTK_Slider>().defaultEvents.OnValueChanged.AddListener(SliderInput);
        startButton.GetComponent<VRTK_Button>().events.OnPush.AddListener(StartButtonInput);
        difficultySlider.GetComponent<VRTK_Slider>().defaultEvents.OnValueChanged.AddListener(DifficultySliderInput);
    }

    void BeginSong()
    {
        metro = gameObject.GetComponent<Metronome>();
        beatTime = 60 / metro.BPM;
        metro.OnTick += Tick;
        metro.StartMetronome();
        bgmSource.Play();
        running = true;

        lowerHitBound = beatTime / 2f;
        upperHitBound = beatTime * 3f;
    }

    void Restart()
    {
        health = 100;
        metro.StartMetronome();
        bgmSource.Play();
        running = true;
    }

    void End()
    {
        bgmSource.Stop();
        metro.StopAllCoroutines();
        metro.CurrentMeasure = 0;
        metro.CurrentStep = 0;
        metro.Base = 4;
        metro.Step = 16;
        metro.BPM = 400;
        seqPosition = 0;
        checkButtonInput = false;
        checkThrottleInput = false;
        checkSliderInput = false;
        buttonInputTimer = 0;
        throttleInputTimer = 0;
        sliderInputTimer = 0;
        running = false;
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
            ThrottleInput(-10f, 10f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartButtonInput();
        }

        if (running)
        {

            if (checkButtonInput)
            {
                //Debug.Log("Checking Button...");
                buttonInputTimer += Time.deltaTime;
                button.GetComponent<Renderer>().material.color = highlightColor;
                if (buttonInputTimer > upperHitBound)
                {
                    buttonFailure();
                }
            }
            else
            {
                button.GetComponent<Renderer>().material.color = normalColor;
                buttonInputTimer = 0f;
            }

            if (checkSliderInput)
            {
                //Debug.Log("Checking Slider...");
                sliderInputTimer += Time.deltaTime;
                slider.GetComponent<Renderer>().material.color = highlightColor;
                if (sliderInputTimer > upperHitBound)
                {
                    sliderFailure();
                }
            }
            else
            {
                slider.GetComponent<Renderer>().material.color = normalColor;
                sliderInputTimer = 0f;
            }

            if (checkThrottleInput)
            {
                //Debug.Log("Checking Throttle...");
                throttleInputTimer += Time.deltaTime;
                throttle.GetComponent<Renderer>().material.color = highlightColor;
                if (throttleInputTimer > upperHitBound)
                {
                    throttleFailure();
                }
            }
            else
            {
                throttle.GetComponent<Renderer>().material.color = normalColor;
                throttleInputTimer = 0;
            }

        }

        healthTextmesh.text = health.ToString();

        if(health <= 0)
        {
            End();
            for (int i = 0; i < regularTVs.Length; i++)
            {
                regularTVs[i].GetComponent<Renderer>().materials[1].color = frownBackgroundColor;
            }
            for (int i = 0; i < frownTVs.Length; i++)
            {
                frownTVs[i].GetComponent<Renderer>().materials[1].color = frownForegroundColor;
            }
            
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
        if(seqPosition % 2 == 0 && !frown)
        {
            for(int i = 0; i < regularTVs.Length; i++)
            {
                setRandomTVColor(regularTVs[i].GetComponent<Renderer>());
            }
            for (int i = 0; i < frownTVs.Length; i++)
            {
                setRandomTVColor(frownTVs[i].GetComponent<Renderer>());
            }
            frownTicker = 0;
        } else if (frown)
        {
            for (int i = 0; i < regularTVs.Length; i++)
            {
                regularTVs[i].GetComponent<Renderer>().materials[1].color = frownBackgroundColor;
            }
            for (int i = 0; i < frownTVs.Length; i++)
            {
                frownTVs[i].GetComponent<Renderer>().materials[1].color = frownForegroundColor;
            }
            frownTicker--;
        }
        if(frownTicker <= 0)
        {
            frown = false;
        }
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
            if(lastSliderVal < 2.5f)
            {
                sliderGoal = true;
            } else
            {
                sliderGoal = false;
            }
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

    void setRandomTVColor(Renderer rend)
    {
        int randColor = UnityEngine.Random.Range(0, 5);
        switch (randColor)
        {
            case 0:
                rend.materials[1].color = tvColor1;
                break;
            case 1:
                rend.materials[1].color = tvColor2;
                break;
            case 2:
                rend.materials[1].color = tvColor3;
                break;
            case 3:
                rend.materials[1].color = tvColor4;
                break;
            case 4:
                rend.materials[1].color = tvColor5;
                break;
        }
    }

    public void ButtonInput()
    {
        //Debug.Log("Button pressed.");
        if (buttonInputTimer >= lowerHitBound && buttonInputTimer <= upperHitBound) {
            buttonSuccess();
        } else {
            // buttonFailure();
        }
    }

    public void StartButtonInput()
    {
        if (running)
        {
            End();
        } else if (firstStart)
        {
            BeginSong();
            firstStart = false;
        } else
        {
            Restart();
        }
    }

    public void DifficultySliderInput(float val, float normalizedVal)
    {
        if (val <= 1)
        {
            difficulty = 1;
        } else if (val >= 2)
        {
            difficulty = 3;
        } else
        {
            difficulty = 2;
        }

        difficultyText.text = "difficulty: " + difficulty;
        switch (difficulty)
        {
            case 1:
                for(int i = 0; i < sequencer1.Length; i++)
                {
                    sequencer1[i] = difficulty1[0, i];
                    sequencer2[i] = difficulty1[1, i];
                    sequencer3[i] = difficulty1[2, i];
                }
                break;
            case 2:
                for (int i = 0; i < sequencer1.Length; i++)
                {
                    sequencer1[i] = difficulty2[0, i];
                    sequencer2[i] = difficulty2[1, i];
                    sequencer3[i] = difficulty2[2, i];
                }
                break;
            case 3:
                for (int i = 0; i < sequencer1.Length; i++)
                {
                    sequencer1[i] = difficulty3[0, i];
                    sequencer2[i] = difficulty3[1, i];
                    sequencer3[i] = difficulty3[2, i];
                }
                break;
        }
    }

    public void SliderInput(float val, float normalizedVal)
    {
        //Debug.Log("Slider moved.");
        //Debug.Log(val + " -- " + normalizedVal);
        lastSliderVal = val;
        if (checkSliderInput) {
            if (sliderGoal)
            {
                if (val >= sliderUpperVal)
                {
                    sliderSuccess();
                }
            } else
            {
                if (val <= sliderLowerVal)
                {
                    sliderSuccess();
                }
            }
        }
    }

    public void ThrottleInput(float val, float normalizedVal)
    {
        //Debug.Log("Throttle engaged.");
        //Debug.Log(val + " -- " + normalizedVal);
        if (checkThrottleInput)
        {
                if (normalizedVal >= throttleThresholdVal)
                {
                    throttleSuccess();
                }
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
        health -= 5;
        failureSource.PlayOneShot(failureClip, 1f);
        checkButtonInput = false;
        buttonInputTimer = 0f;
        Debug.Log("Button Failure.");
        frown = true;
        frownTicker = frownTickLength;
    }

    void sliderFailure()
    {
        health -= 5;
        failureSource.PlayOneShot(failureClip, 1f);
        checkSliderInput = false;
        sliderInputTimer = 0f;
        Debug.Log("Slider Failure.");
        frown = true;
        frownTicker = frownTickLength;
    }

    void throttleFailure()
    {
        health -= 5;
        failureSource.PlayOneShot(failureClip, 1f);
        checkThrottleInput = false;
        throttleInputTimer = 0f;
        Debug.Log("Throttle Failure.");
        frown = true;
        frownTicker = frownTickLength;
    }
}
