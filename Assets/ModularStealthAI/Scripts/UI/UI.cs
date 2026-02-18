using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    public ObtainKey obtainKey;
    public EscapePoint escapePoint;
    public PlayerDied playerDied;

    Input input;

    public TMP_Text infoText;

    void Start()
    {
        input = new Input();
    }

    void Update()
    {
        UpdateInfo();
    }

    void UpdateInfo()
    {
        string keyName = input.Character.Restart.GetBindingDisplayString();

        if (!escapePoint.doPlayerWin && obtainKey.hasKey && !playerDied.doPlayerDie)
        {
            infoText.text = "<< You have the key.";
        }
        else if (playerDied.doPlayerDie)
        {
            infoText.text = "<< You Died! Press " + keyName + " for restart";
        }
        else if (escapePoint.doPlayerWin && obtainKey.hasKey)
        {
            infoText.text = "<< You Won! " + keyName + " for restart";
        }
        else
        {
            infoText.text = "<< You need a key.";
        }
    }
}
