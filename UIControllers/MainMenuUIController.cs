using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : BaseUIController
{
    [SerializeField] private MainMenuStateManager stateManager;

    [SerializeField] private GameObject mainButtonsGroup;
    [SerializeField] private Button editFrogButton;

    [SerializeField] private GameObject editFrogGroup;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private ColorPickerController colorPickerController;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private GameObject frogInTheMiddle;


    private Dictionary<ProgramStates.MainMenuStates, GameObject> stateToUIGroup = new Dictionary<ProgramStates.MainMenuStates, GameObject>();



    private void Start()
    {
        stateToUIGroup[ProgramStates.MainMenuStates.MainButtons] = mainButtonsGroup;
        stateToUIGroup[ProgramStates.MainMenuStates.EditFrog] = editFrogGroup;

        stateManager.OnStateChanged += StateManager_OnStateChanged;

        PlayerInfoHolder.LoadPlayerPrefs();

        SyncFrogColorAndNameWithPlayerInfoHolder();

        colorPickerController.OnSelectedColorChange += ColorPickerController_OnSelectedColorChange;

        editFrogButton.onClick.AddListener(() =>
        {
            SyncColorPickerWithPlayerInfoHolder();
        });

        nameInputField.onEndEdit.AddListener((value) =>
        {
            if (value.Length == 0)
            {
                nameInputField.text = PlayerInfoHolder.FrogName;
            }
        });

        cancelButton.onClick.AddListener(() =>
        {
            SyncFrogColorAndNameWithPlayerInfoHolder();
        });

        saveButton.onClick.AddListener(() =>
        {
            PlayerInfoHolder.SavePlayerPrefs(nameInputField.text, colorPickerController.GetCurrentlySelectedColor());
        });
    }

    private void ColorPickerController_OnSelectedColorChange(object sender, System.EventArgs e)
    {
        frogInTheMiddle.transform.Find("Body").GetComponent<Image>().color = colorPickerController.GetCurrentlySelectedColor();
    }

    private void StateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        var args = e as EventArgsCollection.MainMenuStateChangeArgs;
        var changeFrom = args.CurrentState;
        var changeTo = args.NewState;

        if (changeFrom == ProgramStates.MainMenuStates.IntroAnimation)
        {
            SwitchLayout(null, stateToUIGroup[changeTo]);
        }
        else if (changeTo == ProgramStates.MainMenuStates.OutroAnimation)
        {
            SwitchLayout(stateToUIGroup[changeFrom], null);
            PlayOutroAnimation();
        }
        else
        {
            SwitchLayout(stateToUIGroup[changeFrom], stateToUIGroup[changeTo]);
        }
    }




    private void PlayOutroAnimation()
    {
        frogInTheMiddle.GetComponent<Animator>().SetTrigger("jumpOut");
    }

    private void SyncFrogColorAndNameWithPlayerInfoHolder()
    {
        frogInTheMiddle.transform.Find("Body").GetComponent<Image>().color = PlayerInfoHolder.FrogColor;
        nameInputField.text = PlayerInfoHolder.FrogName;
    }

    private void SyncColorPickerWithPlayerInfoHolder()
    {
        colorPickerController.SetSlidersToColor(PlayerInfoHolder.FrogColor);
    }
}
