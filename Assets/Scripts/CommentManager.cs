using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using StackExchange.Redis;
using System.Linq;
using System.Collections.Generic;
using System;

public class CommentManager : MonoBehaviour
{
    public string EntireMsg = "";
    private string m_channel = "test";
    List<string> MsgList;
    TMP_Text textUI;
    [SerializeField] TMP_Text tmp_text;


    #region MonoBehaviour

    private void Awake()
    {
        tmp_text = GameObject.Find("Text (TMP)").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine("SubScribeMessage");
    }

    private void Update()
    {
        try
        {
            // ������ �޽����� ������
            if (MsgList.Count > 0)
            { 
                string[] messages = MsgList[0].Split(",", 2);
                MsgList.RemoveAt(0);
                // �޽����� id, comment�� �и��ؼ� �߰�
                AddComment(messages[0], messages[1]);
                StartCoroutine("Waitmillisec");
            }
        }
        catch (Exception e) { }

    }

    #endregion


    #region CommentAction
    public void AddComment(string id, string comment)
    {
        // ���� ����� �޽����� id�� comment�� �߰�
        Redis.Instance.entireMessage += id + " : " + comment + '\n';
        EntireMsg += id + " : " + comment + '\n';
        // �߰��� �޽����� ������
        tmp_text.SetText(GetEntireMessage());

        Debug.Log(GetEntireMessage());
    }


    public void SubAction(RedisChannel ch, RedisValue va)
    {
        MsgList.Add(va);
    }

    string GetEntireMessage()
    {
        return Redis.Instance.entireMessage;
    }

    IEnumerator SubScribeMessage()
    {
        Redis.Instance.Subscribe("test", SubAction);
        yield return new WaitForSeconds(0.001f);
    }

    #endregion
}