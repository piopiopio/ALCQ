using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseKinematic
{
    public class WeightAgent
    {
        public double MassFactor = 1;  //Date from cell multiplied by this value get results in grams. 
        private double mass;
        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public void Update(string CompleteMessage)
        {
            int dataFromSensor;
            int.TryParse(CompleteMessage, out dataFromSensor);
            Mass = dataFromSensor * MassFactor;
        }
    }
}
