using UnityEngine.UI;

namespace ZFramework.App
{
    /// <summary>
    /// TestOneUIPanel的子对象UI设计脚本ImgItemElementPanel
    /// </summary>
    public partial class ImgItemElementPanel
    {
        protected override string ComponentName
        {
            get { return this.GetType().Name; }
        }
    }
}