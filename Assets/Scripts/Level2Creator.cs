using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Level2Creator : MonoBehaviour
{
    //спрайти 1 підрівня
    public Sprite[] part1;
    //спрайти 2 підрівня
    public Sprite[] part2;
    //спрайти 3 підрівня
    public Sprite[] part3;
    public GameObject TimerObj;
    public GameObject HeaderObj;
    public GameObject Pointer;
    //по суті порожній ігровий об'єкт, який ми налаштуємо в процесі.На цей об'єкт було накладено скрипт ButtonClickLvl2, який відслідковує натискання
    public GameObject prefab;
    //ігровий об'єкт, що відповідає за ввесь ГШ в цілому
    public GameObject canvas;
    //квадрат, яким маркується вибраний користувачем ігровий об'єкт
    public GameObject Square;
    private bool isPlaying = false;
    private RectTransform gamePanel;
    private GridLayoutGroup grid;
    private LevelManager lm;
    private Coroutine TimerCoroutine = null;
    private List<GameObject> Objects = new List<GameObject>();
    //в цьому масиві буде здійснено перемішування картинок випадковим чином
    private List<Sprite> RandomizedSprites = new List<Sprite>();
    //картинка спеціального ігрового об'єкта
    private Sprite special;
    private AudioSource SpecialAudio;
    private AudioSource BackgroundAudio;
    [HideInInspector]
    //вибрані користувачем ігрові об'єкти
    public List<GameObject> SelectedObjects = new List<GameObject>();
    //деякі необхідні змінні для створення рівня
    int part = 1;
    int time = 20;
    int x_count = 9;
    void Start()
    {
        lm = GetComponent<LevelManager>();
        //ініціалізуємо список вибраних користувачем ігрових об'єктів.За замовчуванням він порожній
        SelectedObjects = new List<GameObject>();
        BackgroundAudio = GameObject.FindGameObjectWithTag("BackgroundAudioObj").GetComponent<AudioSource>();
        SpecialAudio = GameObject.FindGameObjectWithTag("SpecialAudioObj").GetComponent<AudioSource>();
    }
    //функція для створення підрівня
    public void CreatePart()
    {
        RandomizedSprites = new List<Sprite>();
        Text HeaderText = HeaderObj.GetComponent<Text>();
        HeaderText.text = "Знайдіть 2 одинакових елементи:";
        HeaderObj.SetActive(true);
        TimerObj.SetActive(true);
        gamePanel = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<RectTransform>();
        grid = GameObject.FindGameObjectWithTag("GamePanel").GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(80, 80);
        grid.spacing = new Vector2(40, 40);
        grid.constraintCount = x_count;
        //визначаємо об'єкт, який буде спеціальним і додаємо його в список
        switch (part)
        {
            case 1: RandomizedSprites.Add(part1[Random.Range(0, part1.Length)]); break;
            case 2: RandomizedSprites.Add(part2[Random.Range(0, part2.Length)]); break;
            case 3: RandomizedSprites.Add(part3[Random.Range(0, part3.Length)]); break;
            default: part = 1; RandomizedSprites.Add(part1[Random.Range(0, part1.Length)]); break;
        }
        //зберігаємо посилання на цей об'єкт
        special = RandomizedSprites[0];
        //додаємо всі інші елементи в цей список
        switch (part)
        {
            case 1: RandomizedSprites.AddRange(part1); break;
            case 2: RandomizedSprites.AddRange(part2); break;
            case 3: RandomizedSprites.AddRange(part3); break;
            default: part = 1; RandomizedSprites.AddRange(part1); break;
        }  
        //перемішуємо цей список випадковим чином за спеціальним алогоритмом
        for (int i = RandomizedSprites.Count - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = RandomizedSprites[j];
            RandomizedSprites[j] = RandomizedSprites[i];
            RandomizedSprites[i] = temp;
        }
        //створюємо ігрові об'єкти - зображення з нашими спрайтами
        foreach (var sprite in RandomizedSprites)
        {
            var obj = Instantiate(prefab, gamePanel);
            obj.GetComponent<ButtonClickLvl2>().l2c = this;
            obj.GetComponent<Image>().sprite = sprite;
            Objects.Add(obj);
        }
        //запускаємо таймер
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
        foreach(var obj in Objects)
        {
            if(obj.GetComponent<Image>().sprite == special)
            {
                Instantiate(Pointer, obj.transform);
            }
        }
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

    public void GoNextPart()
    {
        if (part > 3)
        {
            part = 1;
            Clear();
            lm.CreateNextLevel();
        }
        else
        {
            Clear();
            CreatePart();
        }
    }
    public IEnumerator GoToStart(float seconds)
    {
        /*   yield return new WaitForSeconds(seconds);
           part = 1;
           GoNextPart();
           yield return null;*/
        yield return GoNext(seconds);
    }
    //функція, що відповідає за натискання по картинках
    //вона викликається самими об'єкатами, на які здійснюється натискання
    //в параметр sender передається посилання на об'єкт, який викликав цю функцію
    public void SelectObject(GameObject sender)
    {
        if (isPlaying)
        {
            //якщо цей об'єкт було вибрано раніше, видаляємо його зі списку
            if (SelectedObjects.Contains(sender))
            {
                SelectedObjects.Remove(sender);
            }
            //якщо ж вибрано не було, додаємо об'єкт в список
            else
            {
                sender.GetComponent<ButtonClickLvl2>().SetSquareGreen();
                SelectedObjects.Add(sender);
            }
            //якщо вибрано 2 об'єкти, здійсніємо порівняння їх картинок і визначаємо чи відповідь правильна
            if (SelectedObjects.Count == 2)
            {
                if (TimerCoroutine != null) StopCoroutine(TimerCoroutine);
                if (SelectedObjects[0].GetComponent<Image>().sprite == SelectedObjects[1].GetComponent<Image>().sprite)
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
                    foreach (var obj in SelectedObjects)
                    {
                        obj.GetComponent<ButtonClickLvl2>().SetSquareRed();
                    }
                    // при невірній відповіді знаходимо правильний варіант і показуємо користувачу
                    foreach (var obj in Objects)
                    {
                        if (obj.GetComponent<Image>().sprite == special)
                        {
                            Instantiate(Pointer, obj.transform);
                        }
                    }
                    StartCoroutine(GoToStart(4f));
                }
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
        Objects.Clear();
        SelectedObjects = new List<GameObject>();
    }
}