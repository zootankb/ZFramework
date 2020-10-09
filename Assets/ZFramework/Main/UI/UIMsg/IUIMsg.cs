using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UI消息接口
    /// </summary>
    public interface IUIMsg
    {
       void SendMsg(int eventId, ZMsg msg);
    }
}