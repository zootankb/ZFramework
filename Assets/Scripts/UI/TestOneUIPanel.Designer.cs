using UnityEngine.UI;
using UnityEngine;

namespace ZFramework.App
{
    /// <summary>
    /// 自定义UI面板属性与数据存储
    /// </summary>
    public partial class TestOneUIPanel
    {
        /// <summary>
        /// 此脚本外不用
        /// </summary>
        private TestOneUIPanelData mPrivateData = null;

        /// <summary>
        /// 传进来的UI数据
        /// </summary>
        private TestOneUIPanelData mData
        {
            get
            {
                return mPrivateData ?? (mPrivateData = new TestOneUIPanelData());
            }
            set
            {
                mPrivateData = value;
                mUiData = value;
            }
        }

		public Image Image;
		public GameObject TestGo;
		public ImgItemElementPanel ImgItem;
    }
}