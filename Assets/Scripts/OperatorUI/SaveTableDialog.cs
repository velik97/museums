using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveTableDialog : MonoBehaviour
{
    public InputField tableNameField;
    public Button saveButton;
    public Button closeButton;

    public GameObject saveInfoObject;
    public Text saveInfoText;
    public Button saveInfoButton;
    public Image savingImage;

    private int locationId;
    private string tableName = "";

    private string TableName
    {
        get
        {
            if (tableName == "")
            {
                if (PlayerPrefs.HasKey("TableName"))
                {
                    tableName = PlayerPrefs.GetString("TableName");
                }
                else
                {
                    PlayerPrefs.SetString("TableName", "");
                }
            }
            return tableName;
        }
        set { tableName = value; }
    }

    private string DesktopFolder
    {
        get { return "C:\\Users\\" + System.Environment.UserName + "\\Desktop"; }
    }

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        saveInfoButton.onClick.AddListener(Close);
        saveButton.onClick.AddListener(StartSaving);
    }

    public void RequestSave(int locId)
    {
        gameObject.SetActive(true);
        locationId = locId;

        tableName = GameInfo.LocationByIndex(locationId);

        if (tableName == "")
        {
            tableName = "Таблица рекордов";
        }

        TableName = tableName;

        tableNameField.text = TableName;

        saveButton.interactable = TableName != "";
        tableNameField.onValueChanged.AddListener(CheckTableName);
    }

    private void StartSaving()
    {
        saveInfoObject.SetActive(true);
        saveInfoText.gameObject.SetActive(false);
        saveInfoButton.interactable = false;

        savingImage.gameObject.SetActive(true);

        if (locationId >= 0 && locationId <= 2)
            LeaderboardParallelRequestManager.Instance.SaveScoresToXlsx(DesktopFolder, tableNameField.text, locationId,
                Savingdone);
        else
            LeaderboardParallelRequestManager.Instance.SaveScoresToXlsx(DesktopFolder, tableNameField.text,
                Savingdone);
    }

    private void Savingdone(string fileName)
    {
        saveInfoText.gameObject.SetActive(true);
        saveInfoText.text = "Таблица \"" + fileName + "\" cохранена на рабочий стол!";
        saveInfoButton.interactable = true;

        TableName = tableNameField.text;

        savingImage.gameObject.SetActive(false);
    }

    private void Close()
    {
        saveInfoObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void CheckTableName(string assumedTableName)
    {
        saveButton.interactable = assumedTableName != "";
    }
}