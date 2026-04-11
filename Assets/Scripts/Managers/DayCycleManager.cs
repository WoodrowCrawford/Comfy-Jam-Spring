using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the day cycle in the game, including the current tasks. 
/// </summary>
public class DayCycleManager : MonoBehaviour
{
   
    public delegate void DayPhaseChangeEventHandler();
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowInfoCard;

    public enum DayPhase
    {
        Start,
        Explore,
        End
    }
  
    [SerializeField] private DayPhase currentDayPhase;
    


    [SerializeField] private int currentDay = 1;
    

    private DayPhase previousDayPhase;


    [Header("Dialogue")]
    [SerializeField] private DialogueObjectBehavior startDayDialogue;

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
        //this only fires  on the first day if it hasnt been shown yet
        OnDayPhaseWantsToShowInfoCard?.Invoke();

        yield break;
    }


    //this is the main gameplay loop where the player can explore the forest
    public IEnumerator ExploreForest()
    {
        Debug.Log("Exploring the forest...");
        yield return new WaitForSeconds(1f); // Simulate some delay for exploring the forest
        Debug.Log("You can explore the forest and find various items, but be careful of the dangers lurking around!");

        // Here we can have the player explore the forest and find various items, but also encounter dangers such as wild animals or traps.


        //once the player is ready to end the day we cn move to the next phase of the game

        yield break;
    }

    public IEnumerator EndDay()
    {
        Debug.Log("Ending day cycle...");
        yield return new WaitForSeconds(1f); // Simulate some delay for ending the day
        Debug.Log("You can now rest and prepare for the next day.");

        // Here we can have the player rest and prepare for the next day, maybe by crafting items or upgrading their equipment.


        //then we start the next day cycle again

        yield break;
    }
}
