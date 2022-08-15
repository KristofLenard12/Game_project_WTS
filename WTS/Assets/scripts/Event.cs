using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

    EventType[] allEvents = new EventType[0]; //eventek számát beírni

	// Use this for initialization
	void Start ()
    {
        //eventeket beírni
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public int randomcrew()
    {
        System.Random rnd = new System.Random();
        int crewtype = rnd.Next(0, 4);
        return crewtype;
    }
    public int randomWeapon()
    {
        System.Random rnd = new System.Random();
        int weaptype = rnd.Next(0, 2);
        int weapon = 0;
        if (weaptype == 0) //missile
        {
            weapon = rnd.Next(); //beírni a fegyverszámtól függően
        }
        else //laser
        {
            weapon = rnd.Next(); //beírni a fegyverszámtól függően
        }
        return weapon;
    }
    public int randomAug()
    {
        System.Random rnd = new System.Random();
        int augtype = rnd.Next(); //beírni az augmentek számát
        return augtype;
    }
    public EventType EventGen()
    {
        EventType @event = new EventType();
        System.Random rnd = new System.Random();
        int id = rnd.Next(); //eventek száma
        foreach (EventType item in allEvents)
        {
            if (id == item.eventId)
            {
                @event.eventDesc = item.eventDesc;
                @event.choice1 = item.choice1;
                @event.reward1 = item.reward1;
                @event.choice2 = item.choice2;
                @event.reward2 = item.reward2;
                @event.third = item.third;
                @event.choice3 = item.choice3;
                @event.reward3 = item.reward3;
                @event.fourth = item.fourth;
                @event.choice4 = item.choice4;
                @event.reward4 = item.reward4;
            }
        }
        return @event;
    }
}

public class EventType
{
    public int eventId; //egyedi azonosító
    public string eventDesc; //event leírása

    public string choice1; //választások
    public int[] reward1 = new int[6]; //scrap, fuel, missile, crewmember, weapon, augment

    public string choice2;
    public int[] reward2 = new int[6];

    public bool third; //Van-e harmadik választás
    public string choice3;
    public int[] reward3 = new int[6];

    public bool fourth;
    public string choice4;
    public int[] reward4 = new int[6];
}
