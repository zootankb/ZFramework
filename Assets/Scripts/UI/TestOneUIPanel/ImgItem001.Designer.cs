using UnityEngine.UI;

namespace ZFramework.App
{
    /// <summary>
    /// TestOneUIPanel的子对象UI设计脚本ImgItem001
    /// </summary>
    public partial class ImgItem001
    {
		public Text TxtItemChild001;

        protected override string ComponentName
        {
            get { return this.GetType().Name; }
        }
    }
}