using UnityEngine;
using System.Collections;
using System;

public class BasicSequencer : MonoBehaviour {

    public AudioSource bgmSource;
    public AudioSource testSfxSource;

    public AudioClip bgmClip = new AudioClip();
    public AudioClip testSfxClip = new AudioClip();

    Metronome metro;

    public bool[] sequencer = new bool[16];
    int seqPosition = 0;

	// Use this for initialization
	void Start () {
        bgmSource = AddAudio(bgmClip, true, false, 1f);
        testSfxSource = AddAudio(testSfxClip, false, false, 1f);
        metro = gameObject.GetComponent<Metronome>();
        metro.OnTick += Tick;
        bgmSource.Play();
	}

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.loop = loop;
        audio.playOnAwake = playAwake;
        audio.volume = vol;
        return audio;
    }

    private void Tick(Metronome metronome)
    {
        if (sequencer[seqPosition])
        {
            testSfxSource.Play();
        }
        seqPosition++;
        if (seqPosition >= sequencer.Length)
        {
            seqPosition = 0;
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
