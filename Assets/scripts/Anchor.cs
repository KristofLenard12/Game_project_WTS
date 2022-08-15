using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
	void Start()
	{
		transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,0.5f,10)) - new Vector3(2.8f,0,0);
	}
}
