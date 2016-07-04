using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;

public class FlandernstrasseOpenSubgame : Subgame
{
    private enum ParsingState { awaitingOpeningHours, readingOpeningHours, readingTimeInterval, readingWeekday, readingStartTime, readingEndTime, done };

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
                DecreaseRemainingWins();
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
        const string subgameResPath         = @".\Assets\Resources\FlandernstrasseOpenSubgame.xml";

        const string elemNameOpeningHours   = "openingHours";
        const string elemNameTimeInterval   = "timeInterval";
        const string elemNameWeekday        = "weekday";
        const string elemNameStartTime      = "start";
        const string elemNameEndTime        = "end";

        const char timeDelimiter = ':';

        char[] ignoreChars = { '\t', '\r', '\n' };

        DayOfWeek loadedWeekday = DayOfWeek.Monday;
        int loadedStartHours = 0;
        int loadedStartMinutes = 0;
        int loadedEndHours = 0;
        int loadedEndMinutes = 0;

        XmlReader subgameResourcesReader = XmlReader.Create(subgameResPath);
        ParsingState state = ParsingState.awaitingOpeningHours;
        bool readSuccess = false;
        
        while (ParsingState.done != state)
        {

            // TO DO: Implement DTD-check

            do
            {
                readSuccess = subgameResourcesReader.Read();
            }
            while (-1 != subgameResourcesReader.Name.IndexOfAny(ignoreChars));

            if (!readSuccess)
            {
                state = ParsingState.done;
            }

            switch (state)
            {
                case ParsingState.awaitingOpeningHours:
                    // <openingHours>
                    if (XmlNodeType.Element == subgameResourcesReader.NodeType && elemNameOpeningHours == subgameResourcesReader.Name)
                    {
                        openingHours = new LinkedList<WeekbasedDateTimeInterval>();

                        state = ParsingState.readingOpeningHours;
                    }
                    break;

                case ParsingState.readingOpeningHours:
                    // <timeInterval>
                    if (XmlNodeType.Element == subgameResourcesReader.NodeType && elemNameTimeInterval == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingTimeInterval;
                    }
                    // </openingHours>
                    else if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameOpeningHours == subgameResourcesReader.Name)
                    {
                        state = ParsingState.done;
                    }
                    break;

                case ParsingState.readingTimeInterval:
                    // </timeInterval>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameTimeInterval == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingOpeningHours;

                        WeekbasedDateTimeInterval addingInterval = new WeekbasedDateTimeInterval(loadedWeekday, loadedStartHours, loadedStartMinutes, loadedWeekday, loadedEndHours, loadedEndMinutes);
                        openingHours.AddLast(addingInterval);

                    }
                    else if (XmlNodeType.Element == subgameResourcesReader.NodeType)
                    {
                        switch(subgameResourcesReader.Name)
                        {
                            // <weekday>
                            case elemNameWeekday:
                                state = ParsingState.readingWeekday;
                                break;

                            // <start>
                            case elemNameStartTime:
                                state = ParsingState.readingStartTime;
                                break;

                            // <end>
                            case elemNameEndTime:
                                state = ParsingState.readingEndTime;
                                break;
                        }
                    }
                    break;

                case ParsingState.readingWeekday:
                    // </weekday>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameWeekday == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingTimeInterval;
                    }
                    // text inside <weekday></weekday>
                    else if (XmlNodeType.Text == subgameResourcesReader.NodeType)
                    {
                        switch (subgameResourcesReader.Value)
                        {
                            case "Monday":
                                loadedWeekday = DayOfWeek.Monday;
                                break;

                            case "Tuesday":
                                loadedWeekday = DayOfWeek.Tuesday;
                                break;

                            case "Wednesday":
                                loadedWeekday = DayOfWeek.Wednesday;
                                break;

                            case "Thursday":
                                loadedWeekday = DayOfWeek.Thursday;
                                break;

                            case "Friday":
                                loadedWeekday = DayOfWeek.Friday;
                                break;

                            case "Saturday":
                                loadedWeekday = DayOfWeek.Saturday;
                                break;

                            case "Sunday":
                                loadedWeekday = DayOfWeek.Sunday;
                                break;
                        }
                    }
                    break;

                case ParsingState.readingStartTime:
                    // </start>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameStartTime == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingTimeInterval;
                    }
                    // text inside <start></start>
                    else if (XmlNodeType.Text == subgameResourcesReader.NodeType)
                    {
                        string wholeTime = subgameResourcesReader.Value;
                        string[] timeParts = wholeTime.Split(timeDelimiter);

                        loadedStartHours = int.Parse(timeParts[0]);
                        loadedStartMinutes = int.Parse(timeParts[1]);
                    }
                    break;

                case ParsingState.readingEndTime:
                    // </end>
                    if (XmlNodeType.EndElement == subgameResourcesReader.NodeType && elemNameEndTime == subgameResourcesReader.Name)
                    {
                        state = ParsingState.readingTimeInterval;
                    }
                    // text inside <end></end>
                    else if (XmlNodeType.Text == subgameResourcesReader.NodeType)
                    {
                        string wholeTime = subgameResourcesReader.Value;
                        string[] timeParts = wholeTime.Split(timeDelimiter);

                        loadedEndHours = int.Parse(timeParts[0]);
                        loadedEndMinutes = int.Parse(timeParts[1]);
                    }
                    break;
            }
        }
    }
    
    protected override bool HasTaskExpired()
    {
        return false;
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
 