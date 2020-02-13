﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int stage;
    private Level1Creator lc1;
    private Level2Creator lc2;
    private Level3Creator lc3;
    public GameObject MainMenu;
    void Start()
    {
        stage = 0;
        lc1 = GetComponent<Level1Creator>();
        lc2 = GetComponent<Level2Creator>();
        lc3 = GetComponent<Level3Creator>();
    }

    public void CreateNextLevel(int selectStage = 0)
    {
        MainMenu.SetActive(false);
        if (selectStage == 0) stage++;
        else stage = selectStage;
        switch (stage)
        {
            case 1: lc1.CreatePart(10, 6, 15); break;
            case 2: lc2.CreatePart(); break;
            case 3: lc3.CreatePart(20, 9, 20); break;
            default: stage = 1; lc1.CreatePart(10, 6, 15); break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            this.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }
}