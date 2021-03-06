using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TV_Lab
{
    public partial class Form1 : Form
    {
        SV exper = new SV();
        public Form1()
        {
            InitializeComponent();
            PARAM.Text = (Convert.ToDouble(INTENSITY.Text) * Convert.ToDouble(TIME.Text)).ToString();
        }

        void FillMainTable()
        {
            int j = 0;
            for (int i = 0; i < exper.real_count; i++)
            {
                double var = (double)(exper.work_yi[i].kek) / (double)exper.count;
                {
                    TABLE_1.Rows.Insert(j++, exper.work_yi[i].number + 1, exper.work_yi[i].Nu, exper.work_yi[i].kek, var, exper.work_yi[i].pi, exper.work_yi[i].prob_disc);
                }
            }
        }

        void FillTableSVSettings()
        {
            TABLE_2.Rows.Insert(0, exper.En, exper.x_medium, Math.Abs(exper.En - exper.x_medium), exper.Dn, exper.Dn_medium, Math.Abs(exper.Dn - exper.Dn_medium), exper.Me, exper.R, exper.D_outrun, exper.prob_disc_max);
            TABLE_2.Rows.Insert(1, exper.En_calc, "", "", exper.Dn_calc);
            INFO_SIZE_F.Text = (exper.select_Fn.Length + 1).ToString();
        }

        void FillChartFnReal()
        {

            System.Windows.Forms.DataVisualization.Charting.Series serialFn = new System.Windows.Forms.DataVisualization.Charting.Series();
            serialFn.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            serialFn.IsValueShownAsLabel = false;
            String name3 = CHART_3.Series[0].Name;
            serialFn.Name = name3;

            for (int i = 0; i < exper.real_Fn.Count(); i++) // строим реальную Fn(x)
            {
                serialFn.Points.AddXY(exper.real_Fn[i].left, exper.real_Fn[i].p);
                serialFn.Points.AddXY(exper.real_Fn[i].right, exper.real_Fn[i].p);

                serialFn.Name = name3 + i;
                serialFn.ChartArea = "ChartArea1";
                serialFn.IsVisibleInLegend = false;
                serialFn.BorderWidth += 2;
                serialFn.Color = Color.Black;

                this.CHART_3.Series.Add(serialFn);

                serialFn = new System.Windows.Forms.DataVisualization.Charting.Series();
                serialFn.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serialFn.IsValueShownAsLabel = false;
            }
        }

        void FillChartFnExper()
        {
            System.Windows.Forms.DataVisualization.Charting.Series serial = new System.Windows.Forms.DataVisualization.Charting.Series();
            serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            serial.IsValueShownAsLabel = false;

            String name2 = CHART_2.Series[0].Name;
            serial.Name = name2;

            {
                serial.Points.AddXY(0, 0);
                serial.Points.AddXY(exper.select_Fn[0].left, 0);

                serial.Name = name2 + "zero";
                serial.ChartArea = "ChartArea1";
                serial.IsVisibleInLegend = false;
                serial.BorderWidth += 2;
                serial.Color = Color.Black;

                this.CHART_2.Series.Add(serial);

                serial = new System.Windows.Forms.DataVisualization.Charting.Series();
                serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serial.IsValueShownAsLabel = false;
            }

            // строим Fn_select

            for (int i = 0; i < exper.real_count; i++) // Строим выборочную Fn(x)
            {
                serial.Points.AddXY(exper.select_Fn[i].left, exper.select_Fn[i].p);
                serial.Points.AddXY(exper.select_Fn[i].right, exper.select_Fn[i].p);

                serial.Name = name2 + i;
                serial.ChartArea = "ChartArea1";
                serial.IsVisibleInLegend = false;
                serial.BorderWidth += 2;
                serial.Color = Color.Black;

                this.CHART_2.Series.Add(serial);

                serial = new System.Windows.Forms.DataVisualization.Charting.Series();
                serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serial.IsValueShownAsLabel = false;
            }
        }

        void FillQuadProbablity()
        {
            int j;
            System.Windows.Forms.DataVisualization.Charting.Series serial = new System.Windows.Forms.DataVisualization.Charting.Series();
            serial.IsValueShownAsLabel = false;

            String name = CHART_1.Series[0].Name;
            serial.Name = name;
            j = 0;

            serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            serial.BorderWidth += 1;
            serial.Points.AddXY(0, 0);
            serial.Points.AddXY(exper.work_yi[0].Nu, exper.work_yi[0].pi);
            serial.Name = name + j++;
            serial.ChartArea = "ChartArea1";
            serial.IsVisibleInLegend = false;
            serial.IsValueShownAsLabel = false;
            serial.Color = Color.Black;

            this.CHART_1.Series.Add(serial);

            serial = new System.Windows.Forms.DataVisualization.Charting.Series();
            for (int i = 0; i < exper.real_count - 1;)
            {

                if (exper.work_yi[i].Nu != exper.work_yi[i + 1].Nu - 1)
                {
                    serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    serial.Points.AddXY(exper.work_yi[i].Nu, exper.work_yi[i].pi);
                    serial.Points.AddXY(exper.work_yi[i + 1].Nu, exper.work_yi[i + 1].pi);
                    //i++;
                    serial.Name = name + j++;
                    serial.ChartArea = "ChartArea1";
                    serial.IsVisibleInLegend = false;
                    serial.IsValueShownAsLabel = false;
                    serial.Color = Color.Black;

                    this.CHART_1.Series.Add(serial);

                    serial = new System.Windows.Forms.DataVisualization.Charting.Series();
                }
                // else
                //{
                serial.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serial.BorderWidth += 1;
                serial.Points.AddXY(exper.work_yi[i].Nu, exper.work_yi[i].pi);
                serial.Points.AddXY(exper.work_yi[i + 1].Nu, exper.work_yi[i + 1].pi);
                i++;
                // }

                serial.Name = name + j++;
                serial.ChartArea = "ChartArea1";
                serial.IsVisibleInLegend = false;
                serial.IsValueShownAsLabel = false;
                serial.Color = Color.Black;

                this.CHART_1.Series.Add(serial);

                serial = new System.Windows.Forms.DataVisualization.Charting.Series();
            }
        }

        void FillChartTwinsFn()
        {
            System.Windows.Forms.DataVisualization.Charting.Series serial_real = new System.Windows.Forms.DataVisualization.Charting.Series();
            serial_real.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            serial_real.IsValueShownAsLabel = false;

            System.Windows.Forms.DataVisualization.Charting.Series serial_select = new System.Windows.Forms.DataVisualization.Charting.Series();
            serial_select.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            serial_select.IsValueShownAsLabel = false;

            int j = 0;

            String name_real = CHART_4.Series[0].Name;
            String name_select = CHART_4.Series[0].Name + j++;

            // накладываем реальную Fn(x)
            for (int i = 0; i < exper.real_Fn.Count(); i++) // строим реальную Fn(x)
            {
                serial_real.Points.AddXY(exper.real_Fn[i].left, exper.real_Fn[i].p);
                serial_real.Points.AddXY(exper.real_Fn[i].right, exper.real_Fn[i].p);

                serial_real.Name = name_real + j++;
                serial_real.ChartArea = "ChartArea1";
                serial_real.IsVisibleInLegend = false;
                serial_real.BorderWidth += 1;
                serial_real.Color = Color.OrangeRed;

                this.CHART_4.Series.Add(serial_real);

                serial_real = new System.Windows.Forms.DataVisualization.Charting.Series();
                serial_real.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serial_real.IsValueShownAsLabel = false;
            }

            for (int i = 0; i < exper.real_count; i++) // дубль цикла построения выборочной Fn(x)
            {
                serial_select.Points.AddXY(exper.select_Fn[i].left, exper.select_Fn[i].p);
                serial_select.Points.AddXY(exper.select_Fn[i].right, exper.select_Fn[i].p);

                serial_select.Name = name_select + j++;
                serial_select.ChartArea = "ChartArea1";
                serial_select.IsVisibleInLegend = false;
                serial_select.BorderWidth += 1;
                serial_select.Color = Color.BlueViolet;

                this.CHART_4.Series.Add(serial_select);

                serial_select = new System.Windows.Forms.DataVisualization.Charting.Series();
                serial_select.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                serial_select.IsValueShownAsLabel = false;
            }
        }

        
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            Application.Exit();
        }

        private void START_1_Click(object sender, EventArgs e)
        {
            if(backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            TABLE_1.Rows.Clear();
            TABLE_2.Rows.Clear();
            CLEAR_STAT_Click(sender, e);
            SHOW_VALUE.Enabled = true;
            COMPARSION.Enabled = true;
            if (Convert.ToDouble(INTENSITY.Text) <= 0)
            {
                MessageBox.Show("Интенсивность должна быть больше нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (Convert.ToDouble(TIME.Text) <= 0)
            {
                MessageBox.Show("Время должно быть больше нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (Convert.ToInt32(SIZE_EXP.Text) <= 0)
            {
                MessageBox.Show("Количество экспериментов должно быть больше нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CLEAR_1.Enabled = true;

            //
            ///
            ////
            backgroundWorker1.RunWorkerAsync();

            //
            ///
            ////
        }

        private void CLEAR_1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            TABLE_1.Rows.Clear();
            TABLE_2.Rows.Clear();
            CLEAR_STAT_Click(sender, e);
            if (CHART_1.Series.Count > 1)
            {
                int i = 1;
                while (CHART_1.Series.Count > 1)
                    CHART_1.Series.RemoveAt(i);
            }
            if (CHART_2.Series.Count > 1)
            {
                int i = 1;
                while (CHART_2.Series.Count > 1)
                    CHART_2.Series.RemoveAt(i);
            }
            if (CHART_3.Series.Count > 1)
            {
                int i = 1;
                while (CHART_3.Series.Count > 1)
                    CHART_3.Series.RemoveAt(i);
            }
            if (CHART_4.Series.Count > 1)
            {
                int i = 1;
                while (CHART_4.Series.Count > 1)
                    CHART_4.Series.RemoveAt(i);
            }
            CLEAR_1.Enabled = false;
            GET_P_TABLE.Enabled = false;
            FILTER_TABLE_1.Enabled = false;
            SHOW_VALUE.Enabled = false;
            SHOW_VALUE.Checked = false;
            COMPARSION.Enabled = false;
            COMPARSION.Checked = false;
            TIME_ELAPSED.Visible = false;
            exper.finalize();
        }

        private void INTENSITY_TextChanged(object sender, EventArgs e)
        {
            if (INTENSITY.Text == "") return;
            try
            {
                Convert.ToDouble(INTENSITY.Text);
            }
            catch
            {
                MessageBox.Show("Интенсивность должна быть типа Double!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PARAM.Text = (Convert.ToDouble(INTENSITY.Text) * Convert.ToDouble(TIME.Text)).ToString();
        }

        private void TIME_TextChanged(object sender, EventArgs e)
        {
            if (TIME.Text == "") return;
            try
            {
                Convert.ToDouble(TIME.Text);
            }
            catch
            {
                MessageBox.Show("Время должно быть типа Double!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PARAM.Text = (Convert.ToDouble(INTENSITY.Text) * Convert.ToDouble(TIME.Text)).ToString();
        }

        private void GET_P_TABLE_Click(object sender, EventArgs e)
        {
           if(TABLE_1.Rows.Count==1)
            {
                MessageBox.Show("Необходимо провести серию экспериментов!", "Оповещение", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
           P_FORM form = new P_FORM(exper);
           form.Show();
        }

        private void стартToolStripMenuItem_Click(object sender, EventArgs e)
        {
            START_1_Click(sender, e);
        }

        private void таблицаВероятностейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GET_P_TABLE_Click(sender, e);
        }

        private void FILTER_TABLE_1_Click(object sender, EventArgs e)
        {
            int j = 0;
            TABLE_1.Rows.Clear();
            int forend = Convert.ToInt32(SIZE_EXP.Text);
            for (int i = 0; i < forend; i++)
            {
                double var = (double)(exper.array_yi[i].kek) / (double)exper.count;
                if (exper.array_yi[i].original)
                {
                    TABLE_1.Rows.Insert(j++, exper.array_yi[i].number + 1, exper.array_yi[i].Nu, exper.array_yi[i].kek, var, exper.array_yi[i].pi, exper.work_yi[i].prob_disc);
                }
            }
            for (int i = 0; i < TABLE_1.Rows.Count; i++)
            {
                if (TABLE_1.Rows[i].Cells[0].Value == "")
                    TABLE_1.Rows.RemoveAt(i);
            }
        }

        private void SHOW_VALUE_CheckedChanged(object sender, EventArgs e)
        {
            if(SHOW_VALUE.Checked)
            {
                for (int i = 0; i < CHART_1.Series.Count; i++)
                {
                    CHART_1.Series[i].IsValueShownAsLabel = true;
                }
                for (int i = 0; i < CHART_2.Series.Count; i++)
                {
                    CHART_2.Series[i].IsValueShownAsLabel = true;
                }
                for (int i = 0; i < CHART_3.Series.Count; i++)
                {
                    CHART_3.Series[i].IsValueShownAsLabel = true;
                }
                for (int i = 0; i < CHART_4.Series.Count; i++)
                {
                    CHART_4.Series[i].IsValueShownAsLabel = true;
                }

            }
            else
            {
                for (int i = 0; i < CHART_1.Series.Count; i++)
                {
                    CHART_1.Series[i].IsValueShownAsLabel = false;
                }
                for (int i = 0; i < CHART_2.Series.Count; i++)
                {
                    CHART_2.Series[i].IsValueShownAsLabel = false;
                }
                for (int i = 0; i < CHART_3.Series.Count; i++)
                {
                    CHART_3.Series[i].IsValueShownAsLabel = false;
                }
                for (int i = 0; i < CHART_4.Series.Count; i++)
                {
                    CHART_4.Series[i].IsValueShownAsLabel = false;
                }
            }
        }

        private void COMPARSION_CheckedChanged(object sender, EventArgs e)
        {
            Random random_color = new Random();
            if (COMPARSION.Checked)
            {
                for (int i = 0; i < CHART_1.Series.Count; i++)
                {
                    CHART_1.Series[i].Color = Color.FromArgb(255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230));
                }
                for (int i = 0; i < CHART_2.Series.Count; i++)
                {
                    CHART_2.Series[i].Color = Color.FromArgb(255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230));
                }
                for (int i = 0; i < CHART_3.Series.Count; i++)
                {
                    CHART_3.Series[i].Color = Color.FromArgb(255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230), 255 - random_color.Next(10, 230));
                }
            }
            else
            {
                for (int i = 0; i < CHART_1.Series.Count; i++)
                {
                    CHART_1.Series[i].Color = Color.Black;
                }
                for (int i = 0; i < CHART_2.Series.Count; i++)
                {
                    CHART_2.Series[i].Color = Color.Black;
                }
                for (int i = 0; i < CHART_3.Series.Count; i++)
                {
                    CHART_3.Series[i].Color = Color.Black;
                }
            }
        }

        private void сбросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CLEAR_1_Click(sender, e);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            exper.finalize();
            exper = new SV();
            exper.Set(Convert.ToDouble(INTENSITY.Text), Convert.ToDouble(TIME.Text), Convert.ToInt32(SIZE_EXP.Text));
            Stopwatch timer = new Stopwatch();
            timer.Start();
            //exper.Generate(); // Single generate
            exper.GenerateParallel(); // Parallel generate
            exper.FilterOriginal();
            exper.FindValueSettings();

            timer.Stop();
            
            TIME_ELAPSED.Text = (timer.ElapsedMilliseconds / 1000.0).ToString() + " сек";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TIME_ELAPSED.Visible = true;
            // заполнение главной таблицы

            // FillMainTable();
            
            // конец заполнения главной таблицы
            // заполнение таблицы с характеристиками

            //FillSVSettings();

            // конец заполнения таблицы с характеристиками

            // очистка графиков

            if (CHART_1.Series.Count > 1)
            {
                int i = 1;
                while (CHART_1.Series.Count > 1)
                    CHART_1.Series.RemoveAt(i);
            }
            if (CHART_2.Series.Count > 1)
            {
                int i = 1;
                while (CHART_2.Series.Count > 1)
                    CHART_2.Series.RemoveAt(i);
            }
            if (CHART_3.Series.Count > 1)
            {
                int i = 1;
                while (CHART_3.Series.Count > 1)
                    CHART_3.Series.RemoveAt(i);
            }
            if (CHART_4.Series.Count > 1)
            {
                int i = 1;
                while (CHART_4.Series.Count > 1)
                    CHART_4.Series.RemoveAt(i);
            }
            Parallel.Invoke(
                () => FillMainTable(),
                () => FillTableSVSettings(),
                () => FillQuadProbablity(),
                () => FillChartFnReal(),
                () => FillChartFnExper(),
                () => FillChartTwinsFn());

            // конец очистки графиков
            // строим Многоугольник распределения

            //FillQuadProbablity();

            // заполнили многоугольник

            // строим Fn_select

            //FillChartFnSelect();

            //закончили Fn_select

            //строим Fn_real

            //FillChartFnReal();

            //построили Fn_real

            //FillChartTwinsFn();


            // накладываем графики
            // при чём очень не оптимизированно

            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //PROGRESS.Value = e.ProgressPercentage;
        }

        private void GET_STAT_Click(object sender, EventArgs e)
        {
            
            if (INTERVALS_K.Text == "") return;
            try
            {
                Convert.ToInt32(INTERVALS_K.Text);
            }
            catch
            {
                MessageBox.Show("Количество интервалов K должно быть целочисленным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int intervals = Convert.ToInt32(INTERVALS_K.Text); 
            if (intervals > exper.select_Fn.Length - 1)
            {
                MessageBox.Show("Количество интервалов K должно быть меньше элементов выборки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (LEVEL_A.Text == "") return;
            try
            {
                Convert.ToDouble(LEVEL_A.Text);
            }
            catch
            {
                MessageBox.Show("Уровень значимости должен быть типа Double", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double level = Convert.ToDouble(LEVEL_A.Text);
            if (level <= 0 || level >= 1)
            {
                MessageBox.Show("Уровень значимости должен быть в интервале [ 0 ; 1 ]", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            exper.a_level = level;
            exper.k_intervals = intervals;
            bool good_test = exper.CalcStat();
            TABLE_STATISTIC.Rows.Clear();
            for (int i = 0; i < intervals; i++)
            {
                TABLE_STATISTIC.Rows.Insert(i, "( " + exper.intervals[i].left.ToString() + ";" + exper.intervals[i].right.ToString() + " ]",
                    exper.intervals[i].count,
                    exper.intervals[i].n,
                    exper.intervals[i].p,
                    exper.count * exper.intervals[i].p,
                    (Math.Pow(exper.intervals[i].count - (double)exper.count * exper.intervals[i].p, 2.0)) / (double)(exper.count * exper.intervals[i].p));
            }
            XI2_VALUE.Text = exper.R0.ToString();
            XI2_CALC.Text = Math.Abs(exper.Xi2).ToString();
            TABLE_STATISTIC.Rows.Insert(intervals, "");
            TABLE_STATISTIC.Rows[intervals].Cells[2].Value = exper.intervals[0].n;
            TABLE_STATISTIC.Rows[intervals].Cells[3].Value = exper.intervals[0].p;
            for (int i = 1; i < intervals; i++)
            {
                TABLE_STATISTIC.Rows[intervals].Cells[2].Value = Convert.ToDouble(TABLE_STATISTIC.Rows[intervals].Cells[2].Value) + exper.intervals[i].n;
                TABLE_STATISTIC.Rows[intervals].Cells[3].Value = Convert.ToDouble(TABLE_STATISTIC.Rows[intervals].Cells[3].Value) + exper.intervals[i].p;
            }
            if (good_test) CHECK_OK.Text = "OK";
            else CHECK_OK.Text = "FALSE";
            FREE_BORDER.Text = (intervals - 1).ToString();
        }

        private void CLEAR_STAT_Click(object sender, EventArgs e)
        {

            TABLE_STATISTIC.Rows.Clear();
            XI2_VALUE.Text = "";
            XI2_CALC.Text = "";
            //INFO_SIZE_F.Text = "";
            FREE_BORDER.Text = "";
            CHECK_OK.Text = "";
        }
    }
}
