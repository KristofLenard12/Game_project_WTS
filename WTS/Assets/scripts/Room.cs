using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	int x, y;
    bool filled = false;
    string crew;
	void Start()
	{
		x = int.Parse (gameObject.transform.name.Remove (0, 4).Remove (2, 3));
		y = int.Parse (gameObject.transform.name.Remove (0, 7));
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.name.Contains ("Icon"))
		{
			gameObject.transform.root.GetComponent<Ship> ()._SetRoomRole (x, y, col.name.Substring (8, col.name.Length - 8));
			col.GetComponent<Rigidbody2D> ().simulated = false;
		}
		if (col.tag == "Crew" & !filled)
		{
            crew = col.name;
            filled = true;
			gameObject.transform.root.GetComponent<Ship> ().CrewChange (x, y, int.Parse (col.gameObject.name.Remove (0, 4)), true);
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.name == crew)
		{
            filled = false;
			gameObject.transform.root.GetComponent<Ship> ().CrewChange (x, y, int.Parse (col.gameObject.name.Remove (0, 4)), false);
		}
	}
	void OnMouseDown()
	{
		gameObject.transform.root.GetComponent<Ship> ().Navigate (x, y);
	}
}
