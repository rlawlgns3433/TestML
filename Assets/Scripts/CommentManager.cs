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
            // 수신한 메시지가 있으면
            if (MsgList.Count > 0)
            { 
                string[] messages = MsgList[0].Split(",", 2);
                MsgList.RemoveAt(0);
                // 메시지를 id, comment로 분리해서 추가
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
        // 기존 저장된 메시지에 id와 comment를 추가
        Redis.Instance.entireMessage += id + " : " + comment + '\n';
        EntireMsg += id + " : " + comment + '\n';
        // 추가된 메시지를 보여줌
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