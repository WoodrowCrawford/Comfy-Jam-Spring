using System.Collections;
using NUnit.Framework;
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
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowWardrobeUI;
    public static event DayPhaseChangeEventHandler OnDaypPhaseWantsToHideWardrobeUI;

    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToShowRewardsScreen;
    public static event DayPhaseChangeEventHandler OnDayPhaseWantsToHideRewardsScreen;


    public static event DayPhaseChangeEventHandler OnNewDay;
    public static event DayPhaseChangeEventHandler OnWeekReset;
    


    [Header("Dialogue")]
    [SerializeField] private DialogueObjectBehavior startDayDialogue;
    [SerializeField] private DialogueObjectBehavior pickHatDialogue;

    [Header("Evening Dialogue list")]
    [SerializeField] private DialogueObjectBehavior[] eveningDialogueDayVariations;
    [SerializeField] private DialogueObjectBehavior playerReturnsHomeDialogue;

    [Header("Collectible Dialogue")]
    [SerializeField] private DialogueObjectBehavior[] grandpaCommentForAllCollectiblesVariations;
    [SerializeField] private DialogueObjectBehavior[] grandpaCommentForSomeCollectiblesVariations;

    [Header("Evening player options dialogue")]
    [SerializeField] private DialogueObjectBehavior hasGrandpaExploredForestDialogue;
    [SerializeField] private DialogueObjectBehavior lotOfThingsFoundDialogue;
    [SerializeField] private DialogueObjectBehavior mamaDialogue;

    [Header("End of Week More than 85 Percent Collected Dialogue")]
    [SerializeField] private DialogueObjectBehavior[] endOfWeekDialogueFor85PercentCollectedVariations;

    [Header("End of week less than 85 percent collected dialogue")]
    [SerializeField] private DialogueObjectBehavior[] endOfWeekDialogueForLessThan85PercentCollectedVariations;

    

    
    [SerializeField] private DialogueObjectBehavior endDayDialogue;
    [SerializeField] private DialogueObjectBehavior weekendDayDialogue;
    [SerializeField] private DialogueObjectBehavior endOfWeekDialogue;

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

    private bool playerPickedHatForTheDay = false;



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
        
        //set the player picked hat for the day to false at the start of the day
        playerPickedHatForTheDay = false;

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


        yield return StartCoroutine(PickHatForTheDay());
    }

    public IEnumerator PickHatForTheDay()
    {
        //play the dialogue for picking the hat for the day
        DialogueUIBehavior.instance.ShowDialogue(pickHatDialogue);

        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);

        //this is when the player gets to pick the hat for the day

        //fire an event to show the hat selection UI, which we will need to create
        OnDayPhaseWantsToShowWardrobeUI?.Invoke();

        yield return new WaitUntil(() => WardrobeBehavior.playerPickedHatForTheDay == true);
        OnDaypPhaseWantsToHideWardrobeUI?.Invoke();
        WardrobeBehavior.playerPickedHatForTheDay = false;

        yield return StartCoroutine(ExploreForest());
    }


    //this is the main gameplay loop where the player can explore the forest
    public IEnumerator ExploreForest()
    {
        //hide the house scene room if its still active
        OnDayPhaseWantsToHideHouseSceneDayTime?.Invoke();

        QuestUIBehavior questUI = FindAnyObjectByType<QuestUIBehavior>();
        if (questUI != null)
        {
            questUI.ToggleQuestPanel(true);
        }

        //here the player can explore the forest.

        //play the exploration music
        SoundManager.instance.PlayMusicClip(SoundManager.instance.musicObject, SoundManager.instance.explorationMusicClip, SoundManager.instance.musicObject.transform, true, 0f, 0f);

        //when they want to end the day they can.

        yield break;
    }

    public IEnumerator EndDay()
    {
        //stop the explore forest music
        SoundManager.instance.StopSoundFXClip(SoundManager.instance.explorationMusicClip);


        //show the evening house scene
        OnDayPhaseWantsToShowHouseSceneNightTime?.Invoke();

        //wait a few seconds
        yield return new WaitForSeconds(1f);


        //we want to play a random evening dialogue from the list of evening dialogues for days 1, 2, and 3
        if (eveningDialogueDayVariations != null && eveningDialogueDayVariations.Length > 0)
        {
            DialogueUIBehavior.instance.ShowDialogue(eveningDialogueDayVariations[Random.Range(0, eveningDialogueDayVariations.Length)]);
        }
        else
        {
            Debug.LogWarning("No evening dialogue variations are assigned on DayCycleManager.");
        }


        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);


        //play the dialogue for the player returning home after exploring, which we will need to create
        DialogueUIBehavior.instance.ShowDialogue(playerReturnsHomeDialogue);


        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);


        //if the player found all the collectibles for the day, we can play a special dialogue for that, which we will need to create
        if (QuestManager.HasCollectedAllQuestItems)
        {
            //play the dialogue for finding all the collectibles
            DialogueUIBehavior.instance.ShowDialogue(grandpaCommentForAllCollectiblesVariations[Random.Range(0, grandpaCommentForAllCollectiblesVariations.Length)]);
            yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);

        }
        else if (!QuestManager.HasCollectedAllQuestItems)
        {
            //play the dialogue for finding some of the collectibles
            DialogueUIBehavior.instance.ShowDialogue(grandpaCommentForSomeCollectiblesVariations[Random.Range(0, grandpaCommentForSomeCollectiblesVariations.Length)]);

            yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
        }


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

        yield return new WaitForSeconds(1f);
        //play the weekend dialogue, which we will need to create
        DialogueUIBehavior.instance.ShowDialogue(weekendDayDialogue);
        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);


        //if the player grabbed 80 percent or more of the collectibles for the week, we want to play a special dialogue for that, which we will need to create
        if (QuestManager.GetWeeklyQuestCompletionPercentage() >= 0.85f)
        {
            Debug.Log("Player collected 80% or more of the collectibles for the week, playing special dialogue.");

            //play a random dialogue from the end of week dialogue for 85 percent collected variations
            if (endOfWeekDialogueFor85PercentCollectedVariations != null && endOfWeekDialogueFor85PercentCollectedVariations.Length > 0)
            {
                DialogueUIBehavior.instance.ShowDialogue(endOfWeekDialogueFor85PercentCollectedVariations[Random.Range(0, endOfWeekDialogueFor85PercentCollectedVariations.Length)]);
                yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
            }
            else            
            {
                Debug.LogWarning("No end of week dialogue for 85 percent collected variations are assigned on DayCycleManager.");
            }
        }
        //else if the player grabbed less than 85 percent of the collectibles
        else if (QuestManager.GetWeeklyQuestCompletionPercentage() < 0.85f)
        {
            Debug.Log("Player collected less than 85% of the collectibles for the week, playing different dialogue.");
            if (endOfWeekDialogueForLessThan85PercentCollectedVariations != null && endOfWeekDialogueForLessThan85PercentCollectedVariations.Length > 0)
            {
                DialogueUIBehavior.instance.ShowDialogue(endOfWeekDialogueForLessThan85PercentCollectedVariations[Random.Range(0, endOfWeekDialogueForLessThan85PercentCollectedVariations.Length)]);
                yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
            }
            else
            {
                Debug.LogWarning("No end of week dialogue for less than 85 percent collected variations are assigned on DayCycleManager.");
            }
        }

        
        //wait until the dialogue is done
        yield return new WaitUntil(() => !DialogueUIBehavior.IsOpen);
        OnDayPhaseWantsToHideHouseSceneNightTime?.Invoke();

        //then show the rewards screen, which we will need to create
        OnDayPhaseWantsToShowRewardsScreen?.Invoke();

    
        //wait until the rewards screen is done showing
        yield return new WaitUntil(() => !HUDBehavior.IsRewardsScreenActive);


        //reset the day count back to 1
        currentDay = 1;

        //fire an event to tel the player to reset their points back to 0
        OnWeekReset?.Invoke();

        //reset the weekly quest progress
        QuestManager.ResetWeeklyQuestProgress();

        //start the morning phase again
        yield return StartCoroutine(StartDay());
    }
}
