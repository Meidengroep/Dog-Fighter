using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    public string SinglePlayerButtonText;
    public string MultiplayerButtonText;
    public string QuitButtonText;

    public float ButtonWidth;

    public float TopMargin;
    public float BottomMargin;

    public float ButtonVerticalSpacing;

    void OnGUI()
    {
        float nrOfButtons = 3;
        float buttonAreaHeight = Screen.height * (1 - TopMargin - BottomMargin) - (nrOfButtons - 1) * ButtonVerticalSpacing;
        float buttonHeight = buttonAreaHeight / nrOfButtons;

        int i = 0;

        // singleplayer button
        if (
            GUI.Button(
            new Rect(Screen.width / 2 - ButtonWidth / 2,
                     TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                     ButtonWidth,
                     buttonHeight),
            new GUIContent(SinglePlayerButtonText))
            )
        {
            GlobalSettings.SinglePlayer = true;
            Application.LoadLevel("ScenePickMenu");
        }

        // multiplayer button
        if (
            GUI.Button(
            new Rect(Screen.width / 2 - ButtonWidth / 2,
                     TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                     ButtonWidth,
                     buttonHeight),
            new GUIContent(MultiplayerButtonText))
            )
        {
            GlobalSettings.SinglePlayer = false;
            Application.LoadLevel("ClientServerMenu");
        }


        // exit button
        if (
            GUI.Button(
            new Rect(Screen.width / 2 - ButtonWidth / 2,
                     TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                     ButtonWidth,
                     buttonHeight),
            new GUIContent(QuitButtonText))
            )
        {
            Debug.Log("Quit pressed. Quit does not work in the editor, only in standalone builds.");
            Application.Quit();
        }
    }
}
