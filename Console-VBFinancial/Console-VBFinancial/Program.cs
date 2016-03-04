using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Console_VBFinancial
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // double pmt = Financial.Pmt(rate, nper, pv, fv, due); 
            Console.WriteLine(Financial.Pmt(1.03, 5, 3.2, 100.3, DueDate.EndOfPeriod));
        }
    }
}