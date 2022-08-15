using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BackgroundTransition : MonoBehaviour 
{
	public GameObject Space1;
	public GameObject Space2;
	public Sprite Background1;
	public Sprite Background2;
	public Sprite Background3;
	public Sprite Background4;
	public Sprite Background5;
	public Sprite Background6;
	public Sprite Background7;
	public Sprite Background8;
	public Sprite Background9;
	int rnd, last1, last2, last3, last4, last5;
	void Start()
	{
		last1 = Random.Range (1, 10);
		switch (last1)
		{
		case 1:
			Space1.GetComponent<Image>().sprite = Background1;
			break;
		case 2:
			Space1.GetComponent<Image>().sprite = Background2;
			break;
		case 3:
			Space1.GetComponent<Image>().sprite = Background3;
			break;
		case 4:
			Space1.GetComponent<Image>().sprite = Background4;
			break;
		case 5:
			Space1.GetComponent<Image>().sprite = Background5;
			break;
		case 6:
			Space1.GetComponent<Image>().sprite = Background6;
			break;
		case 7:
			Space1.GetComponent<Image>().sprite = Background7;
			break;
		case 8:
			Space1.GetComponent<Image>().sprite = Background8;
			break;
		case 9:
			Space1.GetComponent<Image>().sprite = Background9;
			break;
		}
	}
	public void Randomize(float WaitTime)
	{
		while (rnd == last1 | rnd == last2 | rnd == last3 | rnd == last4 | rnd == last5)
		{
			rnd = Random.Range (1, 10);
		}
		last5 = last4;
		last4 = last3;
		last3 = last2;
		last2 = last1;
		last1 = rnd;
		switch (last2)
		{
		case 1:
			Space1.GetComponent<Image>().sprite = Background1;
			break;
		case 2:
			Space1.GetComponent<Image>().sprite = Background2;
			break;
		case 3:
			Space1.GetComponent<Image>().sprite = Background3;
			break;
		case 4:
			Space1.GetComponent<Image>().sprite = Background4;
			break;
		case 5:
			Space1.GetComponent<Image>().sprite = Background5;
			break;
		case 6:
			Space1.GetComponent<Image>().sprite = Background6;
			break;
		case 7:
			Space1.GetComponent<Image>().sprite = Background7;
			break;
		case 8:
			Space1.GetComponent<Image>().sprite = Background8;
			break;
		case 9:
			Space1.GetComponent<Image>().sprite = Background9;
			break;
		}
		switch (last1)
		{
		case 1:
			Space2.GetComponent<Image>().sprite = Background1;
			break;
		case 2:
			Space2.GetComponent<Image>().sprite = Background2;
			break;
		case 3:
			Space2.GetComponent<Image>().sprite = Background3;
			break;
		case 4:
			Space2.GetComponent<Image>().sprite = Background4;
			break;
		case 5:
			Space2.GetComponent<Image>().sprite = Background5;
			break;
		case 6:
			Space2.GetComponent<Image>().sprite = Background6;
			break;
		case 7:
			Space2.GetComponent<Image>().sprite = Background7;
			break;
		case 8:
			Space2.GetComponent<Image>().sprite = Background8;
			break;
		case 9:
			Space2.GetComponent<Image>().sprite = Background9;
			break;
		}
		StartCoroutine (BgWarpAnim (WaitTime));

	}
	IEnumerator BgWarpAnim (float WaitTime)
	{
		Space2.transform.Find ("Transition").GetComponent<BoxCollider2D> ().enabled = true;
		float elapsedTime = 0;
		while (elapsedTime < WaitTime)
		{
			Space2.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp(new Vector2(1350, 0), new Vector2(0, 0), (elapsedTime / WaitTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Space1.GetComponent<Image> ().sprite = Space2.GetComponent<Image> ().sprite;
		Space2.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (1350, 0);
		Space2.transform.Find ("Transition").GetComponent<BoxCollider2D> ().enabled = false;
	}
}