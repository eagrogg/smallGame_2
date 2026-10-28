using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public void StartNewGame()
    {
        StartCoroutine(SaveManager.instance.LoadNewGame());
    }
}
