using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour
{
	public GameObject EnemyProjectile;
	public Sprite PowerBarOn;
	public Sprite PowerBarOff;
	public Sprite Nothing;
	public Sprite ShieldOn;
	public Sprite ShieldOff;
	public GameObject MenuUI;
	public GameObject Console;
	public GameObject UpgradeUI;

	float MusicVolume = 0.3f;

	int HullHp = 30;
	int Scrap = 1000;
	int ShieldMax = 1;
	int Shield = 0;
	float ShieldRegenRate = 2f;
	int Fuel = 1000;
	int Ammo = 3;
	int DronePart = 0;
	float Evade = 10;
	int Oxygen = 100;
	int Energy = 4;
	int EnergyMax = 8;
	int ShieldEnergy = 2;
	int EngineEnergy = 1;
	int OxygenEnergy = 1;
	int MedbayEnergy = 0;
	int WeaponEnergy = 0;
	int ShieldEnergyMax = 2;
	int EngineEnergyMax = 2;
	int OxygenEnergyMax = 1;
	int MedbayEnergyMax = 1;
	int WeaponEnergyMax = 3;
	int CockpitEnergyMax = 1;
	int DoorEnergyMax = 1;
	int ShieldEnergyDamaged = 0;
	int EngineEnergyDamaged = 0;
	int OxygenEnergyDamaged = 0;
	int MedbayEnergyDamaged = 0;
	int WeaponEnergyDamaged = 0;
	int CockpitEnergyDamaged = 0;
	int DoorEnergyDamaged = 0;
	int ShieldCrew = 0;
	int EngineCrew = 0;
	int OxygenCrew = 0;
	int MedbayCrew = 0;
	int WeaponCrew = 0;
	int CockpitCrew = 0;
	int DoorCrew = 0;
	bool Combat = false;
	float WarpTime = 1;
	float WarpWaitTime = 0.25f;
	float DoubleClickDelay = 0.2f;
	float RepairRate =10f;
	float WarningTime = 4f;

	int ShipWidth;
	int ShipHeight;

	bool pause = false;                                 //game paused
	int selected = 0;									//selected crew
	int rooms = 0;										//total number of rooms
	int w, h;											//width+3, height+3
	ship_properties[,] ship;							//ship[wall_x, wall_y, property]
	cost[] costs;

	struct ship_properties
	{
		public bool room;
		public bool breach;
		public bool wall_left;
		public bool wall_top;
		public bool wall_right;
		public bool wall_bottom;
		public bool door_left;
		public bool door_top;
		public bool door_right;
		public bool door_bottom;
		public int crew;
		public int crew_target;
		public int fire;
		public float oxigen;
		public int space;
		public int path;
		public float x;
		public float y;
		public string role;
	}
	struct cost
	{
		public string Shield;
		public string Engine;
		public string Oxygen;
		public string Medbay;
		public string Weapon;
		public string Cockpit;
		public string Door;
	}

	//initialize

	public void InitializeWalls (int a, int b, string orientation)	//initialize walls
	{
		switch (orientation)
		{
		case "left":
			ship [a, b].wall_left = true;
			ship [a, b].door_left = true;
			break;
		case "top":
			ship [a, b].wall_top = true;
			ship [a, b].door_top = true;
			break;
		case "right":
			ship [a, b].wall_right = true;
			ship [a, b].door_right = true;
			break;
		case "bottom":
			ship [a, b].wall_bottom = true;
			ship [a, b].door_bottom = true;
			break;
		}
	}
	public void InitializeDoors ()									//initialize doors
	{
		int x, y, orientation;
		int children = transform.childCount;
		for (int i = 0; i < children; ++i)
		{
			if (transform.GetChild (i).tag == "Door")
			{
				x = int.Parse (transform.GetChild (i).name.Remove (0, 4).Remove (2, 6));
				y = int.Parse (transform.GetChild (i).name.Remove (0, 7).Remove (2, 3));
				orientation = int.Parse (transform.GetChild (i).name.Remove (0, 10));
				switch (orientation)
				{
				case 1:			//vertical
					ship [x, y].wall_left = false;
					ship [x - 1, y].wall_right = false;
					break;
				case 2:			//horizontal
					ship [x, y].wall_top = false;
					ship [x, y - 1].wall_bottom = false;
					break;
				}
			}
		}
	}
	public void Initialize (int a, int b)
	{
		ship = new ship_properties[a, b];				//initialize ship size (a width+3, b height+3)
		ShipWidth = a;
		ShipHeight = b;
		w = a - 1;										//with+2
		h = b - 1;										//heght+2
		for(int i = 1; i <= w; i++)						//initialize ship propertys
		{
			for(int j = 1; j <= h; j++)
			{
				ship [i, j].room = false;
				ship [i, j].breach = false;
				ship [i, j].wall_left = false;
				ship [i, j].wall_top = false;
				ship [i, j].wall_right = false;
				ship [i, j].wall_bottom = false;
				ship [i, j].door_left = false;
				ship [i, j].door_top = false;
				ship [i, j].door_right = false;
				ship [i, j].door_bottom = false;
				ship [i, j].crew = 0;
				ship [i, j].crew_target = 0;
				ship [i, j].fire = 0;
				ship [i, j].oxigen = 100;
				ship [i, j].space = 0;
				ship [i, j].path = 0;
				ship [i, j].x = 0;
				ship [i, j].y = 0;
			}
		}
		int x, y;										//initialize ship rooms
		int children = transform.childCount;
		for (int i = 0; i < children; ++i)
		{                                               //find rooms in children
			if (transform.GetChild (i).name.Contains("room"))
			{
				x = int.Parse (transform.GetChild (i).name.Remove (0, 4).Remove (2, 3));
				y = int.Parse (transform.GetChild (i).name.Remove (0, 7));
				ship [x, y].room = true;
				ship [x, y].x = transform.GetChild (i).transform.localPosition.x;
				ship [x, y].y = transform.GetChild (i).transform.localPosition.y;
				rooms++;
			}
		}
		StartCoroutine (OxygenSys());						//oxigen system
		CombatChange();                                  //combat arrangement
		HullHpChange ();
		ScrapChange ();
		FuelChange ();
		AmmoChange ();
		DronePartChange ();
		EvadeChange ();
		OxygenChange ();
		ShieldChange ();
		EnergyChange ();
		CockpitEnergyChange ();
		DoorEnergyChange ();
		costs = new cost[14];
		for (int i = 0; i < 14; i++)
		{
			costs [i].Shield = "-";
			costs [i].Engine = "-";
			costs [i].Oxygen = "-";
			costs [i].Medbay = "-";
			costs [i].Weapon = "-";
			costs [i].Cockpit = "-";
			costs [i].Door = "-";
		}
		costs [3].Shield = "20";
		costs [4].Shield = "30";
		costs [5].Shield = "40";
		costs [6].Shield = "60";
		costs [7].Shield = "80";
		costs [8].Shield = "100";
		costs [3].Engine = "15";
		costs [4].Engine = "30";
		costs [5].Engine = "40";
		costs [6].Engine = "60";
		costs [7].Engine = "80";
		costs [8].Engine = "120";
		costs [3].Weapon = "25";
		costs [4].Weapon = "35";
		costs [5].Weapon = "50";
		costs [6].Weapon = "75";
		costs [7].Weapon = "90";
		costs [8].Weapon = "100";
		costs [2].Oxygen = "25";
		costs [3].Oxygen = "50";
		costs [2].Medbay = "35";
		costs [3].Medbay = "60";
		costs [2].Cockpit = "20";
		costs [3].Cockpit = "50";
		//costs [2].Door = "20";
		//costs [3].Door = "50";
		StartCoroutine (MusicFadeIn ());
	}

	//ship

	public void DoorChange(int x,int y, int orientation, bool state)	//door system
	{
		switch (orientation)
		{
		case 1:			//vertical
			ship [x, y].door_left = state;
			ship [x - 1, y].door_right = state;
			break;
		case 2:			//horizontal
			ship [x, y].door_top = state;
			ship [x, y - 1].door_bottom = state;
			break;
		}
		SpaceChange ();									//start oxigen sub system
	}
	void SpaceChange()									//oxigen sub system
	{
		int temp1, temp2, temp3, temp4;				//store some values until comparing them
		for(int i = 2; i <= w-1; i++)					//write everithing to 0
		{
			for(int j = 2; j <= h-1; j++)
			{
				ship [i, j].space = 0;
			}
		}
		for (int k = 1; k <= w+h-2; k++)				//maximal refresh times (w+h-1)
		{
			for (int i = 2; i <= w-1; i++)				//refresh all rooms
			{
				for (int j = 2; j <= h - 1; j++)
				{
					if (ship [i, j].room)			//if room present
					{
						temp1 = 0;
						temp2 = 0;
						temp3 = 0;
						temp4 = 0;						//if brech or outer space contact, add vacum
						if (ship [i, j].breach | (!ship [i, j].door_left & !ship [i - 1, j].room) | (!ship [i, j].door_top & !ship [i, j - 1].room) | (!ship [i, j].door_right & !ship [i + 1, j].room) | (!ship [i, j].door_bottom & !ship [i, j + 1].room))
						{
							ship [i, j].space = 100;
						}								//if vacume in next room, add vacum
						if (ship [i - 1, j].space > ship [i, j].space & !ship [i, j].door_left)
						{
							temp1 = ship [i - 1, j].space - 1;
						}
						if (ship [i, j - 1].space > ship [i, j].space & !ship [i, j].door_top)
						{
							temp2 = ship [i, j - 1].space - 1;
						}
						if (ship [i + 1, j].space > ship [i, j].space & !ship [i, j].door_right)
						{
							temp3 = ship [i + 1, j].space - 1;
						}
						if (ship [i, j + 1].space > ship [i, j].space & !ship [i, j].door_bottom)
						{
							temp4 = ship [i, j + 1].space - 1;
						}								//compare values
						if (temp1 >= temp2 & temp1 >= temp3 & temp1 >= temp4 & temp1 != 0)
						{
							ship [i, j].space = temp1;
						}
						if (temp2 >= temp1 & temp2 >= temp3 & temp2 >= temp4 & temp2 != 0)
						{
							ship [i, j].space = temp2;
						}
						if (temp3 >= temp2 & temp3 >= temp1 & temp3 >= temp4 & temp3 != 0)
						{
							ship [i, j].space = temp3;
						}
						if (temp4 >= temp2 & temp4 >= temp3 & temp4 >= temp1 & temp4 != 0)
						{
							ship [i, j].space = temp4;
						}
					}
				}
			}
		}
	}
	IEnumerator OxygenSys()								//oxigen system
	{
		string temp1, temp2;							//shitty object naming solution variables
		int SummOxygen;
		while (true)
		{												//refresh all rooms
			SummOxygen = 0;
			for (int i = 2; i <= w-1; i++)
			{
				for (int j = 2; j <= h-1; j++)
				{										//if room present
					if (ship [i, j].room)
					{									//if vacum present
						if (ship [i, j].space != 0)
						{								//subtract oxigen
							ship [i, j].oxigen -= 10;
						}								//add oxigen
						else
						{	//f(x)=0.0833333x^3 + 0.3x^2 + 1.71667x − 0.5
							ship [i, j].oxigen += 0.0833333f * (float)Math.Pow(OxygenEnergy,3) + 0.3f * (float)Math.Pow(OxygenEnergy,2) + 1.71667f * OxygenEnergy - 0.5f;
						}
						if (ship [i, j].oxigen < 0)			//create limit (0-100)
						{
							ship [i, j].oxigen = 0;
						}
						if (ship [i, j].oxigen > 100)
						{
							ship [i, j].oxigen = 100;
						}								//shitty object naming solutin
						if (i >= 10)
						{
							temp1 = "room";
						}
						else
						{
							temp1 = "room0";
						}
						if (j >= 10)
						{
							temp2 = "_";
						}
						else
						{
							temp2 = "_0";
						}								//change room color
						gameObject.transform.Find (temp1 + i.ToString () + temp2 + j.ToString ()).GetComponent<SpriteRenderer> ().color = new Color32 ((byte)255, (byte)(155 + ship [i, j].oxigen), (byte)(155 + ship [i, j].oxigen), 255);
						SummOxygen += Convert.ToInt32(ship [i, j].oxigen);
						if (ship [i, j].crew != 0)
						{
							if (ship [i, j].oxigen < 5)
							{
								gameObject.transform.Find ("crew" + ship [i, j].crew.ToString ()).GetComponent<Crew> ().Suffocate (true);
							}
							else
							{
								gameObject.transform.Find ("crew" + ship [i, j].crew.ToString ()).GetComponent<Crew> ().Suffocate (false);
							}
						}
					}
				}
			}											//wait half sec
			Oxygen = Convert.ToInt32(SummOxygen/rooms);
			OxygenChange ();
			yield return new WaitForSeconds ((float)0.5);
		}
	}
	public void CrewChange(int x, int y, int crew, bool state)
	{													//crew status change in room
		if (state)
		{
			ship [x, y].crew = crew;
			switch (ship [x, y].role)
			{
			case "Shield":
				ShieldCrew++;
				break;
			case "Engine":
				EngineCrew++;
				break;
			case "Oxygen":
				OxygenCrew++;
				break;
			case "Medbay":
				MedbayCrew++;
				break;
			case "Weapon":
				WeaponCrew++;
				break;
			case "Cockpit":
				CockpitCrew++;
				break;
			case "Door":
				DoorCrew++;
				break;
			}
		}
		else
		{
			ship [x, y].crew = 0;
			switch (ship [x, y].role)
			{
			case "Shield":
				ShieldCrew--;
				break;
			case "Engine":
				EngineCrew--;
				break;
			case "Oxygen":
				OxygenCrew--;
				break;
			case "Medbay":
				MedbayCrew--;
				break;
			case "Weapon":
				WeaponCrew--;
				break;
			case "Cockpit":
				CockpitCrew--;
				break;
			case "Door":
				DoorCrew--;
				break;
			}
		}
		//Debug.Log (ShieldCrew + " " + EngineCrew + " " + OxygenCrew + " " + MedbayCrew + " " + WeaponCrew);
	}
	public void Navigate(int x, int y)					//a room has been clicked
	{
		int a, b;
		if (selected == 0 & ship [x, y].crew != 0)		//if no room selected & crew present
		{
			selected = ship [x, y].crew;					//select room as target
		}
		if (selected != 0 & ship [x, y].crew == 0 & ship [x, y].crew_target == 0)		//if selected room & no crew present
		{
			int temp1, temp2, temp3, temp4;
			for (int i = 2; i <= w - 1; i++)
			{
				for (int j = 2; j <= h - 1; j++)
				{
					ship [i, j].path = 0;				//set navigation values to 0
				}
			}
			for (int k = 1; k <= w + h - 2; k++)		//refresh w+h times
			{
				for (int i = 2; i <= w - 1; i++)
				{
					for (int j = 2; j <= h - 1; j++)
					{
						if (ship [i, j].room)		//if room present
						{
							temp1 = 0;
							temp2 = 0;
							temp3 = 0;
							temp4 = 0;
							ship [x, y].path = 100;		//set target room navgation value to 100
							for (int m = 2; m <= w - 1; m++)
							{
								for (int n = 2; n <= h - 1; n++)
								{
									if (ship [m, n].crew_target == selected)
										ship [m, n].crew_target = 0;
								}
							}
							ship [x, y].crew_target = selected;
							if (ship [i - 1, j].path > ship [i, j].path & !ship [i, j].wall_left)
							{							//if nav value bigger on left & no left wall present
								temp1 = ship [i - 1, j].path - 1;
							}							//set temp1 to left-1 nav value
							if (ship [i, j - 1].path > ship [i, j].path & !ship [i, j].wall_top)
							{							//if nav value bigger on bottom & bottom left wall present
								temp2 = ship [i, j - 1].path - 1;
							}							//set temp2 to bottom-1 nav value
							if (ship [i + 1, j].path > ship [i, j].path & !ship [i, j].wall_right)
							{							//if nav value bigger on right & right left wall present
								temp3 = ship [i + 1, j].path - 1;
							}							//set temp3 to right-1 nav value
							if (ship [i, j + 1].path > ship [i, j].path & !ship [i, j].wall_bottom)
							{							//if nav value bigger on top & no top wall present
								temp4 = ship [i, j + 1].path - 1;
							}							//set temp4 to top-1 nav value
							if (temp1 >= temp2 & temp1 >= temp3 & temp1 >= temp4 & temp1 != 0)
							{
								ship [i, j].path = temp1;
							}
							if (temp2 >= temp1 & temp2 >= temp3 & temp2 >= temp4 & temp2 != 0)
							{
								ship [i, j].path = temp2;
							}
							if (temp3 >= temp2 & temp3 >= temp1 & temp3 >= temp4 & temp3 != 0)
							{
								ship [i, j].path = temp3;
							}
							if (temp4 >= temp2 & temp4 >= temp3 & temp4 >= temp1 & temp4 != 0)
							{
								ship [i, j].path = temp4;
							}							//compare temps & set nav value to the biggest
						}
					}
				}
			}
			a = 1;
			b = 1;
			bool crewPresent = false;
			for (int i = 2; i <= w - 1; i++)
			{
				for (int j = 2; j <= h - 1; j++)
				{
					if (ship [i, j].crew == selected)
					{
						a = i;
						b = j;
						crewPresent = true;
					}
				}
			}
			bool begin = true;
			int fos = 0;
			while (ship [a, b].path != 100 & crewPresent & fos < 10000)
			{
				fos++;
				bool got = false;
				int temp = 0;
				if (ship [a - 1, b].path == ship [a, b].path + 1 & !got & !ship [a, b].wall_left)
				{
					got = true;
					if (ship [a, b].path != 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a - 1, b].x, ship [a - 1, b].y, false, false);
					}
					if (ship [a, b].path == 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a - 1, b].x, ship [a - 1, b].y, true, false);
					}
					if (ship [a, b].path == 99 & begin)
					{
						begin = false;
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a - 1, b].x, ship [a - 1, b].y, true, true);
					}
					if (ship [a, b].path != 99 & begin)
					{
						begin = false;
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a - 1, b].x, ship [a - 1, b].y, false, true);
					}
					temp = 6;
				}
				if (ship [a, b - 1].path == ship [a, b].path + 1 & !got & !ship [a, b].wall_top)
				{
					got = true;
					if (ship [a, b].path != 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b - 1].x, ship [a, b - 1].y, false, false);
					}
					if (ship [a, b].path == 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b - 1].x, ship [a, b - 1].y, true, false);
					}
					if (ship [a, b].path == 99 & begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b - 1].x, ship [a, b - 1].y, true, true);
						begin = false;
					}
					if (ship [a, b].path != 99 & begin)
					{
						begin = false;
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b - 1].x, ship [a, b - 1].y, false, true);
					}
					temp = 7;
				}
				if (ship [a + 1, b].path == ship [a, b].path + 1 & !got & !ship [a, b].wall_right)
				{
					got = true;
					if (ship [a, b].path != 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a + 1, b].x, ship [a + 1, b].y, false, false);
					}
					if (ship [a, b].path == 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a + 1, b].x, ship [a + 1, b].y, true, false);
					}
					if (ship [a, b].path == 99 & begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a + 1, b].x, ship [a + 1, b].y, true, true);
						begin = false;
					}
					if (ship [a, b].path != 99 & begin)
					{
						begin = false;
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a + 1, b].x, ship [a + 1, b].y, false, true);
					}
					temp = 8;
				}
				if (ship [a, b + 1].path == ship [a, b].path + 1 & !got & !ship [a, b].wall_bottom)
				{
					got = true;
					if (ship [a, b].path != 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b + 1].x, ship [a, b + 1].y, false, false);
					}
					if (ship [a, b].path == 99 & !begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b + 1].x, ship [a, b + 1].y, true, false);
					}
					if (ship [a, b].path == 99 & begin)
					{
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b + 1].x, ship [a, b + 1].y, true, true);
						begin = false;
					}
					if (ship [a, b].path != 99 & begin)
					{
						begin = false;
						gameObject.transform.Find ("crew" + selected.ToString ()).GetComponent<Crew> ().AddQueue (ship [a, b + 1].x, ship [a, b + 1].y, false, true);
					}
					temp = 9;
				}
				switch (temp)
				{
				case 6:
					a--;
					break;
				case 7:
					b--;
					break;
				case 8:
					a++;
					break;
				case 9:
					b++;
					break;
				}
			}
			selected = 0;
		}
	}
		
	//void Update (key detection)

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			PauseGame();
		}
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			MenuUI.SetActive (true);
			Time.timeScale = 0;
			GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Paused";
		}
		if (Shield < ShieldEnergy / 2 & !ShieldRegen)
			StartCoroutine (ShieldCharge ());
		if (ShieldEnergyDamaged > 0 & !ShieldRepairing)
			StartCoroutine (ShieldRepair ());
		if (EngineEnergyDamaged > 0 & !EngineRepairing)
			StartCoroutine (EngineRepair ());
		if (OxygenEnergyDamaged > 0 & !OxygenRepairing)
			StartCoroutine (OxygenRepair ());
		if (MedbayEnergyDamaged > 0 & !MedbayRepairing)
			StartCoroutine (MedbayRepair ());
		if (WeaponEnergyDamaged > 0 & !WeaponRepairing)
			StartCoroutine (WeaponRepair ());
		if (CockpitEnergyDamaged > 0 & !CockpitRepairing)
			StartCoroutine (CockpitRepair ());
		if (DoorEnergyDamaged > 0 & !DoorRepairing)
			StartCoroutine (DoorRepair ());
	}
	bool ShieldRegen = false;
	int ShieldPercent = 0;
	IEnumerator ShieldCharge()
	{
		ShieldRegen = true;
		ShieldPercent = 0;
		while (ShieldPercent < ShieldRegenRate * 10)
		{
			GameObject.Find ("ShieldPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(ShieldPercent / (float)ShieldRegenRate * 10, 0)) + "%");
			ShieldPercent ++;
			yield return new WaitForSeconds (0.1f);
		}
		Shield++;
		ShieldChange ();
		ShieldRegen = false;
		GameObject.Find ("ShieldPercent").GetComponent<Text> ().text = ("");
	}
	bool ShieldRepairing = false;
	int ShieldRepairPercent = 0;
	IEnumerator  ShieldRepair()
	{
		ShieldRepairing = true;
		ShieldRepairPercent = 0;
		while (ShieldRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("ShieldRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(ShieldRepairPercent / (float)RepairRate * 10, 0)) + "%");
			ShieldRepairPercent += ShieldCrew;
			yield return new WaitForSeconds (0.1f);
		}
		ShieldEnergyDamaged--;
		ShieldEnergyMax++;
		ShieldEnergyChange ();
		ShieldRepairing = false;
		GameObject.Find ("ShieldRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool EngineRepairing = false;
	int EngineRepairPercent = 0;
	IEnumerator  EngineRepair()
	{
		EngineRepairing = true;
		EngineRepairPercent = 0;
		while (EngineRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("EngineRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(EngineRepairPercent / (float)RepairRate * 10, 0)) + "%");
			EngineRepairPercent += EngineCrew;
			//Debug.Log (EngineCrew);
			yield return new WaitForSeconds (0.1f);
		}
		EngineEnergyDamaged--;
		EngineEnergyMax++;
		EngineEnergyChange ();
		EngineRepairing = false;
		GameObject.Find ("EngineRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool OxygenRepairing = false;
	int OxygenRepairPercent = 0;
	IEnumerator  OxygenRepair()
	{
		OxygenRepairing = true;
		OxygenRepairPercent = 0;
		while (OxygenRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("OxygenRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(OxygenRepairPercent / (float)RepairRate * 10, 0)) + "%");
			OxygenRepairPercent += OxygenCrew;
			yield return new WaitForSeconds (0.1f);
		}
		OxygenEnergyDamaged--;
		OxygenEnergyMax++;
		OxygenEnergyChange ();
		OxygenRepairing = false;
		GameObject.Find ("OxygenRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool MedbayRepairing = false;
	int MedbayRepairPercent = 0;
	IEnumerator  MedbayRepair()
	{
		MedbayRepairing = true;
		MedbayRepairPercent = 0;
		while (MedbayRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("MedbayRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(MedbayRepairPercent / (float)RepairRate * 10, 0)) + "%");
			MedbayRepairPercent += MedbayCrew;
			yield return new WaitForSeconds (0.1f);
		}
		MedbayEnergyDamaged--;
		MedbayEnergyMax++;
		MedbayEnergyChange ();
		MedbayRepairing = false;
		GameObject.Find ("MedbayRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool WeaponRepairing = false;
	int WeaponRepairPercent = 0;
	IEnumerator  WeaponRepair()
	{
		WeaponRepairing = true;
		WeaponRepairPercent = 0;
		while (WeaponRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("WeaponRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(WeaponRepairPercent / (float)RepairRate * 10, 0)) + "%");
			WeaponRepairPercent += WeaponCrew;
			yield return new WaitForSeconds (0.1f);
		}
		WeaponEnergyDamaged--;
		WeaponEnergyMax++;
		WeaponEnergyChange ();
		WeaponRepairing = false;
		GameObject.Find ("WeaponRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool CockpitRepairing = false;
	int CockpitRepairPercent = 0;
	IEnumerator  CockpitRepair()
	{
		CockpitRepairing = true;
		CockpitRepairPercent = 0;
		while (CockpitRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("CockpitRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(CockpitRepairPercent / (float)RepairRate * 10, 0)) + "%");
			CockpitRepairPercent += CockpitCrew;
			yield return new WaitForSeconds (0.1f);
		}
		CockpitEnergyDamaged--;
		CockpitEnergyMax++;
		CockpitEnergyChange ();
		CockpitRepairing = false;
		GameObject.Find ("CockpitRepairPercent").GetComponent<Text> ().text = ("");
	}
	bool DoorRepairing = false;
	int DoorRepairPercent = 0;
	IEnumerator  DoorRepair()
	{
		DoorRepairing = true;
		DoorRepairPercent = 0;
		while (DoorRepairPercent < RepairRate * 10)
		{
			GameObject.Find ("DoorRepairPercent").GetComponent<Text> ().text = (Convert.ToString(Math.Round(DoorRepairPercent / (float)RepairRate * 10, 0)) + "%");
			DoorRepairPercent += DoorCrew;
			yield return new WaitForSeconds (0.1f);
		}
		DoorEnergyDamaged--;
		DoorEnergyMax++;
		DoorEnergyChange ();
		DoorRepairing = false;
		GameObject.Find ("DoorRepairPercent").GetComponent<Text> ().text = ("");
	}

	//UI default

	void CombatChange()
	{
		if (Combat)
		{
			transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.46f, 100)) + new Vector3 (5.8f, 0, 0);
			transform.localScale = new Vector3 (2f, 2f, 1);
			GameObject.Find ("Hostile").GetComponent<SpriteRenderer> ().enabled = true;
		}
		else
		{
			transform.localPosition = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 100));
			transform.localScale = new Vector3 (2.1f, 2.1f, 1);
			GameObject.Find ("Hostile").GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
	bool Warping = false;
	public void Warp(Button button)
	{
		if (EngineEnergy > 0 & Fuel > 0 & !Warping & CockpitEnergyMax > 0)
		{
			Combat = false;
			CombatChange ();
			StartCoroutine (ShipWarpAnim ());
			Fuel--;
			if (pause) {
				GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Pause";
				Time.timeScale = 1;
			}
			pause = !pause;
		}
		else
		{
			StartCoroutine (WarpWarningActivate ());
		}
	}
	IEnumerator WarpWarningActivate()
	{
		ChangeWarpWarning (false, "No Fuel");
		ChangeWarpWarning (false, "Engine Error");
		ChangeWarpWarning (false, "Cockpit Error");
		bool fue = false;
		bool eng = false;
		bool coc = false;
		if (Fuel == 0)
		{
			fue = true;
			ChangeWarpWarning (true, "No Fuel");
		}
		if (EngineEnergy == 0)
		{
			eng = true;
			ChangeWarpWarning (true, "Engine Error");
		}
		if (CockpitEnergyMax == 0)
		{
			coc = true;
			ChangeWarpWarning (true, "Cockpit Error");
		}
		yield return new WaitForSecondsRealtime (WarningTime);
		if (fue)
			ChangeWarpWarning (false, "No Fuel");
		if (eng)
			ChangeWarpWarning (false, "Engine Error");
		if (coc)
			ChangeWarpWarning (false, "Cockpit Error");
	}
	IEnumerator ShipWarpAnim ()
	{
		Warping = true;
		Vector3 StartSize = gameObject.transform.localScale;
		float elapsedTime = 0;
		StartCoroutine (MusicFadeOut ());
		while (elapsedTime < WarpTime)
		{
			gameObject.transform.localScale = Vector3.Lerp(StartSize, new Vector3(0.3f, 0.3f, 1), (elapsedTime / WarpTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		gameObject.transform.localScale = new Vector3 (0.3f, 0.3f, 0);
		GameObject.Find ("Background").GetComponent<BackgroundTransition> ().Randomize (WarpWaitTime * 2);
		yield return new WaitForSecondsRealtime (WarpWaitTime);
		GameObject.Find ("Camera").GetComponent<Audio> ().Randomize ();
		FuelChange ();
		yield return new WaitForSecondsRealtime (WarpWaitTime);
		StartCoroutine (MusicFadeIn ());
		elapsedTime = 0;
		while (elapsedTime < WarpTime)
		{
			gameObject.transform.localScale = Vector3.Lerp(new Vector3(0.3f, 0.3f, 1), StartSize, (elapsedTime / WarpTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		gameObject.transform.localScale = StartSize;
		Warping = false;
	}
	public void Paused(Button button)
	{
		if (!Warping)
		{
			PauseGame ();
		}
	}
	void PauseGame()
	{
		if (pause)
		{
			GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Pause";
			Time.timeScale = 1;
		}
		else
		{
			GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Paused";
			Time.timeScale = 0;
		}
		pause = !pause;
	}
	public void Menu(Button button)
	{
		MenuUI.SetActive (true);
		Time.timeScale = 0;
		GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Paused";
	}
	bool ShieldClicked = false;
	bool ShieldClickedDouble = false;
	public void ClickedShield(Button button)
	{
		StartCoroutine (DoubleClickedShield ());
	}
	IEnumerator DoubleClickedShield ()
	{
		if (ShieldClicked)
		{
			ShieldClickedDouble = true;
			ShieldClicked = false;
			if (Energy < EnergyMax & ShieldEnergy > 0)
			{
				Energy++;
				ShieldEnergy--;
			}
		}
		ShieldClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		ShieldClicked = false;
		if (!ShieldClickedDouble)
		{
			ShieldClickedDouble = false;
			ShieldClicked = false;
			if (Energy > 0 & ShieldEnergy < ShieldEnergyMax)
			{
				Energy--;
				ShieldEnergy++;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		ShieldClickedDouble = false;
		EnergyChange ();
	}
	bool EngineClicked = false;
	bool EngineClickedDouble = false;
	public void ClickedEngine(Button button)
	{
		StartCoroutine (DoubleClickedEngine ());
	}
	IEnumerator DoubleClickedEngine ()
	{
		if (EngineClicked)
		{
			EngineClickedDouble = true;
			EngineClicked = false;
			if (Energy < EnergyMax & EngineEnergy > 0)
			{
				Energy++;
				EngineEnergy--;
			}
		}
		EngineClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		EngineClicked = false;
		if (!EngineClickedDouble)
		{
			EngineClickedDouble = false;
			EngineClicked = false;
			if (Energy > 0 & EngineEnergy < EngineEnergyMax)
			{
				Energy--;
				EngineEnergy++;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		EngineClickedDouble = false;
		EnergyChange ();
	}
	bool OxygenClicked = false;
	bool OxygenClickedDouble = false;
	public void ClickedOxygen(Button button)
	{
		StartCoroutine (DoubleClickedOxygen ());
	}
	IEnumerator DoubleClickedOxygen ()
	{
		if (OxygenClicked)
		{
			OxygenClickedDouble = true;
			OxygenClicked = false;
			if (Energy < EnergyMax & OxygenEnergy > 0)
			{
				Energy++;
				OxygenEnergy--;
			}
		}
		OxygenClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		OxygenClicked = false;
		if (!OxygenClickedDouble)
		{
			OxygenClickedDouble = false;
			OxygenClicked = false;
			if (Energy > 0 & OxygenEnergy < OxygenEnergyMax)
			{
				Energy--;
				OxygenEnergy++;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		OxygenClickedDouble = false;
		EnergyChange ();
	}
	bool MedbayClicked = false;
	bool MedbayClickedDouble = false;
	public void ClickedMedbay(Button button)
	{
		StartCoroutine (DoubleClickedMedbay ());
	}
	IEnumerator DoubleClickedMedbay ()
	{
		if (MedbayClicked)
		{
			MedbayClickedDouble = true;
			MedbayClicked = false;
			if (Energy < EnergyMax & MedbayEnergy > 0)
			{
				Energy++;
				MedbayEnergy--;
			}
		}
		MedbayClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		MedbayClicked = false;
		if (!MedbayClickedDouble)
		{
			MedbayClickedDouble = false;
			MedbayClicked = false;
			if (Energy > 0 & MedbayEnergy < MedbayEnergyMax)
			{
				Energy--;
				MedbayEnergy++;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		MedbayClickedDouble = false;
		EnergyChange ();
	}
	bool WeaponClicked = false;
	bool WeaponClickedDouble = false;
	public void ClickedWeapon(Button button)
	{
		StartCoroutine (DoubleClickedWeapon ());
	}
	IEnumerator DoubleClickedWeapon ()
	{
		if (WeaponClicked)
		{
			WeaponClickedDouble = true;
			WeaponClicked = false;
			if (Energy < EnergyMax & WeaponEnergy > 0)
			{
				Energy++;
				WeaponEnergy--;
			}
		}
		WeaponClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		WeaponClicked = false;
		if (!WeaponClickedDouble)
		{
			WeaponClickedDouble = false;
			WeaponClicked = false;
			if (Energy > 0 & WeaponEnergy < WeaponEnergyMax)
			{
				Energy--;
				WeaponEnergy++;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		WeaponClickedDouble = false;
		EnergyChange ();
	}

	//UI menu

	public void Continue(Button button)
	{
		MenuUI.SetActive (false);
		Time.timeScale = 1;
		GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Pause";
	}
	int DevClicked = 0;
	public void Settings(Button button)
	{
		StartCoroutine (ActivateDevConsole ());
		if (DevClicked > 2)
			Console.SetActive (true);
	}
	IEnumerator ActivateDevConsole ()
	{
		DevClicked++;
		yield return new WaitForSecondsRealtime ((float)1);
		DevClicked--;
	}
	public void MainMenu(Button button)
	{
		SceneManager.LoadScene ("menu");
	}
	public void Exit(Button button)
	{
		Application.Quit ();
	}
	public void Upgrade(Button button)
	{
		if (!Combat & ShieldEnergyDamaged == 0 & EngineEnergyDamaged == 0 & OxygenEnergyDamaged == 0 & MedbayEnergyDamaged == 0 & WeaponEnergyDamaged == 0 & CockpitEnergyDamaged == 0 & DoorEnergyDamaged == 0)
		{
			UpgradeUI.SetActive (true);
			Time.timeScale = 0;
			GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Paused";
			PowerBarActiveEnergyUpgrade = Energy;
			PowerBarEnergyUpgrade = EnergyMax;
			ScrapUpgrade = Scrap;
			ShieldEnergyUpgrade = ShieldEnergyMax;
			EngineEnergyUpgrade = EngineEnergyMax;
			OxygenEnergyUpgrade = OxygenEnergyMax;
			MedbayEnergyUpgrade = MedbayEnergyMax;
			WeaponEnergyUpgrade = WeaponEnergyMax;
			CockpitEnergyUpgrade = CockpitEnergyMax;
			DoorEnergyUpgrade = DoorEnergyMax;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString (ScrapUpgrade);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").GetComponent<Icon> ().Change (ShieldEnergyMax, ShieldEnergy, ShieldEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").GetComponent<Icon> ().Change (EngineEnergyMax, EngineEnergy, EngineEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").GetComponent<Icon> ().Change (OxygenEnergyMax, OxygenEnergy, OxygenEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").GetComponent<Icon> ().Change (MedbayEnergyMax, MedbayEnergy, MedbayEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").GetComponent<Icon> ().Change (WeaponEnergyMax, WeaponEnergy, WeaponEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").GetComponent<Icon> ().Change (CockpitEnergyMax, CockpitEnergyMax, CockpitEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").GetComponent<Icon> ().Change (DoorEnergyMax, DoorEnergyMax, DoorEnergyDamaged);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (ShieldEnergyUpgrade + 1) + "\n+" + Convert.ToString (ShieldEnergyUpgrade + 2) + "\n+" + Convert.ToString (ShieldEnergyUpgrade + 3) + "\n+" + Convert.ToString (ShieldEnergyUpgrade + 4) + "\n+" + Convert.ToString (ShieldEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [ShieldEnergyUpgrade + 1].Shield + "\n" + costs [ShieldEnergyUpgrade + 2].Shield + "\n" + costs [ShieldEnergyUpgrade + 3].Shield + "\n" + costs [ShieldEnergyUpgrade + 4].Shield + "\n" + costs [ShieldEnergyUpgrade + 5].Shield;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (EngineEnergyUpgrade + 1) + "\n+" + Convert.ToString (EngineEnergyUpgrade + 2) + "\n+" + Convert.ToString (EngineEnergyUpgrade + 3) + "\n+" + Convert.ToString (EngineEnergyUpgrade + 4) + "\n+" + Convert.ToString (EngineEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [EngineEnergyUpgrade + 1].Engine + "\n" + costs [EngineEnergyUpgrade + 2].Engine + "\n" + costs [EngineEnergyUpgrade + 3].Engine + "\n" + costs [EngineEnergyUpgrade + 4].Engine + "\n" + costs [EngineEnergyUpgrade + 5].Engine;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (OxygenEnergyUpgrade + 1) + "\n+" + Convert.ToString (OxygenEnergyUpgrade + 2) + "\n+" + Convert.ToString (OxygenEnergyUpgrade + 3) + "\n+" + Convert.ToString (OxygenEnergyUpgrade + 4) + "\n+" + Convert.ToString (OxygenEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [OxygenEnergyUpgrade + 1].Oxygen + "\n" + costs [OxygenEnergyUpgrade + 2].Oxygen + "\n" + costs [OxygenEnergyUpgrade + 3].Oxygen + "\n" + costs [OxygenEnergyUpgrade + 4].Oxygen + "\n" + costs [OxygenEnergyUpgrade + 5].Oxygen;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (MedbayEnergyUpgrade + 1) + "\n+" + Convert.ToString (MedbayEnergyUpgrade + 2) + "\n+" + Convert.ToString (MedbayEnergyUpgrade + 3) + "\n+" + Convert.ToString (MedbayEnergyUpgrade + 4) + "\n+" + Convert.ToString (MedbayEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [MedbayEnergyUpgrade + 1].Medbay + "\n" + costs [MedbayEnergyUpgrade + 2].Medbay + "\n" + costs [MedbayEnergyUpgrade + 3].Medbay + "\n" + costs [MedbayEnergyUpgrade + 4].Medbay + "\n" + costs [MedbayEnergyUpgrade + 5].Medbay;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (WeaponEnergyUpgrade + 1) + "\n+" + Convert.ToString (WeaponEnergyUpgrade + 2) + "\n+" + Convert.ToString (WeaponEnergyUpgrade + 3) + "\n+" + Convert.ToString (WeaponEnergyUpgrade + 4) + "\n+" + Convert.ToString (WeaponEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [WeaponEnergyUpgrade + 1].Weapon + "\n" + costs [WeaponEnergyUpgrade + 2].Weapon + "\n" + costs [WeaponEnergyUpgrade + 3].Weapon + "\n" + costs [WeaponEnergyUpgrade + 4].Weapon + "\n" + costs [WeaponEnergyUpgrade + 5].Weapon;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (CockpitEnergyUpgrade + 1) + "\n+" + Convert.ToString (CockpitEnergyUpgrade + 2) + "\n+" + Convert.ToString (CockpitEnergyUpgrade + 3) + "\n+" + Convert.ToString (CockpitEnergyUpgrade + 4) + "\n+" + Convert.ToString (CockpitEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [CockpitEnergyUpgrade + 1].Cockpit + "\n" + costs [CockpitEnergyUpgrade + 2].Cockpit + "\n" + costs [CockpitEnergyUpgrade + 3].Cockpit + "\n" + costs [CockpitEnergyUpgrade + 4].Cockpit + "\n" + costs [CockpitEnergyUpgrade + 5].Cockpit;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (DoorEnergyUpgrade + 1) + "\n+" + Convert.ToString (DoorEnergyUpgrade + 2) + "\n+" + Convert.ToString (DoorEnergyUpgrade + 3) + "\n+" + Convert.ToString (DoorEnergyUpgrade + 4) + "\n+" + Convert.ToString (DoorEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs [DoorEnergyUpgrade + 1].Door + "\n" + costs [DoorEnergyUpgrade + 2].Door + "\n" + costs [DoorEnergyUpgrade + 3].Door + "\n" + costs [DoorEnergyUpgrade + 4].Door + "\n" + costs [DoorEnergyUpgrade + 5].Door;
			for (int i = 0; i < 30; i++)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = Nothing;
			for (int i = 0; i < PowerBarEnergyUpgrade; i++)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = PowerBarOff;
			for (int i = 0; i < PowerBarActiveEnergyUpgrade; i++)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString (i + 1)).GetComponent<Image> ().sprite = PowerBarOn;
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString (PowerBarEnergyUpgrade + 1) + "\n+" + Convert.ToString (PowerBarEnergyUpgrade + 2) + "\n+" + Convert.ToString (PowerBarEnergyUpgrade + 3) + "\n+" + Convert.ToString (PowerBarEnergyUpgrade + 4) + "\n+" + Convert.ToString (PowerBarEnergyUpgrade + 5);
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString ((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 3) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 4) / 5 + 1)) * 5);
			if (PowerBarEnergyUpgrade > 25)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString ((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 3) / 5 + 1)) * 5) + "\n-";
			if (PowerBarEnergyUpgrade > 26)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString ((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n-\n-";
			if (PowerBarEnergyUpgrade > 27)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString ((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString ((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n-\n-\n-";
			if (PowerBarEnergyUpgrade > 28)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString ((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n-\n-\n-\n-";
			if (PowerBarEnergyUpgrade > 29)
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n-\n-\n-\n-\n-";
		}
		else
		{
			StartCoroutine (UpgradeWarningActivate ());
		}
	}
	IEnumerator UpgradeWarningActivate()
	{
		ChangeUpgradeWarning (false, "Damaged Systems");
		ChangeUpgradeWarning (false, "In Combat");
		bool sys = false;
		bool com = false;
		if (Combat)
		{
			sys = true;
			ChangeUpgradeWarning (true, "In Combat");
		}
		if (!(ShieldEnergyDamaged == 0 & EngineEnergyDamaged == 0 & OxygenEnergyDamaged == 0 & MedbayEnergyDamaged == 0 & WeaponEnergyDamaged == 0 & CockpitEnergyDamaged == 0 & DoorEnergyDamaged == 0))
		{
			com = true;
			ChangeUpgradeWarning (true, "Damaged Systems");
		}
		yield return new WaitForSecondsRealtime (WarningTime);
		if (sys)
			ChangeUpgradeWarning (false, "In Combat");
		if (com)
			ChangeUpgradeWarning (false, "Damaged Systems");
	}

	//UIupgarde

	int ScrapUpgrade, PowerBarEnergyUpgrade, PowerBarActiveEnergyUpgrade, ShieldEnergyUpgrade, EngineEnergyUpgrade, OxygenEnergyUpgrade, MedbayEnergyUpgrade, WeaponEnergyUpgrade, CockpitEnergyUpgrade, DoorEnergyUpgrade;
	public void UpgradeCancel(Button button)
	{
		UpgradeUI.SetActive (false);
		Time.timeScale = 1;
		GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Pause";
	}
	public void UpgradeBuy(Button button)
	{
		UpgradeUI.SetActive (false);
		Time.timeScale = 1;
		GameObject.Find ("PauseBase").transform.Find ("Pause").transform.Find ("Text").GetComponent<Text> ().text = "Pause";
		Scrap = ScrapUpgrade;
		Energy = PowerBarActiveEnergyUpgrade;
		EnergyMax = PowerBarEnergyUpgrade;
		ShieldEnergyMax = ShieldEnergyUpgrade;
		EngineEnergyMax = EngineEnergyUpgrade;
		OxygenEnergyMax = OxygenEnergyUpgrade;
		MedbayEnergyMax = MedbayEnergyUpgrade;
		WeaponEnergyMax = WeaponEnergyUpgrade;
		CockpitEnergyMax = CockpitEnergyUpgrade;
		DoorEnergyMax = DoorEnergyUpgrade;
		EnergyChange ();
		ScrapChange ();
	}
	bool UpgradePowerBarClicked = false;
	bool UpgradePowerBarClickedDouble = false;
	public void ClickedUpgradePowerBar(Button button)
	{
		StartCoroutine (DoubleClickedUpgradePowerBar ());
	}
	IEnumerator DoubleClickedUpgradePowerBar ()
	{
		if (UpgradePowerBarClicked)
		{
			UpgradePowerBarClickedDouble = true;
			UpgradePowerBarClicked = false;
			if (PowerBarEnergyUpgrade > EnergyMax)
			{
				ScrapUpgrade += ((((PowerBarEnergyUpgrade - 1) / 5 + 1)) * 5);
				PowerBarEnergyUpgrade--;
				PowerBarActiveEnergyUpgrade--;
				for (int i = 0; i < 30; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = Nothing;
				for (int i = 0; i < PowerBarEnergyUpgrade; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOff;
				for (int i = 0; i < PowerBarActiveEnergyUpgrade; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOn;
			}
		}
		UpgradePowerBarClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradePowerBarClicked = false;
		if (!UpgradePowerBarClickedDouble)
		{
			UpgradePowerBarClickedDouble = false;
			UpgradePowerBarClicked = false;
			if (ScrapUpgrade > ((((PowerBarEnergyUpgrade - 1) / 5 + 1)) * 5) & PowerBarEnergyUpgrade < 30)
			{
				PowerBarEnergyUpgrade++;
				PowerBarActiveEnergyUpgrade++;
				ScrapUpgrade -= ((((PowerBarEnergyUpgrade - 1) / 5 + 1)) * 5);
				for (int i = 0; i < 30; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = Nothing;
				for (int i = 0; i < PowerBarEnergyUpgrade; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOff;
				for (int i = 0; i < PowerBarActiveEnergyUpgrade; i++)
					GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOn;
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradePowerBarClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(PowerBarEnergyUpgrade + 1) + "\n+" + Convert.ToString(PowerBarEnergyUpgrade + 2) + "\n+" + Convert.ToString(PowerBarEnergyUpgrade + 3) + "\n+" + Convert.ToString(PowerBarEnergyUpgrade + 4) + "\n+" + Convert.ToString(PowerBarEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 3) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 4) / 5 + 1)) * 5);
		if (PowerBarEnergyUpgrade > 25)
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 3) / 5 + 1)) * 5) + "\n-";
		if (PowerBarEnergyUpgrade > 26)
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 2) / 5 + 1)) * 5) + "\n-\n-";
		if (PowerBarEnergyUpgrade > 27)
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n" + Convert.ToString((((PowerBarEnergyUpgrade + 1) / 5 + 1)) * 5) + "\n-\n-\n-";
		if (PowerBarEnergyUpgrade > 28)
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + Convert.ToString((((PowerBarEnergyUpgrade) / 5 + 1)) * 5) + "\n-\n-\n-\n-";
		if (PowerBarEnergyUpgrade > 29)
			GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("PowerBarUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n-\n-\n-\n-\n-";
	}
	bool UpgradeShieldClicked = false;
	bool UpgradeShieldClickedDouble = false;
	public void ClickedUpgradeShield(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeShield ());
	}
	IEnumerator DoubleClickedUpgradeShield ()
	{
		if (UpgradeShieldClicked)
		{
			UpgradeShieldClickedDouble = true;
			UpgradeShieldClicked = false;
			if (ShieldEnergyUpgrade > ShieldEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [ShieldEnergyUpgrade].Shield);
				ShieldEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").GetComponent<Icon> ().Change (ShieldEnergyUpgrade, ShieldEnergy, ShieldEnergyDamaged);
			}
		}
		UpgradeShieldClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeShieldClicked = false;
		if (!UpgradeShieldClickedDouble)
		{
			UpgradeShieldClickedDouble = false;
			UpgradeShieldClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[ShieldEnergyUpgrade + 1].Shield) & ShieldEnergyUpgrade < 8)
			{
				ShieldEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [ShieldEnergyUpgrade].Shield);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").GetComponent<Icon> ().Change (ShieldEnergyUpgrade, ShieldEnergy, ShieldEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeShieldClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(ShieldEnergyUpgrade + 1) + "\n+" + Convert.ToString(ShieldEnergyUpgrade + 2) + "\n+" + Convert.ToString(ShieldEnergyUpgrade + 3) + "\n+" + Convert.ToString(ShieldEnergyUpgrade + 4) + "\n+" + Convert.ToString(ShieldEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconShieldUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[ShieldEnergyUpgrade + 1].Shield + "\n" + costs[ShieldEnergyUpgrade + 2].Shield + "\n" + costs[ShieldEnergyUpgrade + 3].Shield + "\n" + costs[ShieldEnergyUpgrade + 4].Shield + "\n" + costs[ShieldEnergyUpgrade + 5].Shield;
	}
	bool UpgradeEngineClicked = false;
	bool UpgradeEngineClickedDouble = false;
	public void ClickedUpgradeEngine(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeEngine ());
	}
	IEnumerator DoubleClickedUpgradeEngine ()
	{
		if (UpgradeEngineClicked)
		{
			UpgradeEngineClickedDouble = true;
			UpgradeEngineClicked = false;
			if (EngineEnergyUpgrade > EngineEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [EngineEnergyUpgrade].Engine);
				EngineEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").GetComponent<Icon> ().Change (EngineEnergyUpgrade, EngineEnergy, EngineEnergyDamaged);
			}
		}
		UpgradeEngineClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeEngineClicked = false;
		if (!UpgradeEngineClickedDouble)
		{
			UpgradeEngineClickedDouble = false;
			UpgradeEngineClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[EngineEnergyUpgrade + 1].Engine) & EngineEnergyUpgrade < 8)
			{
				EngineEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [EngineEnergyUpgrade].Engine);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").GetComponent<Icon> ().Change (EngineEnergyUpgrade, EngineEnergy, EngineEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeEngineClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(EngineEnergyUpgrade + 1) + "\n+" + Convert.ToString(EngineEnergyUpgrade + 2) + "\n+" + Convert.ToString(EngineEnergyUpgrade + 3) + "\n+" + Convert.ToString(EngineEnergyUpgrade + 4) + "\n+" + Convert.ToString(EngineEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconEngineUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[EngineEnergyUpgrade + 1].Engine + "\n" + costs[EngineEnergyUpgrade + 2].Engine + "\n" + costs[EngineEnergyUpgrade + 3].Engine + "\n" + costs[EngineEnergyUpgrade + 4].Engine + "\n" + costs[EngineEnergyUpgrade + 5].Engine;
	}
	bool UpgradeOxygenClicked = false;
	bool UpgradeOxygenClickedDouble = false;
	public void ClickedUpgradeOxygen(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeOxygen ());
	}
	IEnumerator DoubleClickedUpgradeOxygen ()
	{
		if (UpgradeOxygenClicked)
		{
			UpgradeOxygenClickedDouble = true;
			UpgradeOxygenClicked = false;
			if (OxygenEnergyUpgrade > OxygenEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [OxygenEnergyUpgrade].Oxygen);
				OxygenEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").GetComponent<Icon> ().Change (OxygenEnergyUpgrade, OxygenEnergy, OxygenEnergyDamaged);
			}
		}
		UpgradeOxygenClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeOxygenClicked = false;
		if (!UpgradeOxygenClickedDouble)
		{
			UpgradeOxygenClickedDouble = false;
			UpgradeOxygenClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[OxygenEnergyUpgrade + 1].Oxygen) & OxygenEnergyUpgrade < 8)
			{
				OxygenEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [OxygenEnergyUpgrade].Oxygen);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").GetComponent<Icon> ().Change (OxygenEnergyUpgrade, OxygenEnergy, OxygenEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeOxygenClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(OxygenEnergyUpgrade + 1) + "\n+" + Convert.ToString(OxygenEnergyUpgrade + 2) + "\n+" + Convert.ToString(OxygenEnergyUpgrade + 3) + "\n+" + Convert.ToString(OxygenEnergyUpgrade + 4) + "\n+" + Convert.ToString(OxygenEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconOxygenUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[OxygenEnergyUpgrade + 1].Oxygen + "\n" + costs[OxygenEnergyUpgrade + 2].Oxygen + "\n" + costs[OxygenEnergyUpgrade + 3].Oxygen + "\n" + costs[OxygenEnergyUpgrade + 4].Oxygen + "\n" + costs[OxygenEnergyUpgrade + 5].Oxygen;
	}
	bool UpgradeMedbayClicked = false;
	bool UpgradeMedbayClickedDouble = false;
	public void ClickedUpgradeMedbay(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeMedbay ());
	}
	IEnumerator DoubleClickedUpgradeMedbay ()
	{
		if (UpgradeMedbayClicked)
		{
			UpgradeMedbayClickedDouble = true;
			UpgradeMedbayClicked = false;
			if (MedbayEnergyUpgrade > MedbayEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [MedbayEnergyUpgrade].Medbay);
				MedbayEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").GetComponent<Icon> ().Change (MedbayEnergyUpgrade, MedbayEnergy, MedbayEnergyDamaged);
			}
		}
		UpgradeMedbayClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeMedbayClicked = false;
		if (!UpgradeMedbayClickedDouble)
		{
			UpgradeMedbayClickedDouble = false;
			UpgradeMedbayClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[MedbayEnergyUpgrade + 1].Medbay) & MedbayEnergyUpgrade < 8)
			{
				MedbayEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [MedbayEnergyUpgrade].Medbay);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").GetComponent<Icon> ().Change (MedbayEnergyUpgrade, MedbayEnergy, MedbayEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeMedbayClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(MedbayEnergyUpgrade + 1) + "\n+" + Convert.ToString(MedbayEnergyUpgrade + 2) + "\n+" + Convert.ToString(MedbayEnergyUpgrade + 3) + "\n+" + Convert.ToString(MedbayEnergyUpgrade + 4) + "\n+" + Convert.ToString(MedbayEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconMedbayUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[MedbayEnergyUpgrade + 1].Medbay + "\n" + costs[MedbayEnergyUpgrade + 2].Medbay + "\n" + costs[MedbayEnergyUpgrade + 3].Medbay + "\n" + costs[MedbayEnergyUpgrade + 4].Medbay + "\n" + costs[MedbayEnergyUpgrade + 5].Medbay;
	}
	bool UpgradeWeaponClicked = false;
	bool UpgradeWeaponClickedDouble = false;
	public void ClickedUpgradeWeapon(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeWeapon ());
	}
	IEnumerator DoubleClickedUpgradeWeapon ()
	{
		if (UpgradeWeaponClicked)
		{
			UpgradeWeaponClickedDouble = true;
			UpgradeWeaponClicked = false;
			if (WeaponEnergyUpgrade > WeaponEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [WeaponEnergyUpgrade].Weapon);
				WeaponEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").GetComponent<Icon> ().Change (WeaponEnergyUpgrade, WeaponEnergy, WeaponEnergyDamaged);
			}
		}
		UpgradeWeaponClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeWeaponClicked = false;
		if (!UpgradeWeaponClickedDouble)
		{
			UpgradeWeaponClickedDouble = false;
			UpgradeWeaponClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[WeaponEnergyUpgrade + 1].Weapon) & WeaponEnergyUpgrade < 8)
			{
				WeaponEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [WeaponEnergyUpgrade].Weapon);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").GetComponent<Icon> ().Change (WeaponEnergyUpgrade, WeaponEnergy, WeaponEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeWeaponClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(WeaponEnergyUpgrade + 1) + "\n+" + Convert.ToString(WeaponEnergyUpgrade + 2) + "\n+" + Convert.ToString(WeaponEnergyUpgrade + 3) + "\n+" + Convert.ToString(WeaponEnergyUpgrade + 4) + "\n+" + Convert.ToString(WeaponEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconWeaponUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[WeaponEnergyUpgrade + 1].Weapon + "\n" + costs[WeaponEnergyUpgrade + 2].Weapon + "\n" + costs[WeaponEnergyUpgrade + 3].Weapon + "\n" + costs[WeaponEnergyUpgrade + 4].Weapon + "\n" + costs[WeaponEnergyUpgrade + 5].Weapon;
	}
	bool UpgradeCockpitClicked = false;
	bool UpgradeCockpitClickedDouble = false;
	public void ClickedUpgradeCockpit(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeCockpit ());
	}
	IEnumerator DoubleClickedUpgradeCockpit ()
	{
		if (UpgradeCockpitClicked)
		{
			UpgradeCockpitClickedDouble = true;
			UpgradeCockpitClicked = false;
			if (CockpitEnergyUpgrade > CockpitEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [CockpitEnergyUpgrade].Cockpit);
				CockpitEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").GetComponent<Icon> ().Change (CockpitEnergyUpgrade, CockpitEnergyUpgrade, CockpitEnergyDamaged);
			}
		}
		UpgradeCockpitClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeCockpitClicked = false;
		if (!UpgradeCockpitClickedDouble)
		{
			UpgradeCockpitClickedDouble = false;
			UpgradeCockpitClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[CockpitEnergyUpgrade + 1].Cockpit) & CockpitEnergyUpgrade < 8)
			{
				CockpitEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [CockpitEnergyUpgrade].Cockpit);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").GetComponent<Icon> ().Change (CockpitEnergyUpgrade, CockpitEnergyUpgrade, CockpitEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeCockpitClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(CockpitEnergyUpgrade + 1) + "\n+" + Convert.ToString(CockpitEnergyUpgrade + 2) + "\n+" + Convert.ToString(CockpitEnergyUpgrade + 3) + "\n+" + Convert.ToString(CockpitEnergyUpgrade + 4) + "\n+" + Convert.ToString(CockpitEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconCockpitUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[CockpitEnergyUpgrade + 1].Cockpit + "\n" + costs[CockpitEnergyUpgrade + 2].Cockpit + "\n" + costs[CockpitEnergyUpgrade + 3].Cockpit + "\n" + costs[CockpitEnergyUpgrade + 4].Cockpit + "\n" + costs[CockpitEnergyUpgrade + 5].Cockpit;
	}
	bool UpgradeDoorClicked = false;
	bool UpgradeDoorClickedDouble = false;
	public void ClickedUpgradeDoor(Button button)
	{
		StartCoroutine (DoubleClickedUpgradeDoor ());
	}
	IEnumerator DoubleClickedUpgradeDoor ()
	{
		if (UpgradeDoorClicked)
		{
			UpgradeDoorClickedDouble = true;
			UpgradeDoorClicked = false;
			if (DoorEnergyUpgrade > DoorEnergyMax)
			{
				ScrapUpgrade += Convert.ToInt32(costs [DoorEnergyUpgrade].Door);
				DoorEnergyUpgrade--;
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").GetComponent<Icon> ().Change (DoorEnergyUpgrade, DoorEnergyUpgrade, DoorEnergyDamaged);
			}
		}
		UpgradeDoorClicked = true;
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeDoorClicked = false;
		if (!UpgradeDoorClickedDouble)
		{
			UpgradeDoorClickedDouble = false;
			UpgradeDoorClicked = false;
			if (ScrapUpgrade > Convert.ToInt32(costs[DoorEnergyUpgrade + 1].Door) & DoorEnergyUpgrade < 8)
			{
				DoorEnergyUpgrade++;
				ScrapUpgrade -= Convert.ToInt32(costs [DoorEnergyUpgrade].Door);
				GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").GetComponent<Icon> ().Change (DoorEnergyUpgrade, DoorEnergyUpgrade, DoorEnergyDamaged);
			}
		}
		yield return new WaitForSecondsRealtime (DoubleClickDelay);
		UpgradeDoorClickedDouble = false;
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("Scrap").GetComponent<Text> ().text = "Scrap\n" + Convert.ToString(ScrapUpgrade);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").transform.Find ("Level").GetComponent<Text> ().text = "Upgrade\n+" + Convert.ToString(DoorEnergyUpgrade + 1) + "\n+" + Convert.ToString(DoorEnergyUpgrade + 2) + "\n+" + Convert.ToString(DoorEnergyUpgrade + 3) + "\n+" + Convert.ToString(DoorEnergyUpgrade + 4) + "\n+" + Convert.ToString(DoorEnergyUpgrade + 5);
		GameObject.Find ("UI").transform.Find ("Upgrade").transform.Find ("IconDoorUpgrade").transform.Find ("Cost").GetComponent<Text> ().text = "Cost\n" + costs[DoorEnergyUpgrade + 1].Door + "\n" + costs[DoorEnergyUpgrade + 2].Door + "\n" + costs[DoorEnergyUpgrade + 3].Door + "\n" + costs[DoorEnergyUpgrade + 4].Door + "\n" + costs[DoorEnergyUpgrade + 5].Door;
	}

	//update UI

	void HullHpChange()
	{
		GameObject.Find ("HullHp").GetComponent<RectTransform> ().anchoredPosition = new Vector2(0f, 4.27667f * HullHp -128.3f);
		GameObject.Find ("HullHpText").GetComponent<Text> ().text = Convert.ToString(HullHp);
	}
	void ScrapChange()
	{
		GameObject.Find ("Text_Scrap").GetComponent<Text> ().text = "Scrap: " + Scrap;
	}
	void FuelChange()
	{
		GameObject.Find ("Text_Fuel").GetComponent<Text> ().text = "Fuel: " + Fuel;
	}
	void AmmoChange()
	{
		GameObject.Find ("Text_Ammo").GetComponent<Text> ().text = "Ammo: " + Ammo;
	}
	void DronePartChange()
	{
		GameObject.Find ("Text_DronePart").GetComponent<Text> ().text = "Drone: " + DronePart;
	}
	void EvadeChange()
	{
		GameObject.Find ("Text_Evade").GetComponent<Text> ().text = "Evade: " + Evade + "%";
	}
	void OxygenChange()
	{
		GameObject.Find ("Text_Oxygen").GetComponent<Text> ().text = "Oxygen: " + Oxygen + "%";
	}
	void ShieldChange()
	{
		for (int i = 0; i < 4; i++)
			GameObject.Find ("Shield" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = Nothing;
		for (int i = 0; i < ShieldMax; i++)
			GameObject.Find ("Shield" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = ShieldOff;
		for (int i = 0; i < Shield; i++)
			GameObject.Find ("Shield" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = ShieldOn;
		GameObject.Find ("Ship").transform.Find ("shield").GetComponent<SpriteRenderer> ().color = new Color32 ((byte)255, (byte)255, (byte)255, (byte)(255 - 50 * (4 - Shield)));
		if (Shield == 0)
			GameObject.Find ("Ship").transform.Find ("shield").GetComponent<SpriteRenderer> ().color = new Color32 ((byte)255, (byte)255, (byte)255, (byte)0);
	}
	void EnergyChange()
	{
		for (int i = 0; i < 30; i++)
			GameObject.Find ("UI").transform.Find ("PowerBar").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = Nothing;
		for (int i = 0; i < EnergyMax; i++)
			GameObject.Find ("UI").transform.Find ("PowerBar").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOff;
		for (int i = 0; i < Energy; i++)
			GameObject.Find ("UI").transform.Find ("PowerBar").transform.Find ("PowerBar" + Convert.ToString(i + 1)).GetComponent<Image>().sprite = PowerBarOn;
		ShieldEnergyChange ();
		EngineEnergyChange ();
		OxygenEnergyChange ();
		MedbayEnergyChange ();
		WeaponEnergyChange ();
		CockpitEnergyChange ();
		DoorEnergyChange ();
	}
	void ShieldEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconShield").GetComponent<Icon> ().Change (ShieldEnergyMax, ShieldEnergy, ShieldEnergyDamaged);
		ShieldMax = ShieldEnergyMax / 2;
		if (Shield > ShieldEnergy / 2)
			Shield = ShieldEnergy / 2;
		ShieldChange ();
	}
	void EngineEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconEngine").GetComponent<Icon> ().Change (EngineEnergyMax, EngineEnergy, EngineEnergyDamaged);
		Evade = 10 * (EngineEnergy * 0.25f + 0.75f);
		if (EngineEnergy == 0)
			Evade = 0;
		EvadeChange ();
	}
	void OxygenEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconOxygen").GetComponent<Icon> ().Change (OxygenEnergyMax, OxygenEnergy, OxygenEnergyDamaged);
	}
	void MedbayEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconMedbay").GetComponent<Icon> ().Change (MedbayEnergyMax, MedbayEnergy, MedbayEnergyDamaged);
	}
	void WeaponEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconWeapon").GetComponent<Icon> ().Change (WeaponEnergyMax, WeaponEnergy, WeaponEnergyDamaged);
	}
	void CockpitEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconCockpit").GetComponent<Icon> ().Change (CockpitEnergyMax, CockpitEnergyMax, CockpitEnergyDamaged);
	}
	void DoorEnergyChange()
	{
		GameObject.Find ("UI").transform.Find ("IconDoor").GetComponent<Icon> ().Change (DoorEnergyMax, DoorEnergyMax, DoorEnergyDamaged);
	}
	string[] UpgradeWarning = new string[10];
	int UpgradeWarningCount = 0;
	void ChangeUpgradeWarning (bool Add, string text)
	{
		if (Add)
		{
			bool temp = false;
			for (int i = 0; i < 10; i++)
			{
				if (text == UpgradeWarning [i])
					temp = true;
			}
			if (!temp)
			{
				UpgradeWarning [UpgradeWarningCount] = text;
				UpgradeWarningCount++;
			}
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				if (text == UpgradeWarning [i])
				{
					for (int j = i; j < 9; j++)
						UpgradeWarning [j] = UpgradeWarning [j + 1];
					UpgradeWarning [9] = null;
					UpgradeWarningCount--;
				}
			}
		}
		GameObject.Find ("UI").transform.Find ("Text_UpgradeWarning").GetComponent<Text> ().text = null;
		for (int i = 0; i < UpgradeWarningCount; i++)
		{
			GameObject.Find ("UI").transform.Find ("Text_UpgradeWarning").GetComponent<Text> ().text += UpgradeWarning [i] + "\n";
		}
	}
	string[] WarpWarning = new string[10];
	int WarpWarningCount = 0;
	void ChangeWarpWarning (bool Add, string text)
	{
		if (Add)
		{
			bool temp = false;
			for (int i = 0; i < 10; i++)
			{
				if (text == WarpWarning [i])
					temp = true;
			}
			if (!temp)
			{
				WarpWarning [WarpWarningCount] = text;
				WarpWarningCount++;
			}
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				if (text == WarpWarning [i])
				{
					for (int j = i; j < 9; j++)
						WarpWarning [j] = WarpWarning [j + 1];
					WarpWarning [9] = null;
					WarpWarningCount--;
				}
			}
		}
		GameObject.Find ("UI").transform.Find ("Text_WarpWarning").GetComponent<Text> ().text = null;
		for (int i = 0; i < WarpWarningCount; i++)
		{
			GameObject.Find ("UI").transform.Find ("Text_WarpWarning").GetComponent<Text> ().text += WarpWarning [i] + "\n";
		}
	}

	//Comms (between scripts)

	public void _SetRoomRole(int x, int y, string role)
	{
		ship [x, y].role = role;
	}
	public int _GetMedbayEnergy()
	{
		return(MedbayEnergy);
	}
	public int _GetShield()
	{
		return(Shield);
	}
	public float _GetEvade()
	{
		return(Evade);
	}
	public void _Damage(Vector2 Destination, int SystemDamage, int HullDamage, int CrewDamage, bool Breach, bool Fire)
	{
		HullHp -= HullDamage;
		HullHpChange ();
		switch (ship[(int)Destination.x,(int)Destination.y].role)
		{
		case "Shield":
			ShieldRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (ShieldEnergyMax > 0)
				{
					if (ShieldEnergy == ShieldEnergyMax)
					{
						ShieldEnergy--;
						ShieldEnergyMax--;
						Energy++;
					}
					else
					{
						ShieldEnergyMax--;
					}
					ShieldEnergyDamaged++;
					ShieldEnergyChange ();
				}
			}
			break;
		case "Engine":
			EngineRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (EngineEnergyMax > 0)
				{
					if (EngineEnergy == EngineEnergyMax)
					{
						EngineEnergy--;
						EngineEnergyMax--;
						Energy++;
					}
					else
					{
						EngineEnergyMax--;
					}
					EngineEnergyDamaged++;
					EngineEnergyChange ();
				}
			}
			break;
		case "Oxygen":
			OxygenRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (OxygenEnergyMax > 0)
				{
					if (OxygenEnergy == OxygenEnergyMax)
					{
						OxygenEnergy--;
						OxygenEnergyMax--;
						Energy++;
					}
					else
					{
						OxygenEnergyMax--;
					}
					OxygenEnergyDamaged++;
					OxygenEnergyChange ();
				}
			}
			break;
		case "Medbay":
			MedbayRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (MedbayEnergyMax > 0)
				{
					if (MedbayEnergy == MedbayEnergyMax)
					{
						MedbayEnergy--;
						MedbayEnergyMax--;
						Energy++;
					}
					else
					{
						MedbayEnergyMax--;
					}
					MedbayEnergyDamaged++;
					MedbayEnergyChange ();
				}
			}
			break;
		case "Weapon":
			WeaponRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (WeaponEnergyMax > 0)
				{
					if (WeaponEnergy == WeaponEnergyMax)
					{
						WeaponEnergy--;
						WeaponEnergyMax--;
						Energy++;
					}
					else
					{
						WeaponEnergyMax--;
					}
					WeaponEnergyDamaged++;
					WeaponEnergyChange ();
				}
			}
			break;
		case "Cockpit":
			CockpitRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (CockpitEnergyMax > 0)
				{
					CockpitEnergyMax--;
					CockpitEnergyDamaged++;
					CockpitEnergyChange ();
				}
			}
			break;
		case "Door":
			DoorRepairPercent = 0;
			for (int i = 0; i < SystemDamage; i++)
			{
				if (DoorEnergyMax > 0)
				{
					DoorEnergyMax--;
					DoorEnergyDamaged++;
					DoorEnergyChange ();
				}
			}
			break;
		}
	}
	public void _ShieldDamage(int ShieldDamage)
	{
		ShieldPercent = 0;
		Shield -= ShieldDamage;
		ShieldChange ();
	}

	//Developer Console

	public void DevConsole(InputField ConsoleInput)
	{
		string command = "";
		string value = "";
		Text placeholder = (Text)GameObject.Find ("DevConsole").transform.Find ("Placeholder").GetComponent (typeof(Text));
		string Input = ConsoleInput.text.ToString ();
        if (Input.Length != 0)
        {
            if (Input[Input.Length - 1] == ')' & Input.Contains("("))
            {
                command = Input.Substring(0, Input.IndexOf('('));
                value = Input.Substring(Input.IndexOf('(') + 1, Input.Length - Input.IndexOf('(') - 2);
                string[] values = value.Split(',');
                switch (command)
                {
                    case "Combat":
                        switch (value)
                        {
                            case "true":
                                placeholder.text = ("Combat UI Layer has been set to true");
                                Combat = true;
                                CombatChange();
                                break;
                            case "false":
                                placeholder.text = ("Combat UI Layer has been set to false");
                                Combat = false;
                                CombatChange();
                                break;
                            default:
                                placeholder.text = ("Combat UI Layer can only be false or true");
                                break;
                        }
                        break;
                    case "HullHp":
                        HullHp = Convert.ToInt32(value);
                        HullHpChange();
                        placeholder.text = ("HullHp has been set to " + value);
                        break;
                    case "Scrap":
                        Scrap = Convert.ToInt32(value);
                        ScrapChange();
                        placeholder.text = ("Scrap has been set to " + value);
                        break;
                    case "Fuel":
                        Fuel = Convert.ToInt32(value);
                        FuelChange();
                        placeholder.text = ("Fuel has been set to " + value);
                        break;
                    case "Ammo":
                        Ammo = Convert.ToInt32(value);
                        AmmoChange();
                        placeholder.text = ("Ammo has been set to " + value);
                        break;
                    case "DronePart":
                        DronePart = Convert.ToInt32(value);
                        DronePartChange();
                        placeholder.text = ("DronePart has been set to " + value);
                        break;
                    case "Evade":
                        Evade = Convert.ToInt32(value);
                        EvadeChange();
                        placeholder.text = ("Evade has been set to " + value);
                        break;
                    case "Oxygen":
                        string temp1, temp2;
                        int SummOxygen = 0;
                        for (int i = 2; i <= w - 1; i++)
                        {
                            for (int j = 2; j <= h - 1; j++)
                            {
                                if (ship[i, j].room)
                                {
                                    ship[i, j].oxigen = Convert.ToInt32(value);
                                    if (i >= 10)
                                    {
                                        temp1 = "room";
                                    }
                                    else
                                    {
                                        temp1 = "room0";
                                    }
                                    if (j >= 10)
                                    {
                                        temp2 = "_";
                                    }
                                    else
                                    {
                                        temp2 = "_0";
                                    }
                                    gameObject.transform.Find(temp1 + i.ToString() + temp2 + j.ToString()).GetComponent<SpriteRenderer>().color = new Color32((byte)255, (byte)(155 + ship[i, j].oxigen), (byte)(155 + ship[i, j].oxigen), 255);
                                    SummOxygen += Convert.ToInt32(ship[i, j].oxigen);
                                }
                            }
                        }
                        Oxygen = Convert.ToInt32(SummOxygen / rooms);
                        OxygenChange();
                        placeholder.text = ("Oxygen has been set to " + value);
                        break;
                    case "ShieldMax":
                        ShieldMax = Convert.ToInt32(value);
                        ShieldChange();
                        placeholder.text = ("ShieldMax has been set to " + value);
                        break;
                    case "Shield":
                        Shield = Convert.ToInt32(value);
                        ShieldChange();
                        placeholder.text = ("Shield has been set to " + value);
                        break;
                    case "Energy":
                        Energy = Convert.ToInt32(value);
                        EnergyChange();
                        placeholder.text = ("Energy has been set to " + value);
                        break;
                    case "ShieldEnergy":
                        ShieldEnergy = Convert.ToInt32(value);
                        ShieldEnergyChange();
                        placeholder.text = ("ShieldEnergy has been set to " + value);
                        break;
                    case "EngineEnergy":
                        EngineEnergy = Convert.ToInt32(value);
                        EngineEnergyChange();
                        placeholder.text = ("EngineEnergy has been set to " + value);
                        break;
                    case "OxygenEnergy":
                        OxygenEnergy = Convert.ToInt32(value);
                        OxygenEnergyChange();
                        placeholder.text = ("OxygenEnergy has been set to " + value);
                        break;
                    case "MedbayEnergy":
                        MedbayEnergy = Convert.ToInt32(value);
                        MedbayEnergyChange();
                        placeholder.text = ("MedbayEnergy has been set to " + value);
                        break;
                    case "WeaponEnergy":
                        WeaponEnergy = Convert.ToInt32(value);
                        WeaponEnergyChange();
                        placeholder.text = ("WeaponEnergy has been set to " + value);
                        break;
                    case "EnergyMax":
                        EnergyMax = Convert.ToInt32(value);
                        EnergyChange();
                        placeholder.text = ("EnergyMax has been set to " + value);
                        break;
                    case "ShieldEnergyMax":
                        ShieldEnergyMax = Convert.ToInt32(value);
                        ShieldEnergyChange();
                        placeholder.text = ("ShieldEnergyMax has been set to " + value);
                        break;
                    case "EngineEnergyMax":
                        EngineEnergyMax = Convert.ToInt32(value);
                        EngineEnergyChange();
                        placeholder.text = ("EngineEnergyMax has been set to " + value);
                        break;
                    case "OxygenEnergyMax":
                        OxygenEnergyMax = Convert.ToInt32(value);
                        OxygenEnergyChange();
                        placeholder.text = ("OxygenEnergyMax has been set to " + value);
                        break;
                    case "MedbayEnergyMax":
                        MedbayEnergyMax = Convert.ToInt32(value);
                        MedbayEnergyChange();
                        placeholder.text = ("MedbayEnergyMax has been set to " + value);
                        break;
                    case "WeaponEnergyMax":
                        WeaponEnergyMax = Convert.ToInt32(value);
                        WeaponEnergyChange();
                        placeholder.text = ("WeaponEnergyMax has been set to " + value);
                        break;
                    case "SpawnProjectileCustom":
                        GameObject newObject = Instantiate(EnemyProjectile) as GameObject;
                        newObject.transform.SetParent(GameObject.Find("EnemyProjectiles").transform);
                        newObject.GetComponent<EnemyProjectile>().Initialize(new Vector2(Convert.ToInt32(values[0]), Convert.ToInt32(values[1])), Convert.ToInt32(values[2]), Convert.ToInt32(values[3]), Convert.ToInt32(values[4]), Convert.ToInt32(values[5]), Convert.ToInt32(values[6]), Convert.ToInt32(values[7]), Convert.ToInt32(values[8]), Convert.ToInt32(values[9]));
                        placeholder.text = ("Custom projectile has been spawned");
                        break;
                    case "SpawnProjectiles":
                        if (Convert.ToDouble(values[1]) == 0)
                            for (int i = 0; i < Convert.ToInt32(values[0]); i++)
                            {
                                newObject = Instantiate(EnemyProjectile) as GameObject;
                                newObject.transform.SetParent(GameObject.Find("EnemyProjectiles").transform);
                            }
                        else
                            StartCoroutine(ConsoleSpawnProjectiles(Convert.ToInt32(values[0]), Convert.ToDouble(values[1]), false));
                        placeholder.text = (values[0] + " projectiles has been spawned");
                        break;
                    case "SpawnProjectilesRandom":
                        if (Convert.ToDouble(values[1]) == 0)
                            for (int i = 0; i < Convert.ToInt32(values[0]); i++)
                            {
                                newObject = Instantiate(EnemyProjectile) as GameObject;
                                newObject.transform.SetParent(GameObject.Find("EnemyProjectiles").transform);
                                int x = 0;
                                int y = 0;
                                while (!ship[x, y].room)
                                {
                                    x = UnityEngine.Random.Range(1, ShipWidth);
                                    y = UnityEngine.Random.Range(1, ShipHeight);
                                }
                                newObject.GetComponent<EnemyProjectile>().Initialize(new Vector2(x, y), 0, 1, 1, 10, 1, 10, 10, 10);
                            }
                        else
                            StartCoroutine(ConsoleSpawnProjectiles(Convert.ToInt32(values[0]), Convert.ToDouble(values[1]), true));
                        placeholder.text = (values[0] + " projectiles has been spawned");
                        break;
                    case "DisableConsole":
                        Console.SetActive(false);
                        break;
                    default:
                        placeholder.text = ("Unknown command");
                        break;
                }
            }
        }
        else
        {
            placeholder.text = "Unknown command";
        }
		InputField console = (InputField)GameObject.Find ("DevConsole").GetComponent (typeof(InputField));
		console.text = "";
	}
	IEnumerator ConsoleSpawnProjectiles(int value, Double delay, bool random)
	{
		GameObject newObject;
		for (int i = 0; i < value; i++)
		{
			newObject = Instantiate (EnemyProjectile) as GameObject;
			newObject.transform.SetParent (GameObject.Find ("EnemyProjectiles").transform);
			int x = 0;
			int y = 0;
			if (random)
			{
				while (!ship [x, y].room) {
					x = UnityEngine.Random.Range (1, ShipWidth);
					y = UnityEngine.Random.Range (1, ShipHeight);
				}
				newObject.GetComponent<EnemyProjectile>().Initialize(new Vector2(x, y),0,1,1,10,1,10,10,10);
			}
			yield return new WaitForSeconds ((float)delay);
		}
	}

	//audio

	IEnumerator MusicFadeOut ()
	{
		while (GameObject.Find("Camera").GetComponent<AudioSource>().volume > 0)
		{
			GameObject.Find("Camera").GetComponent<AudioSource>().volume -= MusicVolume * Time.deltaTime / WarpTime;
			yield return null;
		}
	}
	IEnumerator MusicFadeIn ()
	{
		GameObject.Find ("Camera").GetComponent<AudioSource> ().Play ();
		while (GameObject.Find("Camera").GetComponent<AudioSource>().volume < MusicVolume)
		{
			GameObject.Find("Camera").GetComponent<AudioSource>().volume += MusicVolume * Time.deltaTime / WarpTime;
			yield return null;
		}
		GameObject.Find ("Camera").GetComponent<AudioSource> ().volume = MusicVolume;
	}
}