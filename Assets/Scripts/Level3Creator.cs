using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Level3Creator : MonoBehaviour
{
    public Sprite[] ThesomeObjects;
    public Sprite[] UniqueObjects; 
    public GameObject prefabThesome;
    public GameObject prefabUnique;
    public GameObject TimerObj;
    public GameObject HeaderObj;
    public GameObject Pointer;
    private bool isPlaying = false;
    private RectTransform gamePanel;
    private GridLayoutGroup grid;
    private List<GameObject> Objects = new List<GameObject>();
    private GameObject Unique;
    private LevelManager lm;
    private Coroutine TimerCoroutine = null;
    private int part = 1;
    private AudioSource SpecialAudio;
    private AudioSource BackgroundAudio;
    private void Start()
    {
        lm = GetComponent<LevelManager>();
        BackgroundAudio = GameObject.FindGameObjectWithTag("BackgroundAudioObj").GetComponent<AudioSource>();
        SpecialAudio = GameObject.FindGameObjectWithTag("SpecialAudioObj").GetComponent<AudioSource>();
    }
    public void CreatePart(int x_count, int y_count, int time)
    {
        Clear();
        Debug.Log(part);
        isPlaying = false;
        prefabThesome.GetComponent<Image>().sprite = ThesomeObjects[part - 1];
        prefabUnique.GetComponent<Image>().sprite = UniqueObjects[part - 1];
        GameObject UniqueObj = prefabUnique; 
        GameObject ThesomeObj = prefabThesome;

        Text HeaderText = HeaderObj.GetComponent<Text>();
        HeaderText.text = "Знайдіть зайвий елемент:";
        HeaderObj.SetActive(true);
        gamePanel = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<RectTransform>();
        grid = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<GridLayoutGroup>();
        grid.constraintCount = x_count;

        grid.cellSize = new Vector2((gamePanel.sizeDelta.x / x_count) - (gamePanel.sizeDelta.x / x_count) * 0.1f, (gamePanel.sizeDelta.x / x_count) - (gamePanel.sizeDelta.x / x_count) * 0.1f);
        grid.spacing = new Vector2(gamePanel.sizeDelta.x / x_count * 0.1f, gamePanel.sizeDelta.y / y_count - grid.cellSize.y);

        int unique_i = (int)(Random.value * 1000 % x_count);
        int unique_j = (int)(Random.value * 1000 % y_count);

        Objects = new List<GameObject>();
        for (int i = 0; i < x_count; i++)
        {
            for (int j = 0; j < y_count; j++)
            {
                if ((i == unique_i) && (j == unique_j))
                {

                    Unique = Instantiate(UniqueObj, gamePanel.transform);
                    Unique.GetComponent<ButtonClickLvl3>().l3c = this;
                    Objects.Add(Unique);
                }
                else
                {
                    var objToInstantiate = Instantiate(ThesomeObj, gamePanel.transform);
                    objToInstantiate.GetComponent<ButtonClickLvl3>().l3c = this;
                    Objects.Add(objToInstantiate);
                }
            }
        }
        TimerCoroutine = StartCoroutine(Timer(time));
    }
    public IEnumerator Timer(int time)
    {
        TimerObj.SetActive(true);
        isPlaying = true;
        Text timerText = TimerObj.GetComponent<Text>();
        while (time > 0)
        {
            timerText.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        timerText.text = time.ToString();
        HeaderObj.GetComponent<Text>().text = "Час вичерпано";
        Objects.Add(Instantiate(Pointer, Unique.transform));
        isPlaying = false;
        StartCoroutine(GoToStart(4f));
        yield return null;
    }
    public IEnumerator GoNext(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpecialAudio.Stop();
        BackgroundAudio.Play();
        part++;
        GoNextPart();
        yield return null;
    }
    public IEnumerator GoToStart(float seconds)
    {
        /*   yield return new WaitForSeconds(seconds);
           part = 1;
           GoNextPart();
           yield return null;*/
        yield return GoNext(seconds);
    }
    public void GoNextPart()
    {
        Clear();
        if(part > 5)
        {
            part = 1;
            Clear();
            lm.CreateNextLevel();
        }
        else switch (part)
        {
            case 1: CreatePart(20, 9, 20); break;
            case 2: CreatePart(19, 9, 20); break;
            case 3: CreatePart(15, 7, 20); break;
            case 4: CreatePart(18, 8, 20); break;
            case 5: CreatePart(18, 7, 20); break;
            default: part = 1; CreatePart(20, 9, 20); break;
        }
    }

    public void SelectObject(GameObject sender)
    {
        if (TimerCoroutine != null) StopCoroutine(TimerCoroutine);
        if(isPlaying)
        {
            if (sender.GetComponent<Image>().sprite == UniqueObjects[part - 1])
            {
                HeaderObj.GetComponent<Text>().text = "Правильно!";
                BackgroundAudio.Pause();
                SpecialAudio.clip = lm.GoodAnswer;
                SpecialAudio.Play();
                isPlaying = false;
                StartCoroutine(GoNext(4f));
            }
            else
            {
                HeaderObj.GetComponent<Text>().text = "Відповідь не вірна!";
                BackgroundAudio.Pause();
                SpecialAudio.clip = lm.BadAnswer;
                SpecialAudio.Play();
                isPlaying = false;
                foreach (var obj in Objects)
                {
                    if (obj.GetComponent<Image>().sprite == UniqueObjects[part - 1])
                    {
                        Instantiate(Pointer, obj.transform);
                    }
                }
                StartCoroutine(GoToStart(4f));
            }
        }
    }
    public void Clear()
    {
        if (TimerCoroutine != null)
            StopCoroutine(TimerCoroutine);
        TimerObj.SetActive(false);
        HeaderObj.SetActive(false);
        foreach (var obj in Objects)
        {
            Destroy(obj);
        }
        Objects = new List<GameObject>();
    }
}
