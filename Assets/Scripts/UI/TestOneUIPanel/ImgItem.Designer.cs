using UnityEngine.UI;

namespace ZFramework.App
{
    /// <summary>
    /// TestOneUIPanel的子对象UI设计脚本ImgItem
    /// </summary>
    public partial class ImgItem
    {
		public Text TxtItemChild;

        protected override string ComponentName
        {
            get { return this.GetType().Name; }
        }
    }
}