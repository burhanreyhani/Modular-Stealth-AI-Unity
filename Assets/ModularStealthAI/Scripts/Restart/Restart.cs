using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    Input Input;

    void Awake()
    {
        Input = new Input();
    }

    void OnEnable()
    {
        Input.Enable();
    }

    void OnDisable()
    {
        Input.Disable();
    }

    void Update()
    {
        float restartLevel = Input.Character.Restart.ReadValue<float>();

        if (restartLevel > 0.1f)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
