using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
	public AudioClip Music1;
	public AudioClip Music2;
	public AudioClip Music3;
	public AudioClip Music4;
	public AudioClip Music5;
	public AudioClip Music6;
	public AudioClip Music7;
	public AudioClip Music8;
	public AudioClip Music9;
	public AudioClip Music10;
	public AudioClip Music11;
	public AudioClip Music12;
	int rnd, last1, last2, last3, last4, last5;
	void Start()
	{
		last1 = Random.Range (1, 13);
		switch (last1)
		{
		case 1:
			GetComponent<AudioSource> ().clip = Music1;
			break;
		case 2:
			GetComponent<AudioSource> ().clip = Music2;
			break;
		case 3:
			GetComponent<AudioSource> ().clip = Music3;
			break;
		case 4:
			GetComponent<AudioSource> ().clip = Music4;
			break;
		case 5:
			GetComponent<AudioSource> ().clip = Music5;
			break;
		case 6:
			GetComponent<AudioSource> ().clip = Music6;
			break;
		case 7:
			GetComponent<AudioSource> ().clip = Music7;
			break;
		case 8:
			GetComponent<AudioSource> ().clip = Music8;
			break;
		case 9:
			GetComponent<AudioSource> ().clip = Music9;
			break;
		case 10:
			GetComponent<AudioSource> ().clip = Music10;
			break;
		case 11:
			GetComponent<AudioSource> ().clip = Music11;
			break;
		case 12:
			GetComponent<AudioSource> ().clip = Music12;
			break;
		}
		GetComponent<AudioSource> ().Play ();
	}
	public void Randomize()
	{
		while (rnd == last1 | rnd == last2 | rnd == last3 | rnd == last4 | rnd == last5)
		{
			rnd = Random.Range (1, 13);
		}
		last5 = last4;
		last4 = last3;
		last3 = last2;
		last2 = last1;
		last1 = rnd;
		switch (last1)
		{
		case 1:
			GetComponent<AudioSource> ().clip = Music1;
			break;
		case 2:
			GetComponent<AudioSource> ().clip = Music2;
			break;
		case 3:
			GetComponent<AudioSource> ().clip = Music3;
			break;
		case 4:
			GetComponent<AudioSource> ().clip = Music4;
			break;
		case 5:
			GetComponent<AudioSource> ().clip = Music5;
			break;
		case 6:
			GetComponent<AudioSource> ().clip = Music6;
			break;
		case 7:
			GetComponent<AudioSource> ().clip = Music7;
			break;
		case 8:
			GetComponent<AudioSource> ().clip = Music8;
			break;
		case 9:
			GetComponent<AudioSource> ().clip = Music9;
			break;
		case 10:
			GetComponent<AudioSource> ().clip = Music10;
			break;
		case 11:
			GetComponent<AudioSource> ().clip = Music11;
			break;
		case 12:
			GetComponent<AudioSource> ().clip = Music12;
			break;
		}
	}
}