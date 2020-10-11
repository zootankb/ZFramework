using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.UI;

public class TestMsgCenter : MonoBehaviour
{

    UIMsgCenter center = UIMsgCenter.Allocate(typeof(TestMsgCenter).Name);

    void Start()
    {
        center.Register(1, ProcessMsg1);
        center.Register(2, ProcessMsg2);
        center.Register(3, ProcessMsg3);
        center.Register(3, ProcessMsg4);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            center.SendMsg(1, new Msg1() { content = Time.time.ToString() });
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            center.SendMsg(2, new Msg2() { height = 6 });
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            center.SendMsg(3, new Msg2());
        }
    }

    void ProcessMsg1(int eventId, ZMsg msg)
    {
        if(eventId == 1)
        {
            Msg1 m = msg == null ? new Msg1() : msg as Msg1;
            print(m.content);
        }
    }

    void ProcessMsg2(int eventId, ZMsg msg)
    {
        if (eventId == 2)
        {
            Msg2 m = msg == null ? new Msg2() : msg as Msg2;
            print(m.height);
        }
    }

    void ProcessMsg3(int eventId, ZMsg msg)
    {
        if (eventId == 3)
        {
            Msg2 m = msg == null ? new Msg2() : msg as Msg2;
            print(m.height);
        }
    }

    void ProcessMsg4(int eventId, ZMsg msg)
    {
        if (eventId == 3)
        {
            Msg2 m = msg == null ? new Msg2() : msg as Msg2;
            print(m.height);
        }
    }

    class Msg1 : ZMsg
    {
        public string content = "567890";
    }

    class Msg2 : ZMsg
    {
        public int height = 8888;
    }
}
