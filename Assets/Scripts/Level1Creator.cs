using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Level1Creator : MonoBehaviour
{
    //масив унікальних ігрових об'єктів рівня 1
    public GameObject[] UniqueObjects;
    //масив типових ігрових об'єктів рівня 1
    public GameObject[] ThesomeObjects;
    //ігровий об'єкт таймер
    public GameObject TimerObj;
    //ігровий об'єкт надпис вверху екрану
    public GameObject HeaderObj;
    //ігровий об'єкт коло, яке з'являється в кінці рівня при неправильній відповіді і вказує правильну
    public GameObject Pointer;
    //змінна, що відповідає за стан гри
    private bool isPlaying = false;
    //компонент, що відповідає за розміри і розташування ігрової панелі
    private RectTransform gamePanel;
    //компонент, який дозволяє розміщувати ігрові об'єкти сіткою
    private GridLayoutGroup grid;
    //список створених ігрових об'єктів
    private List<GameObject> Objects = new List<GameObject>();
    //посилання на унікальний ігровий об'єкт
    private GameObject Unique;
    //посилання на скрипт - мененжер рівнів
    private LevelManager lm;
    //посилання на сутність, яка відповідає за таймер під час гри
    private Coroutine TimerCoroutine = null;
    //змінна, що дорівнює номеру підрівня даного рівня
    private int part = 1;
    //компонент, що відповідає за звука програшу та виграшу
    private AudioSource SpecialAudio;
    //компонент, що відповідає за музику на фоні
    private AudioSource BackgroundAudio;

    private void Start()
    {
        //ініціалізуємо мененжер рівнів
        lm = GetComponent<LevelManager>();
        //ініціалізуємо компоненти, що відповідають за звуки
        BackgroundAudio = GameObject.FindGameObjectWithTag("BackgroundAudioObj").GetComponent<AudioSource>();
        SpecialAudio = GameObject.FindGameObjectWithTag("SpecialAudioObj").GetComponent<AudioSource>();
    }
    //функція, яка створює підрівень в залежності від параметрів
    public void CreatePart(int x_count, int y_count, int time)
    {
        Clear();
        //визначаємо посилання на унікальний і типовий ігрові об'єкти за допомогою стартового масиву
        GameObject UniqueObj = UniqueObjects[part - 1];
        GameObject ThesomeObj = ThesomeObjects[part - 1];

        //налаштовуємо заголовок вверху екрана і сітку для рошташування елементів
        Text HeaderText = HeaderObj.GetComponent<Text>();
        HeaderText.text = "Знайдіть зайвий елемент:";
        HeaderObj.SetActive(true);
        gamePanel = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<RectTransform>();
        grid = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<GridLayoutGroup>();
        //в залежності від підрівня сітка також буде набувати інших параметрів
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
        //рандомно визначаємо позицію унікального елемента
        int unique_i = (int)(Random.value * 1000 % x_count);
        int unique_j = (int)(Random.value * 1000 % y_count);

        Objects = new List<GameObject>();
        //циклами проходимо по кожній клітинці компонента GridLayoutGroup
        for (int i = 0; i < x_count; i++)
        {
            for (int j = 0; j < y_count; j++)
            {
                //створюємо унікальний ігровий об'єкт і накладаємо на нього компонент, який зчитує дотики і кліки мишкою
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
                //аналогічно створюємо типовий об'єкти і додаємо все в список
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
        //активовуємо відлік таймера
        TimerCoroutine = StartCoroutine(Timer(time));
    }
    //функція, що відповідає за відлік таймера
    public IEnumerator Timer(int time)
    {
        //показати таймер
        TimerObj.SetActive(true);
        isPlaying = true;
        Text timerText = TimerObj.GetComponent<Text>();
        //кожну секунду час зменшується
        while (time > 0)
        {
            timerText.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        //оновлюємо напис на таймері
        timerText.text = time.ToString();
        //якщо час вийшов - виводимо повідомлення і показуємо правильну відповідь
        HeaderObj.GetComponent<Text>().text = "Час вичерпано";
        Objects.Add(Instantiate(Pointer, Unique.transform));
        isPlaying = false;
        StartCoroutine(GoToStart(4f));
        yield return null;
    }
    //функція-перехід на наступний рівень чи підрівень
    //вона очікує вказану кількість секунд перш ніж очистити ігрове поле
    public IEnumerator GoNext(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpecialAudio.Stop();
        BackgroundAudio.Play();
        part++;
        GoNextPart();
        yield return null;
    }
    //дана функція мала б повертати нас в початок рівня
    //в цілях демонстрації деякий код було закоментовано
    public IEnumerator GoToStart(float seconds)
    {
        /*   yield return new WaitForSeconds(seconds);
           part = 1;
           GoNextPart();
           yield return null;*/
        yield return GoNext(seconds);
    }
    //функція виконується при правильній відповіді користувача
    public void GoodAnswer(PointerEventData data)
    {
        if (isPlaying)
        {
            if (TimerCoroutine != null)
                StopCoroutine(TimerCoroutine);
            HeaderObj.GetComponent<Text>().text = "Правильно!";
            BackgroundAudio.Pause();
            SpecialAudio.clip = lm.GoodAnswer;
            SpecialAudio.Play();
            isPlaying = false;
            StartCoroutine(GoNext(2f));
        }
    }
    //функція виконується при неправильній відповіді користувача
    public void BadAnswer(PointerEventData data)
    {
        if (isPlaying)
        {
            if (TimerCoroutine != null)
                StopCoroutine(TimerCoroutine);
            HeaderObj.GetComponent<Text>().text = "Відповідь не вірна!";
            BackgroundAudio.Pause();
            SpecialAudio.clip = lm.BadAnswer;
            SpecialAudio.Play();
            Objects.Add(Instantiate(Pointer, Unique.transform));
            isPlaying = false;
            StartCoroutine(GoToStart(2f));
        }
    }
    //функція миттєво переносить нас на наступний рівень чи підрівень
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
    //функція очищує ігрове поле від всіх об'єктів
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
