using UnityEngine;
using System.Collections;

public class ScenePickMenu : MonoBehaviour 
{
    public ScenePickButton[] SceneButtons;

    public float ButtonWidth;

    public float TopMargin;
    public float BottomMargin;

    public float ButtonVerticalSpacing;

    void OnGUI()
    {
        float buttonAreaHeight = Screen.height * (1 - TopMargin - BottomMargin) - (SceneButtons.Length - 1) * ButtonVerticalSpacing;
        float buttonHeight = buttonAreaHeight / SceneButtons.Length;

        for (int i = 0; i < SceneButtons.Length; i++)
        {
            if (
                GUI.Button(
                new Rect(Screen.width / 2 - ButtonWidth / 2,
                         TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i,
                         ButtonWidth,
                         buttonHeight),
                new GUIContent(SceneButtons[i].ButtonText))
                )
            {
                Application.LoadLevel(SceneButtons[i].SceneName);
            }
        }
    }
}
