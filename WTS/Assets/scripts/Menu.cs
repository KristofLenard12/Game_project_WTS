using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	float MusicVolume = 0.3f;
	float FadeTime = 1f;

	void Start ()
	{
		StartCoroutine (MusicFadeIn ());
		Time.timeScale = 1;
	}
	public void NewGame(Button newgame)
	{
		Time.timeScale = 1;
		SceneManager.LoadScene ("game");
	}
	public void Options(Button options)
	{
		
	}
	public void Quit(Button quit)
	{
		Application.Quit ();
	}
	IEnumerator MusicFadeOut ()
	{
		while (GameObject.Find("Camera").GetComponent<AudioSource>().volume > 0)
		{
			GameObject.Find("Camera").GetComponent<AudioSource>().volume -= MusicVolume * Time.deltaTime / FadeTime;
			yield return null;
		}
	}
	IEnumerator MusicFadeIn ()
	{
		GameObject.Find ("Camera").GetComponent<AudioSource> ().Play ();
		while (GameObject.Find("Camera").GetComponent<AudioSource>().volume < MusicVolume)
		{
			GameObject.Find("Camera").GetComponent<AudioSource>().volume += MusicVolume * Time.deltaTime / FadeTime;
			yield return null;
		}
		GameObject.Find ("Camera").GetComponent<AudioSource> ().volume = MusicVolume;
	}
}
