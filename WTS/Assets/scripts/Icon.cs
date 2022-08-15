using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour {

	public Sprite BarOff1;
	public Sprite BarOn1;
	public Sprite BarDamaged1;
	public Sprite BarOff2;
	public Sprite BarOn2;
	public Sprite BarDamaged2;
	public Sprite Nothing;
	public void Change (int EnergyMax, int Energy, int EnergyDamaged)
	{
		for (int i = 0; i < 8; i++)
			gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = Nothing;
		for (int i = 0; i < EnergyMax; i++)
			if (i < 4)
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarOff1;
			}
			else
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarOff2;
			}
		for (int i = 0; i < Energy; i++)
			if (i < 4)
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarOn1;
			}
			else
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarOn2;
			}
		for (int i = EnergyMax; i < EnergyMax + EnergyDamaged; i++)
			if (i < 4)
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarDamaged1;
			}
			else
			{
				gameObject.transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = BarDamaged2;
			}
	}
}
