using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public GameObject MenuPanel;
    public GameObject Spawn;
    private LevelManager lm;
    private List<UnityAction> buttonFunctions;
    private List<string> buttonText;
    public bool AddContinue = false;
    void Start()
    {
        MenuPanel.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 50);
    }

    public void Continue()
    {
        foreach(var el in lm.elementsToPause)
        {
            el.SetActive(true);
        }
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }

    public void NewGame()
    {
        AddContinue = true;
        lm.CreateNextLevel(1);
        this.gameObject.SetActive(false);
    }
    public void OpenOptions()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
    // генерує колекцію функцій кнопок
    private void ActoinsToList(bool AddContinue = false)
    {
        buttonFunctions = new List<UnityAction>();
        if (AddContinue)
        { buttonFunctions.Add(new UnityAction(Continue)); }
        buttonFunctions.Add(new UnityAction(NewGame));
        buttonFunctions.Add(new UnityAction(OpenOptions));
        buttonFunctions.Add(new UnityAction(Quit));
    }
    // генерує колекцію тексту кнопок
    private void TextToList(bool AddContinue = false)
    {
        buttonText = new List<string>();
        if (AddContinue)
        { buttonText.Add("Continue"); }
        buttonText.Add("New Game");
        buttonText.Add("Options");
        buttonText.Add("Quit");
    }

    void Update()
    {
        
    }
}
