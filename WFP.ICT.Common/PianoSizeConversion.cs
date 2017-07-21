using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFP.ICT.Common
{
    public class PianoSizeConversion
    {
        public static string GetFeetInches(double centimeters)
        {
            Double Feet = (centimeters / 2.54) / 12;
            int iFeet = (int)Feet;
            Double inches = (Feet - (double)iFeet) * 12;

            return Feet+"'"+inches+"''";
        }
    }
}
