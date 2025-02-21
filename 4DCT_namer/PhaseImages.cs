using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using System.Windows;
using System.Linq;
using Reactive.Bindings.Extensions;
using System.Linq.Expressions;
using System.Numerics;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Models.FourDCT_namer
{
    public class PhaseImages : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();

        public string Phase { get; set; }
        public string Name { get; set; }
        private Image phase_image { get; set; }

        public void Dispose() => _disposable.Dispose();

        public PhaseImages() { }
        public PhaseImages(in Image img) 
        {
            var ser = img.Series;
            Phase = ParsePhaseComment(ser.Comment).Item2;
            Name = img.Id;

            phase_image = img;
        }
        public (bool, string) ParsePhaseComment(in string comment)
        {
            bool is_phase = true;
            string phase = "null";

            if ((comment == null) || (comment == "")) { phase = "null"; is_phase = false; }
            else if (comment.Contains("T=0%,")) { phase = "0"; }
            else if (comment.Contains("T=5%,")) { phase = "5"; }
            else if (comment.Contains("T=10%,")) { phase = "10"; }
            else if (comment.Contains("T=15%,")) { phase = "15"; }
            else if (comment.Contains("T=20%,")) { phase = "20"; }
            else if (comment.Contains("T=25%,")) { phase = "25"; }
            else if (comment.Contains("T=30%,")) { phase = "30"; }
            else if (comment.Contains("T=35%,")) { phase = "35"; }
            else if (comment.Contains("T=40%,")) { phase = "40"; }
            else if (comment.Contains("T=45%,")) { phase = "45"; }
            else if (comment.Contains("T=50%,")) { phase = "50"; }
            else if (comment.Contains("T=55%,")) { phase = "55"; }
            else if (comment.Contains("T=60%,")) { phase = "60"; }
            else if (comment.Contains("T=65%,")) { phase = "65"; }
            else if (comment.Contains("T=70%,")) { phase = "70"; }
            else if (comment.Contains("T=75%,")) { phase = "75"; }
            else if (comment.Contains("T=80%,")) { phase = "80"; }
            else if (comment.Contains("T=85%,")) { phase = "85"; }
            else if (comment.Contains("T=90%,")) { phase = "90"; }
            else if (comment.Contains("T=95%,")) { phase = "95"; }
            else if (comment.Contains("MIP")) { phase = "MIP"; }
            else if (comment.Contains("Ave")) { phase = "Ave"; }
            else { phase = comment; is_phase = false; }

            return (is_phase, phase);
        }

        public void ChangeImageName(in string name)
        {
            phase_image.Id = name;
        }
        
    }

    public class PhaseImagesArray : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();


        public ReactiveCollection<PhaseImages> Images { get; set; } = new ReactiveCollection<PhaseImages>();


        public PhaseImagesArray()
        {
            Images = new ReactiveCollection<PhaseImages>();
        }

    }
}
