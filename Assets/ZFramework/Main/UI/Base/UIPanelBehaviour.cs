using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.UI
{
    /// <summary>
    /// UIPanel与monobahaviour的中间层
    /// </summary>
    public class UIPanelBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 传递给ui的数据
        /// </summary>
        public UIPanelData uidata = null;

        /// <summary>
        /// 初始化UI数据
        /// </summary>
        /// <param name="uidata"></param>
        protected void InitUIData(UIPanelData uidata)
        {
            this.uidata = uidata;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="evenId"></param>
        /// <param name="msg"></param>
        protected virtual void SendMsg(int evenId, UIMsg msg)
        {

        }

        /// <summary>
        /// 消息传输
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="msg"></param>
        protected virtual void ProcessMsg(int eventId, UIMsg msg)
        {
            // TODO
        }

        protected virtual void OnInit(IUIData uIData)
        {

        }

        protected virtual void OnOpen(IUIData uIData)
        {

        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnHide()
        {

        }
        protected virtual void OnClose()
        {

        }
    }
}