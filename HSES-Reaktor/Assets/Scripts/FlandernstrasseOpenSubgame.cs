using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;

public class FlandernstrasseOpenSubgame : Subgame
{
    // paths of contained objects in the prefab
    private readonly string[] shownTimeViewPaths = { taskViewNames[0] + "/Clock", taskViewNames[1] + "/Clock" };
    private readonly string[] shownWeekdayViewPaths = { taskViewNames[0] + "/Calendar/Text", taskViewNames[1] + "/Calendar/Text" };

    // references to displayed time and weekday in both TaskViews
    private Text[] shownTimeViews = new Text[taskViewsCount];
    private Text[] shownWeekdayViews = new Text[taskViewsCount];
    private int determinedFontSize;

    private DateTime currentTime = new DateTime(1, 1, 1, 0, 0, 0);
    private LinkedList<WeekbasedDateTimeInterval> openingHours;

    /***********************************************************/
    /********************** Unity Methods **********************/
    /***********************************************************/

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < taskViewsCount; i++)
        {
            shownTimeViews[i] = GameObject.Find(shownTimeViewPaths[i]).GetComponent<Text>();
            shownWeekdayViews[i] = GameObject.Find(shownWeekdayViewPaths[i]).GetComponent<Text>();
        }

        LoadXML();
        SetRandomDateTime();
        determinedFontSize = shownTimeViews[0].fontSize;
        UpdateTimeView();
        UpdateWeekdayView();
    }

    public override void Start()
    {
    }

    public override void Update()
    {
        AdvanceTimeMins();
        UpdateTimeView();
        UpdateWeekdayView();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /***********************************************************/
    /*********************** User Methods **********************/
    /***********************************************************/

    protected override bool OnExpectsReaction()
    {
        foreach (WeekbasedDateTimeInterval checkingInterval in openingHours)
        {
            if (checkingInterval.Contains(currentTime))
            {
                return true;
            }
        }
        return false;
    }

    private void AdvanceTimeMins()
    {
        currentTime = currentTime.AddMinutes(1);
    }

    private void SetRandomDateTime()
    {
        System.Random randomNumberCreator = new System.Random();

        int randomAddingMins = randomNumberCreator.Next(0, WeekbasedDateTimeInterval.minutesInWeek);
        currentTime = currentTime.AddMinutes(randomAddingMins);
    }

    private void UpdateTimeView()
    {
        // Create padded hours and minutes strings.
        string newHours = currentTime.Hour.ToString("D2");
        string newMinutes = currentTime.Minute.ToString("D2");
        const char delimiter = ':';

        string shownTime = String.Concat(newHours, delimiter, newMinutes);

        foreach(Text updatingTimeView in shownTimeViews)
        {
            updatingTimeView.text = shownTime;
            updatingTimeView.fontSize = determinedFontSize;
        }
    }

    private void UpdateWeekdayView()
    {
        foreach(Text updatingWeekdayView in shownWeekdayViews)
        {
            switch(currentTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    updatingWeekdayView.text = "Mo";
                    break;

                case DayOfWeek.Tuesday:
                    updatingWeekdayView.text = "Di";
                    break;

                case DayOfWeek.Wednesday:
                    updatingWeekdayView.text = "Mi";
                    break;

                case DayOfWeek.Thursday:
                    updatingWeekdayView.text = "Do";
                    break;

                case DayOfWeek.Friday:
                    updatingWeekdayView.text = "Fr";
                    break;

                case DayOfWeek.Saturday:
                    updatingWeekdayView.text = "Sa";
                    break;

                case DayOfWeek.Sunday:
                    updatingWeekdayView.text = "So";
                    break;
            }
        }
    }

    private void LoadXML()
    {
        WeekbasedDateTimeInterval addingInterval = new WeekbasedDateTimeInterval(DayOfWeek.Tuesday, 6, 30, DayOfWeek.Tuesday, 19, 0); // dummy value

        openingHours = new LinkedList<WeekbasedDateTimeInterval>();
        openingHours.AddLast(addingInterval);

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
    
    protected override bool HasTaskExpired()
    {
        return false; // dummy value
    }

    protected override void PlayFadeInAnimations()
    {
        
    }

    protected override void PlayFadeOutAnimations()
    {
        
    }

    protected override void OnLoadNewTask()
    {
        SetRandomDateTime();
        UpdateTimeView();
        UpdateWeekdayView();
    }

    protected override void OnActiveEntry()
    {
    }

    protected override void OnPauseEntry()
    {
    }

    protected override void OnActiveExit()
    {
    }

    protected override void OnFadingInExit()
    {
    }
}
 