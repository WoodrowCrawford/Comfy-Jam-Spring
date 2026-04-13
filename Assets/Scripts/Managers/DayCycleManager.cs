using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the day cycle in the game, including the current tasks. 
/// </summary>
public class DayCycleManager : MonoBehaviour
{
   
    public delegate void DayPhaseChangeEventHandler();
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowInfoCard;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsInitializePlayerName;

    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowHouseSceneDayTime;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToHideHouseSceneDayTime;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowHouseSceneNightTime;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToHideHouseSceneNightTime;

    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowRewardsScreen;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToHideRewardsScreen;


    public static event DayPhaseChangeEventHandler OnNewDay;
    public static event DayPhaseChangeEventHandler OnWeekReset;
    


    [Header("Dialogue")]
    [SerializeField] private DialogueObjectBehavior startDayDialogue;
    [SerializeField] private DialogueObjectBehavior endDayDialogue;
    [SerializeField] private DialogueObjectBehavior weekendDayDialogue;

    public enum DayPhase
    {
        Start,
        Explore,
        End
    }
  
    [SerializeField] private DayPhase currentDayPhase;
    


    [SerializeField] private int currentDay = 1;

    public static int CurrentDay { get { return FindAnyObjectByType<DayCycleManager>().currentDay; } }
    

    private DayPhase previousDayPhase;



    void Start()
    {
        StartCoroutine(StartDay());
        previousDayPhase = currentDayPhase;
    }

    public void OnValidate()
    {

        if(currentDayPhase != previousDayPhase)
        {
            if(currentDayPhase == DayPhase.Start)
            {
                StartCoroutine(StartDay());
            }
            else if(currentDayPhase == DayPhase.Explore)
            {
                StartCoroutine(ExploreForest());
            }
            else if(currentDayPhase == DayPhase.End)
            {
                StartCoroutine(EndDay());
            }


            previousDayPhase = currentDayPhase;
        }
    }


    public void ChangeDayPhase(string newPhase)
    {
       if(newPhase == "Start")
        {
            currentDayPhase = DayPhase.Start;
            StartCoroutine(StartDay());
        }
        else if(newPhase == "Explore")
        {
            currentDayPhase = DayPhase.Explore;
            StartCoroutine(ExploreForest());
        }
        else if(newPhase == "End")
        {
            currentDayPhase = DayPhase.End;
            StartCoroutine(EndDay());

        }
        Debug.Log("Changing day phase to: " + currentDayPhase);
    }


    public IEnumerator StartDay()
    {
        

        if(FindAnyObjectByType<PlayerBehavior>().PlayerName == string.Empty)
        {
            //if the player hasnt created their name yet, we want to show the prompt message to create their name
            OnDayPhaseWantsInitializePlayerName?.Invoke();
        }


        if(HUDBehavior.hasShownIntroCard == false)
        {
            //if we havent shown the intro card yet, we want to show it
            OnDayPhaseWantsToShowInfoCard?.Invoke();   
        }

       
        //wait until the player has created their name before we can show the dialogue
        yield return new WaitUntil(() => FindAnyObjectByType<PlayerBehavior>().PlayerName != string.Empty);


        //fire an event to tell the hud to show the house scene room
        OnDayPhaseWantsToShowHouseSceneDayTime?.Invoke();
        OnNewDay?.Invoke();
        
        //wait a few
        yield return new WaitForSeconds(1f);

        // Show start dialogue for days 1, 2, and 3
        if (currentDay <= 3)
        {
            //then we show the dialogue for the day
            DialogueUIBehavior.instance.ShowDialogue(startDayDialogue);

            //after the dialogue is done we can hide the house scene room
            yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
        }


        yield return StartCoroutine(ExploreForest());
    }


    //this is the main gameplay loop where the player can explore the forest
    public IEnumerator ExploreForest()
    {
        //hide the house scene room if its still active
        OnDayPhaseWantsToHideHouseSceneDayTime?.Invoke();
        
        //here the player can explore the forest.

        //when they want to end the day they can.

        yield break;
    }

    public IEnumerator EndDay()
    {
        //show the evening house scene
        OnDayPhaseWantsToShowHouseSceneNightTime?.Invoke();

        //wait a few seconds
        yield return new WaitForSeconds(1f);

        //then we can show the end of day dialogue, which we will need to create
        DialogueUIBehavior.instance.ShowDialogue(endDayDialogue);

        //at the end increase the day count and reset the day phase to start
        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
        currentDay++;
        
        //if the current day is greater than 3, start the weekend phase
        if(currentDay > 3)
        {
            yield return StartCoroutine(WeekendDay());
        }

        else if(currentDay <= 3)
        {
            //Start the loop again!
            yield return StartCoroutine(StartDay());
        }

        
    }

    public IEnumerator WeekendDay()
    {
        //show the weekend house scene, which we will need to create
        OnDayPhaseWantsToShowHouseSceneNightTime?.Invoke();

        //play the weekend dialogue, which we will need to create
        DialogueUIBehavior.instance.ShowDialogue(weekendDayDialogue);

        //wait until the dialogue is done
        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);

        //then show the rewards screen, which we will need to create
        OnDayPhaseWantsToShowRewardsScreen?.Invoke();


        yield return StartCoroutine(StartDay());
    
        //wait unitl the rewards screen is done showing
        yield return new WaitUntil(() => !HUDBehavior.IsRewardsScreenActive);


        //reset the day count back to 1
        currentDay = 1;

        //fire an event to tel the player to reset their points back to 0
        OnWeekReset?.Invoke();

        //start the morning phase again
        yield return StartCoroutine(StartDay());
    }
}
