using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour
{
	float Hp = 100;
	bool suffocating = false;
	bool medbay = false;
	float[,] queue = new float[101, 3];
	int j = 0;
	float seconds = 0.4f; //speed
	int k;
	bool finished = true;
	bool brk = false;
	void Start ()
	{
		for (int i = 1; i <= 100; i++)
		{
			queue [i, 1] = 0;
			queue [i, 2] = 0;
		}
		StartCoroutine (Health ());
	}
	public void AddQueue(float x, float y, bool end, bool begin)
	{
		if (begin)
		{
			if (!finished)
			{
				brk = true;
			}
			else
			{
				finished = false;
			}
			j = 0;
			for (int i = 1; i <= 100; i++) {
				queue [i, 1] = 0;
				queue [i, 2] = 0;
			}
		}
		j++;
		queue [j, 1] = x;
		queue [j, 2] = y;
		if (end)
		{
			k = 1;
			StartCoroutine (goTo (k));
		}
	}
	IEnumerator goTo (int a)
	{
		k++;
		Vector3 endPos = new Vector3(queue [a, 1], queue [a, 2], 0);
		float elapsedTime = 0;
		Vector3 startingPos = transform.localPosition;
		if (endPos.x - startingPos.x > 0.1f)
		{
			GetComponent<Animator> ().SetBool ("Idle", false);
			GetComponent<Animator> ().SetBool ("Right", true);
			GetComponent<Animator> ().SetBool ("Up", false);
			GetComponent<Animator> ().SetBool ("Left", false);
			GetComponent<Animator> ().SetBool ("Down", false);
		}
		if (endPos.x - startingPos.x < -0.1f)
		{
			GetComponent<Animator> ().SetBool ("Idle", false);
			GetComponent<Animator> ().SetBool ("Right", false);
			GetComponent<Animator> ().SetBool ("Up", false);
			GetComponent<Animator> ().SetBool ("Left", true);
			GetComponent<Animator> ().SetBool ("Down", false);
		}
		if (startingPos.y - endPos.y > 0.1f)
		{
			GetComponent<Animator> ().SetBool ("Idle", false);
			GetComponent<Animator> ().SetBool ("Right", false);
			GetComponent<Animator> ().SetBool ("Up", false);
			GetComponent<Animator> ().SetBool ("Left", false);
			GetComponent<Animator> ().SetBool ("Down", true);
		}
		if (startingPos.y - endPos.y < -0.1f)
		{
			GetComponent<Animator> ().SetBool ("Idle", false);
			GetComponent<Animator> ().SetBool ("Right", false);
			GetComponent<Animator> ().SetBool ("Up", true);
			GetComponent<Animator> ().SetBool ("Left", false);
			GetComponent<Animator> ().SetBool ("Down", false);
		}
		while (elapsedTime < seconds)
		{
			transform.localPosition = Vector3.Lerp(startingPos, endPos, (elapsedTime / seconds));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			if (brk)
				break;
		}
		transform.localPosition = endPos;
		if (queue [k, 1] == 0 & queue [k, 2] == 0)
		{
			GetComponent<Animator> ().SetBool ("Idle", true);
			GetComponent<Animator> ().SetBool ("Right", false);
			GetComponent<Animator> ().SetBool ("Up", false);
			GetComponent<Animator> ().SetBool ("Left", false);
			GetComponent<Animator> ().SetBool ("Down", false);
		}
		if (queue [k, 1] != 0 & queue [k, 2] != 0 & !brk)
		{
			StartCoroutine (goTo (k));
		}
		else
		{
			finished = true;
			brk = false;
		}
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (queue [k, 1] == 0 & queue [k, 2] == 0)
		{
			if (col.tag.Contains ("RoomComp"))
			{
				GetComponent<Animator> ().SetBool ("Idle", false);
				GetComponent<Animator> ().SetBool ("Computer", true);
				GetComponent<Animator> ().SetBool ("Right", false);
				GetComponent<Animator> ().SetBool ("Up", false);
				GetComponent<Animator> ().SetBool ("Left", false);
				GetComponent<Animator> ().SetBool ("Down", false);
			}
			if (col.tag == "RoomCompR")
			{
				GetComponent<Animator> ().SetBool ("Right", true);
			}
			if (col.tag == "RoomCompL")
			{
				GetComponent<Animator> ().SetBool ("Left", true);
			}
			if (col.tag == "RoomCompD")
			{
				GetComponent<Animator> ().SetBool ("Down", true);
			}
			if (col.tag == "RoomCompU")
			{
				GetComponent<Animator> ().SetBool ("Up", true);
			}
		}
		if (col.tag.Contains ("Icon") & col.name.Contains ("IconMedbay"))
		{
			medbay = true;
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag.Contains("RoomComp"))
		{
			GetComponent<Animator> ().SetBool ("Computer", false);
		}
		if (col.tag.Contains ("Icon") & col.name.Contains ("IconMedbay"))
		{
			medbay = false;
		}
	}
	public void Suffocate(bool value)
	{
		suffocating = value;
	}
	IEnumerator Health ()
	{
		while (true)
		{
			if (suffocating & !(medbay & GameObject.Find("Ship").GetComponent<Ship>()._GetMedbayEnergy() > 0))
			{
				Hp -= 5;
			}
			if (medbay)
			{	//f(x)=1.25x^3-5x^2+8.75x
				Hp += 1.25f * (float)Math.Pow(GameObject.Find("Ship").GetComponent<Ship>()._GetMedbayEnergy(),3) - 5f * (float)Math.Pow(GameObject.Find("Ship").GetComponent<Ship>()._GetMedbayEnergy(),2) + 8.75f * GameObject.Find("Ship").GetComponent<Ship>()._GetMedbayEnergy();
			}
			if (Hp < 0)
				Destroy(gameObject);
			if (Hp > 100)
				Hp = 100;
			transform.Find ("Hp").localScale = new Vector3 (Hp * 0.006221217f, 0.5065791f, 1f);
			transform.Find ("Hp").localPosition = new Vector3 (Hp * 0.000873f - 0.0873f, 0.116f, 0f);
			yield return new WaitForSeconds ((float)0.5);
		}
	}
}
