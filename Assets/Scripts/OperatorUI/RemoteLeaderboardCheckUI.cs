using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoteLeaderboardCheckUI : MonoBehaviour
{
    public InputField computerIdField;
    public InputField remotePcNameField;

    public Button saveButton;

    public MatchImages matchImages;
    public Text descriptionText;

    private string[] descriptions = new []
    {
        "Не видно общей папки на другом компьютере. Проверьте следующие пункты:\n" +
            "1) Второй компьютер может быть выключен\n" +
            "2) Указанное имя может не соответствовать имени другого компьютера\n" +
            "3) На другом компьютере может отсутствовать папка \"net folder\" с общим доступом\n" +
            "4) Доступ в папку \"net folder\" на другом компьютере закрыт",
        
        "Не видно таблицы рекордов на другом компьютере\n" +
            "Запустите игру на другом компьютере, таблица создасться автоматически",
        
        "Не получается получить доступ к таблице рекордов на другом компьютере\n" +
            "НА ДРУГОМ КОМПЬЮТЕРЕ удалите файл \"leaderboard.xml\" в папке \"net folder\"\n" +
            "Данное действие удалит все результаты на другом компьютере!",
        
        "Возможно, таблица рекордов на другом компьютере была испорчена\n" +
            "НА ДРУГОМ КОМПЬЮТЕРЕ удалите файл \"leaderboard.xml\" в папке \"net folder\"\n" +
            "Данное действие удалит все результаты на другом компьютере!",
        
        "Указанные номера компьютеров не должны совпадать\n" +
            "Укажите на этом компьютере 1, а на другом, например, 2 или наоборот",
        
        "Не все поля заполнены"
    };

    public float waitBeforeCheckTime = .4f;

    private int computerId;
    private string lastComputerIdString;
    private int initialComputerId = -1;

    private void Awake()
    {
        int.TryParse(computerIdField.text, out initialComputerId);
        lastComputerIdString = initialComputerId.ToString();
        computerId = initialComputerId;

        saveButton.interactable = false;
    }

    public void Open()
    {
        SetLoading();
        
        remotePcNameField.interactable = false;
        computerIdField.interactable = false;
        
        remotePcNameField.text = "";
        computerIdField.text = "";        
        
        LeaderboardParallelRequestManager.Instance.GetRemotePcName(PresetRemotePcName);
        LeaderboardParallelRequestManager.Instance.GetComputerId(PresetComputerId);
        
        descriptionText.text = "";
        gameObject.SetActive(true);
    }

    public void Close()
    {
        StopAllCoroutines();
        
        remotePcNameField.interactable = false;
        computerIdField.interactable = false;
        
        remotePcNameField.onValueChanged.RemoveAllListeners();
        computerIdField.onValueChanged.RemoveAllListeners();

        descriptionText.text = "";
        gameObject.SetActive(false);
    }

    public void Save()
    {
        LeaderboardParallelRequestManager.Instance.SetComputerId(computerId);
        LeaderboardParallelRequestManager.Instance.SetRemotePcName(remotePcNameField.text);
    }

    private void PresetRemotePcName(string pcName)
    {
        remotePcNameField.text = pcName;
        
        remotePcNameField.onValueChanged.AddListener(CheckPcName);
        remotePcNameField.interactable = true;

        SetEmpty();
    }

    private void PresetComputerId(int id)
    {                
        initialComputerId = id;

        computerIdField.text = id.ToString();
        lastComputerIdString = initialComputerId.ToString();   
        
        computerIdField.onValueChanged.AddListener(CheckComputerId);
        computerIdField.interactable = true;
        
        SetEmpty();
    }

    private void CheckPcName(string pcName)
    {
        StopAllCoroutines();
        StartCoroutine(WaitForCheck());
    }

    private void CheckComputerId(string computerIdString)
    {
        if (!int.TryParse(computerIdString, out computerId))
        {
            computerId = int.Parse(lastComputerIdString);
            if (computerIdString != "")
                computerIdField.text = lastComputerIdString;
        }

        lastComputerIdString = computerId.ToString();        

        StopAllCoroutines();
        StartCoroutine(WaitForCheck());
    }

    private IEnumerator WaitForCheck()
    {        
        if (remotePcNameField.text == "" || computerIdField.text == "")
        {
            descriptionText.text = descriptions[5];
            SetEmpty();
            yield break;
        }
        SetLoading();
        descriptionText.text = "";
        yield return new WaitForSeconds(waitBeforeCheckTime);

        LeaderboardParallelRequestManager.Instance.GetRemoteLeaderboardAccessCode(remotePcNameField.text,
            computerId, ShowAccess);
    }

    private void ShowAccess(RemoteLeaderboardAccess access)
    {
        switch (access)
        {
            case RemoteLeaderboardAccess.Accessible:
                SetMatch();
                break;
            case RemoteLeaderboardAccess.NetFolderDoesntExistOrNotAccessible:
                SetNotMatch();
                descriptionText.text = descriptions[0];
                break;
            case RemoteLeaderboardAccess.XmlDoesntExist:
                SetNotMatch();
                descriptionText.text = descriptions[1];
                break;
            case RemoteLeaderboardAccess.XmlIsNotAccessible:
                SetNotMatch();
                descriptionText.text = descriptions[2];
                break;
            case RemoteLeaderboardAccess.BadRemoteXml:
                SetNotMatch();
                descriptionText.text = descriptions[3];
                break;
            case RemoteLeaderboardAccess.ComputerIdsAreSame:
                SetNotMatch();
                descriptionText.text = descriptions[4];
                break;
        }

        if (access != RemoteLeaderboardAccess.Accessible &&
            access != RemoteLeaderboardAccess.ComputerIdsAreSame && computerId != initialComputerId)
        {
            SetEmpty();
            saveButton.interactable = true;
        }
    }

    private void SetLoading()
    {
        matchImages.SetLoading();

        saveButton.interactable = false;
    }

    private void SetMatch()
    {
        matchImages.SetMatch();

        saveButton.interactable = true;
    }

    private void SetNotMatch()
    {
        matchImages.SetNotMatch();

        saveButton.interactable = false;
    }

    private void SetEmpty()
    {
        matchImages.SetEmpty();

        saveButton.interactable = false;
    }
}