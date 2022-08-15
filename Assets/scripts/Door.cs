using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	public Sprite Stage1;
	public Sprite Stage2;
	public Sprite Stage3;
	public Sprite Stage4;
	public Sprite Stage5;
	public Sprite Stage6;
	public Sprite Nothing;
	float time = 0.02f;
	bool state = true;		//closed
	int x, y, orientation;	//orientation 2=horizontal 1=vertical
	void Start()
	{
		x = int.Parse (gameObject.transform.name.Remove (0, 4).Remove (2, 6));
		y = int.Parse (gameObject.transform.name.Remove (0, 7).Remove (2, 3));
		orientation = int.Parse (gameObject.transform.name.Remove (0, 10));
	}
	void OnMouseDown()
	{
		if (state)
		{
			state = false;
			StartCoroutine (Open ());
		}
		else
		{
			state = true;
			StartCoroutine (Close ());
		}
		gameObject.transform.root.GetComponent<Ship> ().DoorChange (x, y, orientation, state);
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Crew")
		{
			StartCoroutine (Open ());
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Crew")
		{
			if (state)
			{
				StartCoroutine (Close ());
			}
		}
	}
	IEnumerator Close ()
	{
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage1;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage2;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage3;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage4;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage5;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage6;
	}
	IEnumerator Open ()
	{
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage5;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage4;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage3;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage2;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Stage1;
		yield return new WaitForSecondsRealtime (time);
		gameObject.GetComponent<SpriteRenderer> ().sprite = Nothing;
	}
}
