using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework.UI;

namespace ZFramework.TestApp
{
    /// <summary>
    /// UI数据
    /// </summary>
    public class TestUIPanelDatta : UIPanelData
    {

    }

    /// <summary>
    /// 自定义UI面板操作
    /// </summary>
    public partial class TestUIPanel : UIPanel
    {
        protected override void OnInit(IUIData uiData)
        {
            mData = uiData as TestUIPanelDatta ?? new TestUIPanelDatta();

            if (btnOne == null)
            {
                print("属性btnOne存在");
            }
            if(mData == null)
            {
                print("属性mData存在");
            }
            if(mUiData == null)
            {
                print("属性mUiData存在");
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
        }
    }
}