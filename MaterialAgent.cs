using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace ReverseKinematic
{
    public class MaterialAgent
    {
        public int sensorRead;
        public double sensorDistance=5; //Distance from sensor in mm
        public double sensorFactor = 0.00368; //Input data multiplied by this factor is distance in mm. 
        public Tuple<string, double> Material=new Tuple<string, double>("",0);
        private Materials _materials;
        public MaterialAgent()
        {
            //numbers = new List<int>();
            //numbers.Add(2);
            //numbers.Add(5);
            //numbers.Add(7);
            //numbers.Add(10);
            _materials=new Materials();
        }

        public void Update(string CompleteMessage)
        {
            //int measuredDistance;
            int.TryParse(CompleteMessage, out sensorRead);
          //  double materialIndex = (measuredDistance * sensorFactor) / sensorDistance;
           // Material = _materials.MaterialList.Aggregate((x, y) => Math.Abs(x.Item2 - materialIndex) < Math.Abs(y.Item2 - materialIndex) ? x : y);

        }
    }
}
