using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WardrobeBehavior : MonoBehaviour
{

    [SerializeField] private Animator playerAnimator;

    public GameObject hatsParent;
    public GameObject hatButtonPrefab;
    private readonly List<Button> spawnedHatButtons = new();
    
    public static bool playerPickedHatForTheDay = false;


    [Header("Buttons")]
    [SerializeField] private Button goAsIsButton;

    [Header("Hat Sprites")]
    [SerializeField] private Sprite redHatSprite;
    [SerializeField] private Sprite blueHatSprite;
    [SerializeField] private Sprite greenHatSprite;

    [Header("Bools")]
    [SerializeField] private bool redHatEquipped;
    [SerializeField] private bool blueHatEquipped;
    [SerializeField] private bool greenHatEquipped;




    void Awake()
    {
        PlayerBehavior playerBehavior = FindAnyObjectByType<PlayerBehavior>();
        RewardsBehavior.OnHatRewardSelected += HandleRewardHatSelected;
    }



    void OnEnable()
    {
        Debug.Log("WardrobeBehavior enabled, wiring Go As Is button.");
        goAsIsButton.onClick.AddListener(() =>
        {
            Debug.Log("Go As Is button clicked.");
            playerPickedHatForTheDay = true;
        });
    }


    void OnDisable()
    {
        Debug.Log("WardrobeBehavior disabled, unsubscribing from events.");
        goAsIsButton.onClick.RemoveAllListeners();
    }

    void OnDestroy()
    {
        RewardsBehavior.OnHatRewardSelected -= HandleRewardHatSelected;
    }


    void Start()
    {
        blueHatEquipped = false;
        redHatEquipped = false;
        greenHatEquipped = true;

        CreateHatSlot(RewardsBehavior.HatType.Green, greenHatSprite);
    }


    private void HandleRewardHatSelected(RewardsBehavior.HatType hatType, Sprite hatSprite)
    {
        CreateHatSlot(hatType, hatSprite);
    }



    //a function to handle when a hat button is clicked, which will equip the hat the player selected
    private void HandleHatButtonClicked(RewardsBehavior.HatType hatType)
    {

        //if they clicked the red hat button, equip the red hat, if they clicked the blue hat button, equip the blue hat
        if (hatType == RewardsBehavior.HatType.Red)
        {
            Debug.Log("Red hat button clicked, equipping red hat.");
            playerPickedHatForTheDay = true;
            redHatEquipped = true;
            blueHatEquipped = false;
            greenHatEquipped = false;

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("BlueHatPicked", false);
                playerAnimator.SetBool("GreenHatPicked", false);
                playerAnimator.SetBool("RedHatPicked", true);
                playerAnimator.Play("Base Layer.Idle(RedHat)", 0, 0f);
            }
            else
            {
                Debug.LogWarning("Player animator not found! Cannot play equip red hat animation.");
            }
            
        }

        else if (hatType == RewardsBehavior.HatType.Blue)
        {
            Debug.Log("Blue hat button clicked, equipping blue hat.");
            playerPickedHatForTheDay = true;
            blueHatEquipped = true;
            redHatEquipped = false;
            greenHatEquipped = false;

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("RedHatPicked", false);
                playerAnimator.SetBool("GreenHatPicked", false);
                playerAnimator.SetBool("BlueHatPicked", true);
                playerAnimator.Play("Base Layer.Idle(BlueHat)", 0, 0f);
            }
            else
            {
                Debug.LogWarning("Player animator not found! Cannot play equip blue hat animation.");
            }
        }

        else if (hatType == RewardsBehavior.HatType.Green)
        {
            Debug.Log("Green hat button clicked, equipping green hat.");
            playerPickedHatForTheDay = true;
            greenHatEquipped = true;
            redHatEquipped = false;
            blueHatEquipped = false;

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("RedHatPicked", false);
                playerAnimator.SetBool("BlueHatPicked", false);
                playerAnimator.SetBool("GreenHatPicked", true);
                playerAnimator.Play("Base Layer.Idle(GreenHat)", 0, 0f);
            }
            else
            {
                Debug.LogWarning("Player animator not found! Cannot play equip green hat animation.");
            }
        }
    }


    //a function to create a new hat slot in the wardrobe for a given hat sprite, and add a listener to the button to equip the hat when clicked
    public void CreateHatSlot(RewardsBehavior.HatType hatType, Sprite hatSprite)
    {

        //create a new hat button in the wardrobe for the given hat sprite
        GameObject spawnedButtonObject = Instantiate(hatButtonPrefab, hatsParent.transform);
        Button spawnedHatButton = spawnedButtonObject.GetComponent<Button>();
        spawnedButtonObject.GetComponent<Image>().preserveAspect = true;

        //set the button's image to be the hat sprite
        spawnedButtonObject.transform.GetChild(0).GetComponent<Image>().sprite = hatSprite;

        //add a listener to the button to equip the hat when clicked
        spawnedHatButton.onClick.AddListener(() => HandleHatButtonClicked(hatType));

       
        spawnedHatButtons.Add(spawnedHatButton);
    }
    
    

}

