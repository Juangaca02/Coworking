using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TFG.ViewModel
{
    public class CalendarViewModel
    {
        public ICommand AcceptCommand { get; set; }
        public CalendarViewModel()
        {
            AcceptCommand = new Command(ActionButtonClicked);
        }
        private void ActionButtonClicked()
        {
            // To do your requirement here.
        }
    }
}
