using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TV_Lab
{
    public partial class P_FORM : Form
    {
        private SV exper = new SV();
        public P_FORM(SV _exper)
        {
            InitializeComponent();
            exper = new SV(_exper);
            for (int i = 0; i < exper.count; i++)
            {
                P_TABLE.Columns.Add("SV" + (i + 1), "С.В. " + (i + 1));
            }
            int max = exper.FindMaxNu();
            for (int i = 0; i <= max; i++)
            {
                P_TABLE.Rows.Add();
            }
            for (int i = 0; i < exper.count; i++)
            {
                P_TABLE.Rows[0].Cells["SV" + (i + 1)].Value = exper.array_yi[i].Nu;
            }

            for (int i = 0; i < exper.count; i++) 
            {
               // for (int j = 0; j < exper.array_yi[i].p.Count(); j++)
               // {
                //    P_TABLE.Rows[j + 1].Cells[i].Value = exper.array_yi[i].p[j].ToString();
               // }
            }
            exper.finalize();

        }
    }
}
