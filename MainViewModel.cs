namespace ReverseKinematic
{
    public class MainViewModel : ViewModelBase
    {
        internal void Render()
        {
        }

        #region Private fields

        private string _text;
        private Scene _scene;

        #endregion Private fields

        #region Public Properties

        public MainViewModel()
        {
            Scene = new Scene();
        }

        public Scene Scene
        {
            get => _scene;
            set
            {
                _scene = value;
                OnPropertyChanged();
            }
        }


        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        #endregion Public Properties

        #region Private Methods

        #endregion Private Methods
    }
}