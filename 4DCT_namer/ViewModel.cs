using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Reactive.Disposables;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using Models.FourDCT_namer;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ViewModels
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CompositeDisposable _disposable { get; } = new CompositeDisposable();
        // GUI interfaces
        public ReactivePropertySlim<string> Id { get; } = new ReactivePropertySlim<string>("some ID");
        public ReactivePropertySlim<string> Name { get; } = new ReactivePropertySlim<string>("some Name");
        public ReactivePropertySlim<string> Comment { get; } = new ReactivePropertySlim<string>("");
        public ReactiveCollection<string> DateTimeList { get; private set; } = new ReactiveCollection<string>();


        // intermediate variables
        private Dictionary<DateTime, PhaseImagesArray> PhaseImagesArrays { get; } = new Dictionary<DateTime, PhaseImagesArray>() { };
        public ReactivePropertySlim<string> SelectedDateTimeNumImages { get; set; }
        public ReactivePropertySlim<DateTime> SelectedDateTime { get; set; }
        public ReactivePropertySlim<string> SeriesName { get; set; }
        private ReactivePropertySlim<bool> IsGenNameValid { get; set; } = new ReactivePropertySlim<bool>(false);
        public ReactiveCommand ClickCommand { get; set; } 


        ScriptContext context;


        public ViewModel()
        {
            SelectedDateTimeNumImages = new ReactivePropertySlim<string>().AddTo(_disposable);
            SelectedDateTime = new ReactivePropertySlim<DateTime>().AddTo(_disposable);
            SeriesName = new ReactivePropertySlim<string>().AddTo(_disposable);

            SelectedDateTimeNumImages.Where(a => string.IsNullOrEmpty(a) == false).Subscribe(a =>
            {
                string[] strs = a.Split(',');

                if (DateTime.TryParse(strs[0], out DateTime dt))
                {
                    SelectedDateTime.Value = dt;
                }
                else {; }
            }).AddTo(_disposable);

            Comment.Where(a => string.IsNullOrEmpty(a) == false).Subscribe(a =>
            {
                string buf = $"'位相_{Comment.Value}'";
                (IsGenNameValid.Value, _) = GenNameValid(buf);

                buf = buf.Replace("_", "__");
//                buf = $"'位相__{Comment.Value}'";
//                MessageBox.Show(buf);
                SeriesName.Value = $"{buf}にリネームする。";
            }).AddTo(_disposable);

            ClickCommand = IsGenNameValid
                .ToReactiveCommand()
                .WithSubscribe(() => Rename4DCTSeries())
                .AddTo(_disposable);

        }

        public void SetScriptContext (in ScriptContext _context)
        {
            context = _context;
            Id.Value = context.Patient.Id;
            Name.Value = context.Patient.FirstName + ", " + context.Patient.LastName;
            //Date.Value = context.Image.Series.Study.CreationDateTime.ToString();

            // debug (Structure count)
            //            string buf = "";
            //            var numofphases = InstModel.NumOfPhases;
            //            var dict = InstModel.StructureNumOfPhasesPairs;
            //            buf += "NumOfPhases: " + numofphases;
            //            foreach (var elem in dict)
            //            {
            //                buf += $"\n\t Name: {elem.Key}\tCount: {elem.Value}";
            //            }
            //            MessageBox.Show(buf);

            var studies = context.Patient.Studies;

            foreach(var study in studies)
            {
                foreach (var series in study.Series)
                {
                    foreach (var img in series.Images)
                    {
                        if (Double.IsNaN(img.ZDirection.x) | Double.IsNaN(img.ZDirection.y) | Double.IsNaN(img.ZDirection.z)) 
                        {
                            // if it's not a volume image
                            continue;
                        }
                        else
                        {
                            // for debug begin 
//                            string buf;
//                            buf = $"{img.Id} {img.ZDirection.x} {img.ZDirection.y} {img.ZDirection.z}\n";
//                            MessageBox.Show(buf);
                            // for debug end

                            var datetime = img.CreationDateTime;

                            if (datetime == null)
                            {
                                continue;
                            }
//                            else if (PhaseImagesArrays.ContainsKey(datetime.GetValueOrDefault()) == false) {  // to avoid throwing Exception, GetValueOrDefault() is used instead of .Value
//                                PhaseImagesArrays.Add(datetime.Value, new PhaseImagesArray());
//    //                            PhaseImagesArrays[datetime.Value].Images.Add(new PhaseImages(img));
//                            }
//                            else {; }
//                            PhaseImagesArrays[datetime.Value].Images.Add(new PhaseImages(img));

                            else
                            {
                                var datetime_val = datetime.Value;
                                bool key_found = false;

                                foreach(var dt in PhaseImagesArrays.Keys)
                                {
                                    if (Math.Abs((dt - datetime_val).TotalMinutes) <= 1) {
                                        PhaseImagesArrays[dt].Images.Add(new PhaseImages(img));
                                        key_found = true;
                                    }
                                    else {; }
                                }

                                if (key_found == false)
                                {
                                    PhaseImagesArrays.Add(datetime_val, new PhaseImagesArray());
                                    PhaseImagesArrays[datetime_val].Images.Add(new PhaseImages(img));
                                }
                                else {; }

                            }

                        }
                    }
                }
            }


            foreach(var elem in PhaseImagesArrays)
            {
                var dt = elem.Key;
                var arr = elem.Value;

                string buf = $"{dt}, {arr.Images.Count} images";

                DateTimeList.Add(buf);

            }
        }

        public (bool, string) GenNameValid(in string _name)
        {
            bool res = false;
            string gen_name = _name;
            string error_message = "";

            if ((gen_name == null) || (gen_name == ""))
            {
                res = false;
                error_message = "名前が空です。";
            }
            else if ((gen_name.Length > 16) || (gen_name.Length < 1))
            {
                res = false;
                error_message = "文字数が不正です（1-16文字ならOK）。";
            }
////            else if (_context.ExternalPlanSetup.StructureSet.Structures.Any(elem => elem.Id == gen_name))
//            else if (Structures_RC.Any(elem => elem.Id == gen_name))
//            {
//                res = false;
//                error_message = "入力されたストラクチャーが既に存在します。";
//            }
            else if (gen_name.Contains("\\")){
                res = false;
                error_message = "不正な文字が使われています。";
            }
            else
            {
                res = true;
                error_message = "";
            }

            return (res, error_message);
        }

        public void Rename4DCTSeries()
        {
//            string buf = $"'位相_{Comment.Value}'";
//            MessageBox.Show($"Convert to {buf}");

            try
            {
                context.Patient.BeginModifications();

                var phase_imgs = PhaseImagesArrays[SelectedDateTime.Value];
                foreach(var img in phase_imgs.Images)
                {
                    string buf = $"{img.Phase}_{Comment.Value}";
                    img.ChangeImageName(buf);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: '{e.Message}' occured in Rename4DCTSeries()");
            }
        }

        public void Dispose() => _disposable.Dispose();


    }



}
