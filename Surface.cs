using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseKinematic
{
    public class Surface:ViewModelBase
    {

        public Surface()
        {
            SurfacesList.Add(new Tuple<string, int[]>("test", new int[] { 1, 2, 3 }));
        }

        public Surface(string surfacesString)
        {
            string[] lines = surfacesString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var item in lines)
            {
                string[] strings = item.Split(new[] { ",", ";" }, StringSplitOptions.None);
                int[] rgbParameter=new int[3];
                int.TryParse(strings[1], out rgbParameter[0]);
                int.TryParse(strings[2], out rgbParameter[1]);
                int.TryParse(strings[3], out rgbParameter[2]);
                SurfacesList.Add(new Tuple<string, int[]>(strings[0], rgbParameter));
            }
        }


        //Nazwa materiału, współczynnik
        public List<Tuple<string, int[]>> SurfacesList { get; set; } = new List<Tuple<string, int[]>>();

        public Tuple<string, int[]> SelectedSurface { get; set; }

        public List<Tuple<string, int[]>> IdentifySurface(double[] colorRecognitionAdjustedColorsMean)
        {
            var OrderedSurfaceList = SurfacesList.OrderBy(x => (Math.Abs(x.Item2[0]-(int)colorRecognitionAdjustedColorsMean[0])+ Math.Abs(x.Item2[1] - (int)colorRecognitionAdjustedColorsMean[1])+ Math.Abs(x.Item2[2] - (int)colorRecognitionAdjustedColorsMean[2])));
            SelectedSurface = OrderedSurfaceList.First();
            OnPropertyChanged(nameof(SelectedSurface));
            return OrderedSurfaceList.ToList();
        }


    }
}
