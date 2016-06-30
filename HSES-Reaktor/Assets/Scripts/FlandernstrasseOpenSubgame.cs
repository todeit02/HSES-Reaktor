using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;

public class FlandernstrasseOpenSubgame : Subgame
{
    public override bool ExpectsReaction
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override void Awake()
    {
    }

    public override SubgameState Run()
    {

    }

    public override void Start()
    {
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
    }

    private DateTime currentTime;
    private LinkedList<WeekbasedDateTimeInterval> openingHours;
    private void AdvanceTimeMins();
    private void UpdateTimeView();
    private void SetRandomTime();
    private void LoadXML();


    protected override bool HasTaskExpired()
    {
        return false; // dummy value
    }

    /*
     * <openingHours>
	<timeInterval>
		<weekday>Monday</weekday>
		<start>06:30</start>
		<end>19:00</end>
	</timeInterval>
	<timeInterval>
		<weekday>Tuesday</weekday>
		<start>06:30</start>
		<end>19:00</end>
	</timeInterval>
	<timeInterval>
		<weekday>Wednesday</weekday>
		<start>06:30</start>
		<end>19:00</end>
	</timeInterval>
	<timeInterval>
		<weekday>Thursday</weekday>
		<start>06:30</start>
		<end>19:00</end>
	</timeInterval>
	<timeInterval>
		<weekday>Friday</weekday>
		<start>06:30</start>
		<end>19:00</end>
	</timeInterval>
</openingHours>
*/
}
 