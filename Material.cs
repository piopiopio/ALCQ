using System;
using System.Collections.Generic;
using System.Linq;

namespace ReverseKinematic
{
    public class Material : ViewModelBase
    {
        private Tuple<string, double, double> selectedMaterial;

        private Tuple<double, double> selectedThickness;

        public Material()
        {
            MaterialList.Add(new Tuple<string, double, double>("Stainless steel", 30, 7700));
            MaterialList.Add(new Tuple<string, double, double>("Carbon steel", 3, 7900));
            MaterialList.Add(new Tuple<string, double, double>("Aluminum", 25, 2700));


            MaterialThicknessList.Add(new Tuple<double, double>(0.5, 0.01));
            MaterialThicknessList.Add(new Tuple<double, double>(1.0, 0.02));
            MaterialThicknessList.Add(new Tuple<double, double>(2.0, 0.03));
            MaterialThicknessList.Add(new Tuple<double, double>(3.0, 0.05));
            MaterialThicknessList.Add(new Tuple<double, double>(4.0, 0.08));
            OnPropertyChanged(nameof(MaterialList));
            OnPropertyChanged(nameof(MaterialThicknessList));

            SelectedThickness = MaterialThicknessList.First();
            SelectedMaterial = MaterialList.First();
        }

        //Nazwa materiału, cena 1 kg, gestosc g/cm3
        public List<Tuple<string, double, double>> MaterialList { get; set; } =
            new List<Tuple<string, double, double>>();

        //Grubość materiału, koszt ciecia 1mm
        public List<Tuple<double, double>> MaterialThicknessList { get; set; } = new List<Tuple<double, double>>();

        public Tuple<string, double, double> SelectedMaterial
        {
            get => selectedMaterial;
            set
            {
                selectedMaterial = value;
                OnThresholdReached(null);
            }
        }

        public Tuple<double, double> SelectedThickness
        {
            get => selectedThickness;
            set
            {
                selectedThickness = value;
                OnThresholdReached(null);
            }
        }

        public double GetOneMM2Price => 0.001 * 0.001 * 0.001 * SelectedThickness.Item1 * SelectedMaterial.Item3 *
                                        SelectedMaterial.Item2;

        public event EventHandler ThresholdReached;

        protected virtual void OnThresholdReached(EventArgs e)
        {
            var handler = ThresholdReached;
            if (handler != null) handler(this, e);
        }
    }

    public class ThresholdReachedEventArgs : EventArgs
    {
        public int Threshold { get; set; }
        public DateTime TimeReached { get; set; }
    }
}