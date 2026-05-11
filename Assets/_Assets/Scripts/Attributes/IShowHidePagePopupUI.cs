using UnityEngine;

namespace WatKhaoWong.Attributes
{
    public interface IShowHidePagePopupUI
    {
        public void OpenPage(float delayBeforePlay);

        public void OpenPageOpposite(float delayBeforePlay);

        public void ClosePage(float delayBeforePlay);

        public void ClosePageOpposite(float delayBeforePlay);
    }
}