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
    }
	
	// Update is called once per frame
	void Update () {
	    
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
                // test for input
            }

        if (sequencer2[seqPosition] == 1) {
            switchCueSource.Play();
        } else if (sequencer2[seqPosition] == 2) {
                // test for input
            }

        if (sequencer3[seqPosition] == 1) {
            throttleCueSource.Play();
        } else if (sequencer3[seqPosition] == 2) {
                // test for input
            }

        seqPosition++;
        if (seqPosition >= sequencer1.Length)
        {
            seqPosition = 0;
        }
    }

}
