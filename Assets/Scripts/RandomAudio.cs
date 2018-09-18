using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudio : MonoBehaviour {

    [SerializeField] List<AudioClip> soundtrack;
    [SerializeField] [Range(0,1)] float audioVolume = 0.2f;

	// Use this for initialization
	void Start () {
        int randomAudios = UnityEngine.Random.Range(0, soundtrack.Count-1);
        AudioSource.PlayClipAtPoint(soundtrack[randomAudios], transform.position, audioVolume);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
