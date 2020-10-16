using ZFramework.UI;

namespace ZFramework.App
{
    /// <summary>
    /// TestOneUIPanel的数据传输结构
    /// </summary>
    public class TestOneUIPanelData : UIPanelData
    {
        // TODO
    }

    /// <summary>
    /// 自定义TestOneUIPanel面板操作
    /// </summary>
    public partial class TestOneUIPanel : UIPanel
    {
        protected override void SendMsg(int eventId, ZMsg msg)
        {
            base.SendMsg(eventId, msg);
        }

        protected override void OnInit(IUIData uiData)
        {
            mData = uiData as TestOneUIPanelData ?? new TestOneUIPanelData();
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