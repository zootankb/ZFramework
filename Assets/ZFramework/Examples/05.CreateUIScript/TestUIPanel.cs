using ZFramework.UI;

namespace ZFramework.TestApp
{
    /// <summary>
    /// UI的数据传输结构
    /// </summary>
    public class TestUIPanelDatta : UIPanelData
    {
        // TODO
    }

    /// <summary>
    /// 自定义UI面板操作
    /// </summary>
    public partial class TestUIPanel : UIPanel
    {
        protected override void SendMsg(int eventId, ZMsg msg)
        {
            base.SendMsg(eventId, msg);
        }

        protected override void OnInit(IUIData uiData)
        {
            mData = uiData as TestUIPanelDatta ?? new TestUIPanelDatta();
            // TODO
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