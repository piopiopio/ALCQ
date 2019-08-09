using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace ReverseKinematic
{
    public class Materials : ViewModelBase
    {

        private int scale;

        public Materials()
        {
            MaterialList.Add(new Tuple<string, double>("test", 1));
        }

        public Materials(string materialsString, string scaleInput)
        {

            
            scale = Int32.Parse(scaleInput.Remove(scaleInput.Length-1));

            string[] lines = materialsString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var item in lines)
            {
                string[] strings = item.Split(new[] { ",", ";" }, StringSplitOptions.None);
                double distanceParameter;
                double.TryParse(strings[1], out distanceParameter);
                MaterialList.Add(new Tuple<string, double>(strings[0], distanceParameter));
            }

        }


        //Nazwa materiału, współczynnik
        public List<Tuple<string, double>> MaterialList { get; set; } = new List<Tuple<string, double>>();


        public Tuple<string, double> SelectedMaterial { get; set; }

        public void IdentifyMaterial(int input)
        {
            var OrderedMaterialList = MaterialList.OrderBy(x => (Math.Abs(x.Item2 - ((double)scale/input))));
            SelectedMaterial = OrderedMaterialList.First();
            OnPropertyChanged(nameof(SelectedMaterial));
          MaterialList = OrderedMaterialList.ToList();
          OnPropertyChanged(nameof(MaterialList));
          
        }

    }
}