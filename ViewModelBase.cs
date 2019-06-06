using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReverseKinematic
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler TurnOnAnimationModeReverseKinematic;

        protected void TurnOnAnimation()
        {
            if (TurnOnAnimationModeReverseKinematic != null)
                TurnOnAnimationModeReverseKinematic(this,
                    new PropertyChangedEventArgs("TurnOnControlsReverseKinematic"));
        }

        public event PropertyChangedEventHandler TurnOffAnimationModeReverseKinematic;

        protected void TurnOffAnimation()
        {
            if (TurnOffAnimationModeReverseKinematic != null)
                TurnOffAnimationModeReverseKinematic(this,
                    new PropertyChangedEventArgs("TurnOffControlsReverseKinematic"));
        }
    }
}