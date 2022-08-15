using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
	public Sprite SpritePaused;
	public Sprite SpritePause;
	public void ChangeSprite (bool pause)
	{
		if (pause)
		{
			GetComponent<Image>().sprite = SpritePause;
		}
		else
		{
			GetComponent<Image>().sprite = SpritePaused;
		}
	}
}
