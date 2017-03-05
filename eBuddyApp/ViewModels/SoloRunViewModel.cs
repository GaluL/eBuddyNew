using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBuddy;
using Template10.Mvvm;

namespace eBuddyApp.ViewModels
{
    class SoloRunViewModel : ViewModelBase
    {
        private RunManager _RunManager;
        public SoloRunViewModel() : base()
        {

        }

        #region Properties

        #endregion

        #region Commands
        public void StartRun() => _RunManager.Start();

        public void StopRun() => _RunManager.Stop();

        #endregion
    }
}
