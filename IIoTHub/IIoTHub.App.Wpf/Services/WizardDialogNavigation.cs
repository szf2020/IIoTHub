namespace IIoTHub.App.Wpf.Services
{
    public class WizardDialogNavigation
    {
        private int _currentPageIndex;
        private readonly List<Func<object>> _pageViewFactories;

        public WizardDialogNavigation(List<Func<object>> pageViewFactories)
        {
            _pageViewFactories = pageViewFactories;
        }

        /// <summary>
        /// 目前顯示的頁面
        /// </summary>
        public object CurrentPageView => _pageViewFactories[_currentPageIndex]();

        /// <summary>
        /// 是否可以往下一頁
        /// </summary>
        public bool CanMoveNext => _currentPageIndex < _pageViewFactories.Count - 1;

        /// <summary>
        /// 是否可以往上一頁
        /// </summary>
        public bool CanMovePrevious => _currentPageIndex > 0;

        /// <summary>
        /// 移動到下一頁
        /// </summary>
        public void MoveNext()
        {
            if (CanMoveNext)
            {
                _currentPageIndex++;
            }
        }

        /// <summary>
        /// 移動到上一頁
        /// </summary>
        public void MovePrevious()
        {
            if (CanMovePrevious)
            {
                _currentPageIndex--;
            }
        }
    }
}
