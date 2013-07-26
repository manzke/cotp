using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devsurf.Security.Otp;
using System.ComponentModel;
using System.Windows;

namespace Devsurf.Security.Otp.Client
{
    class MotpProvider : INotifyPropertyChanged
    {
        private String last = "";

        private String pin = "";

        private String secret = "";

        private Motp motp;

        public event PropertyChangedEventHandler PropertyChanged;

        public String Now
        {
            get { 
                if(motp == null){
                    motp = new Motp(Pin, Secret);
                }
                return motp.Now; 
            }
        }

        public String Secret {
            private get {
                return secret;
            }
            set {
                secret = value;
                motp = new Motp(Pin, value);
                NotifyPropertyChanged("Now");
            }
        }

        public String Pin
        {
            private get {
                return pin;
            }
            set
            {
                pin = value;
                motp = new Motp(value, Secret);
                NotifyPropertyChanged("Now");
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && last != null && !last.Equals(Now) )
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                Clipboard.SetText(Now);
            }
        }
    }
}
