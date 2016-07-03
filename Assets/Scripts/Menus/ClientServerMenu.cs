using UnityEngine;
using System.Collections;

public class ClientServerMenu : MonoBehaviour 
{        
    public string ClientText;
    public string IpLabel;

    public float IpInputWidth;
    public float IpInputHeight;

    public string ServerText;

    public string PrevMenuText;
    public string PrevMenuScene;

    public float ButtonWidth;

    public float TopMargin;
    public float BottomMargin;

    public float ButtonVerticalSpacing;

    private string ipInputContent;

    void Start()
    {
        ipInputContent = "";
    }

    void OnGUI()
    {
        int nrOfButtoms = 4;

        float buttonAreaHeight = Screen.height * (1 - TopMargin - BottomMargin) - (nrOfButtoms - 1) * ButtonVerticalSpacing;
        float buttonHeight = buttonAreaHeight / nrOfButtoms;

        int i = 0;

        GUI.Label(
                  new Rect(Screen.width / 2 - IpInputWidth / 2 - IpLabel.Length * 6,
                           TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i - IpInputHeight,
                           IpInputWidth,
                           IpInputHeight),
                  new GUIContent(IpLabel));

        ipInputContent = GUI.TextField(
                                       new Rect(Screen.width / 2 - IpInputWidth / 2,
                                                TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i - IpInputHeight,
                                                IpInputWidth,
                                                IpInputHeight),
                                       ipInputContent);

        // Client
        if (
            GUI.Button(
            new Rect(Screen.width / 2 - ButtonWidth / 2,
                     TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                     ButtonWidth,
                     buttonHeight),
            new GUIContent(ClientText))
            )
        {
            StartAsClient(ipInputContent);
        }

        // Server
        if (
            GUI.Button(
           new Rect(Screen.width / 2 - ButtonWidth / 2,
                    TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                    ButtonWidth,
                    buttonHeight),
           new GUIContent(ServerText))
           )
        {
            StartAsServer();
        }

        // Previous Menu
        if (
            GUI.Button(
            new Rect(Screen.width / 2 - ButtonWidth / 2,
                     TopMargin * Screen.height + (buttonHeight + ButtonVerticalSpacing) * i++,
                     ButtonWidth,
                     buttonHeight),
            new GUIContent(PrevMenuText))
            )
        {
            Application.LoadLevel(PrevMenuScene);
        }
    }

    private void StartAsClient(string ipAdress)
    {
        Debug.Log(ipAdress);

        if (ipAdress != "")
            GlobalSettings.ServerIP = ipAdress;

        GlobalSettings.IsServer = false;

        Application.LoadLevel("MultiplayerPrototype1");
    }

    private void StartAsServer()
    {
        GlobalSettings.IsServer = true;

        Application.LoadLevel("MultiplayerPrototype1");
    }
}
