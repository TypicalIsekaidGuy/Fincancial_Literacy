    using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System;
using TMPro;
using System.Data;
using System.Globalization;
using UnityEngine.UI;
using Newtonsoft.Json.Bson;

public class GameManager : MonoBehaviour
{
    public UIManager UIManager;

    /*Переменные, которые являются ориентиром для игрока*/
    public float saturation = 100f;
    public float happiness = 100f;
    public float perfomance = 0f;
    public float financial_literacy = 0f;

    /*Переменные, которые являются опорой для использования предыдущих переменных*/
    public float money;
    public DateTime dateTime = new DateTime(2017, 9, 1, 6, 0, 0);
    private States state = States.BOMJ;

    /*Временные переменные, которые являются вспомогательным модулем для предыдущих переменных*/
    public enum States { BOMJ, CAROWNER, HOUSEOWNER, MILLONER };
    private DateTime tmpDate = new DateTime(2017, 9, 1, 23, 0, 0);
    private float saturationMutliplier = 2f;
    private float happinessMutliplier = 1f;
    private bool isPerfomanceInDanger = false;

    /*UI*/
    /*Текстовые поля*/
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI moneyText;

    /*Слайдеры*/
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private Slider hapinessSlider;
    [SerializeField] private Slider perfomanceSlider;
    [SerializeField] private Slider financial_literacySlider;

    /*Панель проигрыша и вариации 2д сцен*/
    [SerializeField] private GameObject losePanel;
    public GameObject[] Scenes;

    /*Загрузка либо создание сохранений*/
    private void Start()
    {
        string saveFile = Application.persistentDataPath + "/saveFile.json";

        if (!File.Exists(saveFile))// Проверка на несуществования файла
        {
            File.Create(saveFile).Dispose();// Создание файла, если его нет
            SaveData();
        }
        else
        {
            LoadData();
            StateChanger();
        }
        /*Вывод на UI состояния */
        SetDate();
        SetTime();
        SetMoney();
        SetSaturation();
        SetHapiness();
        SetPerfomance();
        SetFinancial_literacy();
        Debug.Log(saturation);
    }
    #region UIUpdate
    /* регион для изменения UI, который отоброжает актульное состояние переменной */
    private void SetDate()
    {
        dateText.text = dateTime.Day.ToString() + "." + dateTime.Month.ToString() + "." + dateTime.Year.ToString();
    }
    private void SetTime()
    {
        timeText.text = dateTime.Hour.ToString() + ":" + dateTime.Minute.ToString();
    }
    private void SetMoney()
    {
        moneyText.text = money.ToString() + " Р";
    }
    private void SetSaturation()
    {
        saturationSlider.value = saturation;
    }
    private void SetHapiness()
    {
        hapinessSlider.value = happiness;
    }
    private void SetPerfomance()
    {
        perfomanceSlider.value = perfomance;
    }
    private void SetFinancial_literacy()
    {
        financial_literacySlider.value = financial_literacy;
    }
    #endregion
    #region ChangeVar
    /* регион для изменения каждой переменной */
    public void ChangeDateTime(float i)
    {
        Debug.Log(dateTime);
        Debug.Log(tmpDate);
        dateTime = dateTime.AddHours((double)i);
        ChangeSaturation(-i * saturationMutliplier);
        ChangeHappiness(-i * happinessMutliplier);
        if (dateTime.CompareTo(tmpDate) > 0 || dateTime.CompareTo(tmpDate) == 0)
        {
            double hours = (dateTime - tmpDate).TotalHours;
            tmpDate = tmpDate.AddDays(1);
            Debug.Log(tmpDate + "temp");
            i = (float)(7 + hours);
            dateTime = dateTime.AddHours((double)i);
            if (dateTime.Day == 2 && (dateTime.Month != 1 || dateTime.Month != 6 || dateTime.Month != 7 || dateTime.Month != 8 || dateTime.Month != 9))
            {
                PerfomanceCheck();
            }
        }
        SaturationCheck();
        HappinessCheck();
        SaveData();
        SetDate();
        SetTime();
        SetSaturation();
        SetHapiness();
        SetPerfomance();
    }
    public void ChangeSaturation(float i)
    {
        saturation += i;
        saturation = CheckLimit(saturation);
    }
    public void ChangeHappiness(float i)
    {
        happiness += i;
        happiness = CheckLimit(happiness);
    }
    public void ChangePerfomance(float i)
    {
        perfomance += i;
        perfomance = CheckLimit(perfomance);
    }
    public void ChangeFinancialLiteracy(float i)
    {
        financial_literacy += i;
        financial_literacy = CheckLimit(financial_literacy);
    }
    public void ChangeMoney(float i)
    {
        money += i;
        SetMoney();
        StateChanger();
    }
    private float CheckLimit(float var)
    {
        if (var > 100)
            return 100;
        return var;
    }
    private void StateChanger()
    {
        switch (state) {
            case States.BOMJ:
                if (money < 30000 && money > 20000)
                    state = States.CAROWNER;
                break;
            case  States.CAROWNER:
                if (money > 25000 && money < 70000)
                    state = States.HOUSEOWNER;
                else
                    state = States.BOMJ;
                break;
            case States.HOUSEOWNER:
                if (money > 60000 && money<140000)
                    state = States.MILLONER;
                else if (money < 60000)
                    state = States.CAROWNER;
                break;
        }
        for (int i = 0; i < Scenes.Length; i++)
        {
            if (i == (int)state)
                Scenes[i].SetActive(true);
            else
                Scenes[i].SetActive(false);
        }
    }
    private void PerfomanceCheck() {

        switch (perfomance)
        {
            case < 39:
                if (isPerfomanceInDanger)
                    GameOver();
                else
                    isPerfomanceInDanger = true;
            break;
            case > 40 and < 59:
                    ChangeMoney(1000);
                break;
            case > 60 and < 79:
                    ChangeMoney(3000);
                break;
            case > 80:
                    ChangeMoney(10000);
                break;
        }
    }
    private void SaturationCheck() {

        Debug.Log(saturation);
        if (saturation <= 0)
        {
            GameOver();
        }
    }
    private void HappinessCheck() {

        Debug.Log(happiness);
        if (happiness <= 0)
        {
                GameOver();
        }
    }
    #endregion
    private void GameOver()
    {
        losePanel.SetActive(true);
    }
    public void Restart()
    {
        Wrapper wrapper = new Wrapper(new DateTime(2017, 9, 1, 6, 0, 0).ToString(), new DateTime(2017, 9, 1, 23, 0, 0).ToString(), 100, 100, 0, 0, 0, States.BOMJ, false);
        string json = JsonUtility.ToJson(wrapper);
        string saveFile = Application.persistentDataPath + "/saveFile.json";
        File.WriteAllText(saveFile, json);
        LoadData();
        SetDate();
        SetTime();
        SetMoney();
        SetSaturation();
        SetHapiness();
        SetPerfomance();
        SetFinancial_literacy();
        losePanel.SetActive(false);
    }
    #region Serialization
    /*регион для сохранений*/
    public void SaveData() //метод, который реализует класс, в котором записываются все переменные в этом скрипте, сохраняются в json и записываются в файл
    {
        Wrapper wrapper = new Wrapper(dateTime.ToString(), tmpDate.ToString(), saturation, happiness, perfomance,financial_literacy,money,state, isPerfomanceInDanger);
        string json = JsonUtility.ToJson(wrapper);
        string saveFile = Application.persistentDataPath + "/saveFile.json";
        File.WriteAllText(saveFile,json);
    }
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/saveFile.json";
        StreamReader reader = new StreamReader(path);
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(reader.ReadToEnd());
        dateTime = DateTime.Parse(wrapper.dateTime);
        tmpDate = DateTime.Parse(wrapper.tmpDate);
        saturation = wrapper.saturation;
        happiness = wrapper.happiness;
        perfomance = wrapper.perfomance;
        financial_literacy = wrapper.financial_literacy;
        money = wrapper.money;
        state = wrapper.state;
        isPerfomanceInDanger = wrapper.isPerfomanceInDanger;
    }
    public class Wrapper
    {
        public Wrapper(
        string dateTime,
        string tmpDate,
        float saturation,
        float happiness,
        float perfomance,
        float financial_literacy,
        float money,
        GameManager.States state,
        bool isPerfomanceInDanger
        )
        {
            this.dateTime = dateTime;
            this.tmpDate = tmpDate;
            this.saturation = saturation;
            this.happiness = happiness;
            this.perfomance = perfomance;
            this.financial_literacy = financial_literacy;
            this.money = money;
            this.state = state;
            this.isPerfomanceInDanger = isPerfomanceInDanger;
        }
        public string dateTime;
        public string tmpDate;
        public float saturation;
        public float happiness;
        public float perfomance;
        public float financial_literacy;
        public float money;
        public GameManager.States state;
        public bool isPerfomanceInDanger;
    }
    #endregion
}
