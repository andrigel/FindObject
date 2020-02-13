using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject ButtonExample;
    public GameObject Options;
    public GameObject Spawn;
    public GameObject OptionsBackButton;
    public GameObject Scrollbar;
    public GameObject Value;
    public AudioClip MenuAudio;
    private LevelManager lm;
    private List<UnityAction> buttonFunctions;
    private List<string> buttonText;
    private Scrollbar scrollbar;
    

    void Start()
    {
        GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 70);
        lm = Spawn.GetComponent<LevelManager>();
        InstantiateButtons();
        OptionsBackButton.GetComponent<Button>().onClick.AddListener(new UnityAction(OptionsBack));
        scrollbar = Scrollbar.GetComponent<Scrollbar>();
        scrollbar.onValueChanged.AddListener(delegate { ChangeVolume(); });
        scrollbar.value = 0.5f;
    }
    public void NewGame()
    {
        lm.CreateNextLevel(1);
        this.gameObject.SetActive(false);
    }

    public void OpenOptions()
    {
        var buttons = GameObject.FindGameObjectsWithTag("MenuButton");
        foreach (var b in buttons)
        {
            Destroy(b);
        }
        Options.SetActive(true);
    }

    public void OptionsBack()
    {
        InstantiateButtons();
        Options.SetActive(false);
    }
    void ChangeVolume()
    {
        AudioListener.volume = (scrollbar.value);
        Value.GetComponent<Text>().text = ((int)(scrollbar.value * 100)).ToString() + "%";
    }



    public void Quit()
    {
        Application.Quit();
    }
    // генерує колекцію функцій кнопок
    private void ActionsToList()
    {
        buttonFunctions = new List<UnityAction>();

        buttonFunctions.Add(new UnityAction(NewGame));
        buttonFunctions.Add(new UnityAction(OpenOptions));
        buttonFunctions.Add(new UnityAction(Quit));
    }
    // генерує колекцію тексту кнопок
    private void TextToList()
    {
        buttonText = new List<string>();

        buttonText.Add("Нова гра");
        buttonText.Add("Опції");
        buttonText.Add("Вихід");
    }

    private void InstantiateButtons()
    {
        var buttons = GameObject.FindGameObjectsWithTag("MenuButton");
        foreach (var b in buttons)
        {
            Destroy(b);
        }
        int j = 0;
        ActionsToList();
        TextToList();
        foreach (var f in buttonFunctions)
        {
            GameObject button = Instantiate(ButtonExample, this.transform);
            Button b = button.GetComponent<Button>();
            Text t = button.GetComponentsInChildren<Text>().FirstOrDefault();
            b.onClick.AddListener(f);
            t.text = buttonText[j];
            j++;
        }
    }
}
