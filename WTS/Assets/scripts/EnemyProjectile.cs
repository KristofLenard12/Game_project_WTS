using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour{

	Vector2 Destination = new Vector2(3,5);
	Vector2 StartPos, EndPos;
	int ShieldPenetration = 0;
	int ShieldDamage = 1;
	int HullDamage =  1;
	int CrewDamage = 0;
	int SystemDamage = 1;
	int BreachChance = 5;
	int FireChance = 5;
	int Speed = 10;

	bool missed = false;

	public void Initialize(Vector2 _Destination, int _ShieldPenetration, int _ShieldDamage, int _HullDamage, int _CrewDamage, int _SystemDamage, int _BreachChance, int _FireChance, int _Speed)
	{
		Destination = _Destination;
		ShieldPenetration = _ShieldPenetration;
		ShieldDamage = _ShieldDamage;
		HullDamage =  _HullDamage;
		CrewDamage = _CrewDamage;
		SystemDamage = _SystemDamage;
		BreachChance = _BreachChance;
		FireChance = _FireChance;
		Speed = _Speed;
	}
	void Start ()
	{
		switch (UnityEngine.Random.Range (1, 5))
		{
		case 1:
			transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0, UnityEngine.Random.Range (1, 101) / 100f, 100)) + new Vector3 (-1, 0, 0);
			break;
		case 2:
			transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (1, UnityEngine.Random.Range (1, 101) / 100f, 100)) + new Vector3 (1, 0, 0);
			break;
		case 3:
			transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (UnityEngine.Random.Range (1, 101) / 100f, 0, 100)) + new Vector3 (0, -1, 0);
			break;
		case 4:
			transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (UnityEngine.Random.Range (1, 101) / 100f, 1, 100)) + new Vector3 (0, 1, 0);
			break;
		}
		string temp;
		if (Destination.x < 10)
			temp = "room0" + Destination.x;
		else
			temp = "room" + Destination.x;
		if (Destination.y < 10)
			temp = temp + "_0" + Destination.y;
		else
			temp = temp + "_" + Destination.y;
		EndPos = GameObject.Find ("Ship").transform.Find (temp).position;
		StartPos = transform.position;
		if ((EndPos - StartPos).x < 0)
			transform.localEulerAngles = new Vector3 (0, 0, (float)Math.Atan ((EndPos - StartPos).y / (EndPos - StartPos).x) / (float)Math.PI * 180);
		else
			transform.localEulerAngles = new Vector3 (0, 0, (float)Math.Atan ((EndPos - StartPos).y / (EndPos - StartPos).x) / (float)Math.PI * 180 + 180);
		StartCoroutine (ProjectileAnim((EndPos - StartPos).magnitude / Speed));
	}
	IEnumerator ProjectileAnim (float WaitTime)
	{
		float elapsedTime = 0;
		while (elapsedTime < WaitTime)
		{
			transform.position = Vector2.Lerp(StartPos, EndPos, (elapsedTime / WaitTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		transform.position = EndPos;
		if (!missed)
			GetComponent<CircleCollider2D> ().enabled = true;
		else
		{
			elapsedTime = 0;
			while (elapsedTime < WaitTime * 100)
			{
				transform.position = Vector2.Lerp (StartPos + (EndPos - StartPos), EndPos + (EndPos - StartPos) * 100, (elapsedTime / WaitTime / 100));
				elapsedTime += Time.deltaTime;
				if (!(transform.position.x > (Camera.main.ViewportToWorldPoint (new Vector3 (0, UnityEngine.Random.Range (1, 101) / 100f, 100)) + new Vector3 (-1, 0, 0)).x & transform.position.x < (Camera.main.ViewportToWorldPoint (new Vector3 (1, UnityEngine.Random.Range (1, 101) / 100f, 100)) + new Vector3 (1, 0, 0)).x) | !(transform.position.y > (Camera.main.ViewportToWorldPoint (new Vector3 (UnityEngine.Random.Range (1, 101) / 100f, 0, 100)) + new Vector3 (0, -1, 0)).y & transform.position.y < (Camera.main.ViewportToWorldPoint (new Vector3 (UnityEngine.Random.Range (1, 101) / 100f, 1, 100)) + new Vector3 (0, 1, 0)).y))
					Destroy (gameObject);
				yield return new WaitForEndOfFrame ();
			}
			Destroy (gameObject);
		}
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.name == "Transition")
			Destroy (gameObject);
		if (col.tag == "Shield")
		{
			if (GameObject.Find ("Ship").GetComponent<Ship> ()._GetEvade () > UnityEngine.Random.Range (1, 101))
				missed = true;
			else
				if (GameObject.Find ("Ship").GetComponent<Ship> ()._GetShield () > ShieldPenetration)
				{
					GameObject.Find ("Ship").GetComponent<Ship> ()._ShieldDamage (ShieldDamage);
					Destroy (gameObject);
				}
			GetComponent<PolygonCollider2D> ().enabled = false;
		}
		if (col.tag.Contains ("Room")) 
		{
			bool Breach = false;
			bool Fire = false;
			if (UnityEngine.Random.Range (1, 101) < BreachChance)
				Breach = true;
			if (UnityEngine.Random.Range (1, 101) < FireChance)
				Fire = true;
			GameObject.Find ("Ship").GetComponent<Ship> ()._Damage (Destination, SystemDamage, HullDamage, CrewDamage, Breach, Fire);
			Destroy (gameObject);
		}
	}
}
