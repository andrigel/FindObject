using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Level1Creator : MonoBehaviour
{
    public GameObject[] UniqueObjects;
    public GameObject[] ThesomeObjects;
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
    private void Start()
    {
        lm = GetComponent<LevelManager>();
    }
    public void CreatePart(int x_count, int y_count, int time)
    {
        Clear();
        GameObject UniqueObj = UniqueObjects[part - 1];
        GameObject ThesomeObj = ThesomeObjects[part - 1];

        Text HeaderText = HeaderObj.GetComponent<Text>();
        HeaderText.text = "Знайдіть зайвий елемент:";
        HeaderObj.SetActive(true);
        gamePanel = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<RectTransform>();
        grid = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<GridLayoutGroup>();
        grid.constraintCount = x_count;
        switch (x_count)
        {
            case 10:
                grid.cellSize = new Vector2(80, 80);
                grid.spacing = new Vector2(15, 15);
                break;
            case 11:
                grid.cellSize = new Vector2(80, 80);
                grid.spacing = new Vector2(10, 10);
                break;
            case 12:
                grid.cellSize = new Vector2(73, 73);
                grid.spacing = new Vector2(10, 15);
                break;
            default:
                grid.cellSize = new Vector2(80, 80);
                grid.spacing = new Vector2(10, 10);
                break;
        }
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
                    EventTrigger trigger = Unique.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => { GoodAnswer((PointerEventData)data); });
                    trigger.triggers.Add(entry);
                    Objects.Add(Unique);
                }
                else
                {
                    GameObject Thesome = Instantiate(ThesomeObj, gamePanel.transform);
                    EventTrigger trigger = Thesome.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => { BadAnswer((PointerEventData)data); });
                    trigger.triggers.Add(entry);
                    Objects.Add(Thesome);
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
    public void GoodAnswer(PointerEventData data)
    {
        if (isPlaying)
        {
            if (TimerCoroutine != null)
                StopCoroutine(TimerCoroutine);
            HeaderObj.GetComponent<Text>().text = "Правильно!";
            isPlaying = false;
            StartCoroutine(GoNext(2f));
        }
    }
    public void BadAnswer(PointerEventData data)
    {
        if (isPlaying)
        {
            if (TimerCoroutine != null)
                StopCoroutine(TimerCoroutine);
            HeaderObj.GetComponent<Text>().text = "Відповідь не вірна!";
            Objects.Add(Instantiate(Pointer, Unique.transform));
            isPlaying = false;
            StartCoroutine(GoToStart(2f));
        }
    }
    public void GoNextPart()
    {
        if (part > 3)
        {
            part = 1;
            Clear();
            lm.CreateNextLevel();
        }
        else
            CreatePart(9 + part, 6, 20 - 5 * part);
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
        Objects.Clear();
    }
}
