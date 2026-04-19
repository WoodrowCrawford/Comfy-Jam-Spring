using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class RewardsBehavior : MonoBehaviour
{
    public enum HatType
    {
        Red,
        Blue,
        Green
    }

    public delegate void RewardsEventHandler();
    public static event RewardsEventHandler OnRewardsScreenComplete;
    public delegate void HatRewardSelectedHandler(HatType selectedHatType, Sprite selectedHat);
    public static event HatRewardSelectedHandler OnHatRewardSelected;


   
    [Header("Rewards Text")]
    [SerializeField] private TMP_Text rewardsHeaderText;
    [SerializeField] private TMP_Text playerPointsText;

    [Header("Progress Bar")]
    [SerializeField] private Slider progressBarSlider;
    [SerializeField] private float fillTime = 3f;
    [SerializeField] private int maxPointsForFullRewards = 10;
    [SerializeField] private int maxIncreasePerOverflow = 5;


    [Header("Rewards")]
    [SerializeField] private GameObject hatRewardOptionsParent;
    [SerializeField] private GameObject hatRewardButtonPrefab;
    [SerializeField] private Sprite redHatSprite;
    [SerializeField] private Sprite blueHatSprite;

    [Header("Bools")]
    public bool BlueHatRewardChosen { get; private set; }
    public bool RedHatRewardChosen { get; private set; }


    private Coroutine fillCoroutine;
    private readonly List<Button> spawnedHatRewardButtons = new();
    private bool hatRewardWasSelected;


    private void OnEnable()
    {
        HUDBehavior.OnRewardsScreenOpen += StartWeeklyRewardCalculation;
    }



    private void OnDisable()
    {
        HUDBehavior.OnRewardsScreenOpen -= StartWeeklyRewardCalculation;

        ClearSpawnedHatRewardButtons();
    }

    private void ClearSpawnedHatRewardButtons()
    {

        foreach (Button spawnedHatRewardButton in spawnedHatRewardButtons)
        {
            if (spawnedHatRewardButton == null)
            {
                continue;
            }

            spawnedHatRewardButton.onClick.RemoveAllListeners();
            Destroy(spawnedHatRewardButton.gameObject);
        }

        spawnedHatRewardButtons.Clear();
    }




    //function that fills the progress bar over time depending on the time given and the points the player has
    private IEnumerator FillProgressBar(int pointsToFill)
    {
       //create a local variable to keep track of the current points filled, the max points for the current rewards level, and the total points to fill
        float totalPointsToAdd = pointsToFill;
        int currentMax = Mathf.Max(1, Mathf.RoundToInt(progressBarSlider.maxValue));
        float currentValue = Mathf.Clamp(progressBarSlider.value, 0f, currentMax);

        //set the progress bar's max value to the current max and the current value to the current value
        progressBarSlider.minValue = 0f;
        progressBarSlider.maxValue = currentMax;
        progressBarSlider.value = currentValue;

        //calculate the time it should take to fill one point based on the total points to fill and the fill time
        float secondsPerPoint = 0f;
        if (fillTime > 0f)
        {
            secondsPerPoint = fillTime / totalPointsToAdd;
        }

        //while we still have points to fill, we want to fill the progress bar
        float remainingPointsToFill = pointsToFill;
        while (remainingPointsToFill > 0f)
        {
            //calculate how many points we can fill in this pass, which is the minimum of the remaining points to fill and the points until we reach the current max
            float pointsUntilMax = currentMax - currentValue;

            //if we can't fill any points without overflowing, we need to level up the bar before we can continue filling
            if (pointsUntilMax <= 0f)
            {
                LevelUpBar(ref currentMax, ref currentValue);
                continue;
            }

            //get the remaining points to fill for this pass
            float pointsThisPass = Mathf.Min(remainingPointsToFill, pointsUntilMax);
            float targetValue = currentValue + pointsThisPass;
            float passDuration = secondsPerPoint * pointsThisPass;

            //animate the slider value from the current value to the target value over the duration of this pass
            yield return StartCoroutine(AnimateSliderValue(currentValue, targetValue, passDuration));

            //set the current value to the target value and subtract the points we just filled from the remaining points to fill
            currentValue = targetValue;
            remainingPointsToFill -= pointsThisPass;

            //if we have filled enough points to reach or exceed the current max, we need to level up the bar before we can continue filling
            if (currentValue >= currentMax)
            {
                LevelUpBar(ref currentMax, ref currentValue);
            }
        }
    }

    //this function will increase the max value of the progress bar and reset the current value to 0 when the player overflows the progress bar
    private void LevelUpBar(ref int currentMax, ref float currentValue)
    {
        currentMax += Mathf.Max(1, maxIncreasePerOverflow);
        maxPointsForFullRewards = currentMax;
        progressBarSlider.maxValue = currentMax;
        currentValue = 0f;
        progressBarSlider.value = 0f;
    }


    //A function that animates the slider value from a start value to a target value over a duration
    private IEnumerator AnimateSliderValue(float startValue, float targetValue, float duration)
    {
        //set the slider value to the start value at the beginning of the animation
        progressBarSlider.value = startValue;

        //if the start value and target value are approximately the same, we can skip the animation
        if (Mathf.Approximately(startValue, targetValue))
        {
            yield break;
        }

        //if the duration is 0 or negative, we can just set the slider value to the target value immediately
        if (duration <= 0f)
        {
            progressBarSlider.value = targetValue;
            yield break;
        }

        //animate the slider value from the start value to the target value over the duration
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            //update the elapsed time based on the delta time and the fill time
            elapsedTime += Time.deltaTime *(maxPointsForFullRewards / progressBarSlider.maxValue) * fillTime;

            //calculate the new slider value based on the elapsed time and the total duration, and set the slider value to that
            float t = 0f;
            if (duration > 0f)
            {
                t = elapsedTime / duration;
            }

            //if the duration is 0 or negative, we can just set the slider value to the target value immediately
            else
            {
                t = 1f;
            }

            //lerp the slider value from the start value to the target value based on t, and set the slider value to that
            progressBarSlider.value = Mathf.Lerp(startValue, targetValue, Mathf.Clamp01(t));
            yield return null;
        }

        //set the slider value to the target value at the end of the animation
        progressBarSlider.value = targetValue;
    }


    //this function will start the process of calculating the rewards for the player based on the points they have
    public IEnumerator StartWeeklyRewardCalculationSetUp()
    {
        hatRewardWasSelected = false;

        //set the rewards header text to "End of week report"
        rewardsHeaderText.text = "End of week report";

        //first we want to hide the rewards text and the progress bar
        playerPointsText.gameObject.SetActive(false);
        progressBarSlider.gameObject.SetActive(false);

        //make sure the rewards options are hidden
        hatRewardOptionsParent.SetActive(false);
        ClearSpawnedHatRewardButtons();

        //then we wait a second
        yield return new WaitForSeconds(1f);

        //then we show the the points game object
        playerPointsText.gameObject.SetActive(true);
       
        //then we get the player's points and show them on the screen
        PlayerBehavior playerBehavior = FindAnyObjectByType<PlayerBehavior>();
        playerPointsText.text = "Points: " + playerBehavior.WeeklyPoints.ToString();


        //then we wait a second
        yield return new WaitForSeconds(1f);

        //then we show the rewards bar
        progressBarSlider.gameObject.SetActive(true);
      

        //then we show the rewards that the player has earned based on the points they have
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        //start the coroutine to fill the progress bar based on the player's points
        fillCoroutine = StartCoroutine(FillProgressBar(playerBehavior.WeeklyPoints));

        //wait until the progress bar is done filling
        yield return fillCoroutine;

        //check to see if a level up happened
        {
            int currentMax = Mathf.RoundToInt(progressBarSlider.maxValue);
            if (playerBehavior.WeeklyPoints >= currentMax)
            {
                //if the player filled the bar enough to level up, we can show a message or animation here to indicate that they have leveled up their rewards
                Debug.Log("Player leveled up their rewards!");

                yield return ChooseHatReward();
            }
        }

        if (!hatRewardWasSelected)
        {
            yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame);
        }

        //hide the rewards parent object to hide all rewards UI elements
        hatRewardOptionsParent.SetActive(false);
        ClearSpawnedHatRewardButtons();

        //then we can hide this rewards screen and show the next scene, which we will need to create
        OnRewardsScreenComplete?.Invoke();

        yield break;
    }
    

    //A function to handle when a hat button is clicked, which will invoke the OnHatRewardSelected event with the selected hat type and sprite
    private void HandleHatButtonClicked(HatType hatType, Sprite hatSprite)
    {
        if (hatSprite == null)
        {
            Debug.LogError("Hat sprite is null!");
            return;
        }

        OnHatRewardSelected?.Invoke(hatType, hatSprite);

        //here we would set the player's hat for the day based on which button they clicked
        if (hatType == HatType.Red)
        {
            Debug.Log("Player chose the red hat reward!");
            RedHatRewardChosen = true;
        }
        else if (hatType == HatType.Blue)
        {
            Debug.Log("Player chose the blue hat reward!");
            BlueHatRewardChosen = true;
        }

        hatRewardWasSelected = true;
    }


    //A function to create a new hat reward button in the rewards screen for a given hat sprite, and add a listener to the button to handle when it is clicked
    public void CreateHatRewardButton(HatType hatType, Sprite hatSprite)
    {
        GameObject spawnedButtonObject = Instantiate(hatRewardButtonPrefab, hatRewardOptionsParent.transform);
        Button spawnedHatRewardButton = spawnedButtonObject.GetComponent<Button>();
        spawnedButtonObject.GetComponent<Image>().preserveAspect = true;

        spawnedButtonObject.transform.GetChild(0).GetComponent<Image>().sprite = hatSprite;

        spawnedHatRewardButton.onClick.AddListener(() => HandleHatButtonClicked(hatType, hatSprite));
        spawnedHatRewardButtons.Add(spawnedHatRewardButton);
    }


    //A function that will allow the player to choose a reward from the rewards screen
    //For now will be a placeholder until we get hat assets
    public IEnumerator ChooseHatReward()
    {
        hatRewardWasSelected = false;
        ClearSpawnedHatRewardButtons();

        //if player has already chosen both hat rewards, we can skip this step
        if(BlueHatRewardChosen && RedHatRewardChosen)        
        {
            Debug.Log("Player has already chosen both hat rewards, skipping hat reward selection!");
            yield break;
        }

        Debug.Log("Player can choose a hat reward!");

        //change the rewards header text to "Choose your hat reward!"
        rewardsHeaderText.text = "Choose your hat!";

        //show the hat reward options
        hatRewardOptionsParent.SetActive(true);

        //hide the points text and the progress bar
        playerPointsText.gameObject.SetActive(false);
        progressBarSlider.gameObject.SetActive(false);

        //create the hat reward buttons

        if(!RedHatRewardChosen)
        {
            CreateHatRewardButton(HatType.Red, redHatSprite);
        }
        if(!BlueHatRewardChosen)
        {
            CreateHatRewardButton(HatType.Blue, blueHatSprite);
        }

        yield return new WaitUntil(() => hatRewardWasSelected);

        yield break;
    }


    //this function will be called when the rewards screen is opened and will start the process of calculating the rewards for the player based on the points they have
    public void StartWeeklyRewardCalculation()
    {
        StartCoroutine(StartWeeklyRewardCalculationSetUp());
    }
}
