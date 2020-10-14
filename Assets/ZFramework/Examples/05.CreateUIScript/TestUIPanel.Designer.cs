using UnityEngine.UI;

namespace ZFramework.TestApp
{
    /// <summary>
    /// 自定义UI面板属性与数据存储
    /// </summary>
    public partial class TestUIPanel
    {
        /// <summary>
        /// 此脚本外不用
        /// </summary>
        private TestUIPanelDatta mPrivateData = null;

        /// <summary>
        /// 传进来的UI数据
        /// </summary>
        private TestUIPanelDatta mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new TestUIPanelDatta());
            }
            set
            {
                mPrivateData = value;
                mUiData = value;
            }
        }
    }
}