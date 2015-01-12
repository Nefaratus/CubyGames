using UnityEngine;
using System.Collections;

public class SoundEffects : MonoBehaviour {

	AudioSource source;
	public AudioClip ding;
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayDing()
	{
		source.PlayOneShot (ding);
	}
}
