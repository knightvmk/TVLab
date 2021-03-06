using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace TV_Lab
{
    public struct Y
    {
        //public List<double> p; // вектор вероятностей
        public int size; // размер вектора вероятностей 
        public int Nu; // случайная величина
        public int kek; // сколько раз встретили случайную величину
        public int number; // номер испытания
        public bool original; // уникальность
        public double prob_disc; // невязка вероятности
        public double pi; // вероятность найденная
    }
    public struct Fn // хранение кусков функции выборочной функции распределения
    {
        public int left; // левая граница
        public int right; // правая граница
        public double p; // вероятность
    }
    public struct FXi // хранение интервального разбиения
    {
        public int left; // левая граница
        public int right; // правая граница
        public double n; // частота
        public double p; // вероятность
        public int count; // количество попавших значений СВ из выборки в границу
    }

    public class SV
    {
        public double intensity; // интенсивность
        public double time; // время
        public int count; // количество экспериментов
        public double param; // intencity*time // параметр 
        public int real_count;  // количество найденных значений СВ
        private bool IsSorted;
        //public double[] y;
        public Y[] array_yi; // вычислительная структура СВ (несорт, нефильтр)
        public Y[] work_yi; // рабочая структура полученных и сортированных СВ
        public Fn[] real_Fn; // хранение кусков функции распределения
        public Fn[] select_Fn; // хранение кусков выборочной функции распределения
        // характеристики СВ
        public int kek_all; // сколько получили ИТОГО СВ
        public double x_medium; // среднее значение СВ
        public double prob_disc_max; // максимальное значение невязки вероятности
        public double En; // матожидание
        public double En_calc; // матожидание посчитанное экспериментально
        public double Dn; // дисперсия
        public double Dn_calc; // дисперсия посчитанная экспериментально
        public double Dn_medium; // выборочная дисперсия
        public double Me; // медиана
        public double R; // размах выборки
        public double D_outrun; // мера расхождения // max(abs(Fn_real - Fn_select))
        // статистика
        public int k_intervals; // k интервалов разбиения
        public double a_level; // уровень значимости alpha
        public double Xi2; // статистика Хи^2
        public double R0;
        public FXi[] intervals; // разбитая выборка с вероятностью


        static private Random rand;

        private long factorial(long x)
        {
            return (x == 0) ? 1 : x * factorial(x - 1);
        }

        public SV() { }

        public SV(double _intensity, double _time, int _count) // конструктор по входным параметрам
        {
            intensity = _intensity; // интенсивность а/м
            time = _time; // время
            count = _count; // количество испытаний

            IsSorted = false;

            array_yi = new Y[count];
            array_yi.AsParallel();
        }

        public SV(SV enter)
        {
            intensity = enter.intensity;
            time = enter.time;
            count = enter.count;
            array_yi = new Y[count];
            IsSorted = enter.IsSorted;
            for (int i = 0; i < count; i++)
            {
                array_yi[i].pi = enter.array_yi[i].pi;
                array_yi[i].Nu = enter.array_yi[i].Nu;
                array_yi[i].size = enter.array_yi[i].size;
                array_yi[i].number = enter.array_yi[i].number;
                array_yi[i].original = enter.array_yi[i].original;
            }
        }

        public void Set(double _intensity, double _time, int _count) 
        {
            if(count!=0)
            {
                for (int i = 0; i < count; i++)
                {
                    array_yi[i].original = true;
                }
                array_yi = null;
            }
            IsSorted = false;
            intensity = _intensity; // интенсивность а/м
            time = _time; // время
            count = _count; // количество испытаний

            param = time * intensity;



            //y = new double[count]; // массив для y i-ых.
            array_yi = new Y[count];
        }

        public void finalize()
        {
            for (int i = 0; i < count; i++)
            {
                //array_yi[i].p.Clear();
                //array_yi[i].p = null;
                array_yi[i].Nu = 0;
                array_yi[i].pi = 0;
                array_yi[i].size = 0;
                array_yi[i].number = 0;
                array_yi[i].original = true;
            }
            array_yi = null;
            work_yi = null;
            select_Fn = null;
            real_Fn = null;
            intensity = 0;
            time = 0;
            count = 0;
        }

        public void GenerateParallel()
        {
            ThreadRandom ParallelRandom = new ThreadRandom();
            double param = intensity * time;
            Parallel.For(0, count, (i) =>
              {
                  //Thread.Sleep(1);
                  double u = ParallelRandom.NextDouble(); // "случайно" сгенерированное число "u"
                  double p1, p0 = Math.Exp(-1 * param);
                  double tmp = p0;
                  array_yi[i].pi = tmp;
                  int j = 0;
                  array_yi[i].size = j;
                  while (u > tmp)
                  {
                      j++;
                      p1 = (param / j) * p0;
                      array_yi[i].pi = p1;
                      p0 = p1;
                      tmp += p1;
                  }

                  {
                      array_yi[i].size = j;
                      array_yi[i].Nu = j;
                      array_yi[i].kek = 1;
                      array_yi[i].number = i;
                      array_yi[i].original = true;
                  }
              });
            Check();
        }

        public void Generate()
        {
            rand = new Random();
            double param = intensity * time; // параметр распределения Пуассона
            for (int i = 0; i < count; i++)
            {
                double u = rand.NextDouble(); // "случайно" сгенерированное число "u"
                double p1, p0 = Math.Exp(-1 * param); // вероятности для счёта суммы pj
                double tmp = p0; // переменная для промежуточного хранения
                array_yi[i].pi = tmp; // запишем первый участок
                int j = 0;
                array_yi[i].size = j;
                while (u > tmp) // пока u < сумма pj
                {
                    j++;
                    p1 = (param / j) * p0;
                    array_yi[i].pi = p1;
                    p0 = p1;
                    tmp += p1;
                }

                {
                    array_yi[i].size = j;
                    array_yi[i].Nu = j; // значение св
                    array_yi[i].kek = 1; // частота, считается в методе Check()
                    array_yi[i].number = i;
                    array_yi[i].original = true; // флаг для проверки в функции FilterOriginal()
                }
                
            }
            Check();
        }

        private void SortOriginal() // пузырек наше всё
        {
            if (IsSorted) return;
            Y tmp;
            //int tmp_Nu;
            //int tmp_size;
            //int tmp_kek;
            //int tmp_number;
            //double tmpi;
            //double tmp_prob;
            for (int i = 0; i < real_count; i++)
            {
                for (int j = 0; j < real_count; j++) 
                {
                    if (work_yi[i].Nu < work_yi[j].Nu)
                    {
                        //tmp_Nu = work_yi[i].Nu;
                        //tmp_size = work_yi[i].size;
                        //tmp_kek = work_yi[i].kek;
                        //tmp_number = work_yi[i].number;
                        //tmpi = work_yi[i].pi;
                        //tmp_prob = work_yi[i].prob_disc;

                        //work_yi[i].Nu = work_yi[j].Nu;
                        //work_yi[i].size = work_yi[j].size;
                        //work_yi[i].kek = work_yi[j].kek;
                        //work_yi[i].number = work_yi[j].number;
                        //work_yi[i].pi = work_yi[j].pi;
                        //work_yi[i].prob_disc = work_yi[j].prob_disc;

                        //work_yi[j].Nu = tmp_Nu;
                        //work_yi[j].size = tmp_size;
                        //work_yi[j].kek = tmp_kek;
                        //work_yi[j].number = tmp_number;
                        //work_yi[j].pi = tmpi;
                        //work_yi[j].prob_disc = tmp_prob;
                        tmp = array_yi[i];
                        array_yi[i] = array_yi[j];
                        array_yi[j] = tmp;
                    }
                }
            }
            IsSorted = true;
        }

        private void SortFullArray() // пузырек наше всё
        {
            Y tmp;
            //int tmp_Nu;
            //int tmp_size;
            //int tmp_kek;
            //int tmp_number;
            //double tmpi;
            //double tmp_prob;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array_yi[i].Nu < array_yi[j].Nu)
                    {
                        //tmp_Nu = array_yi[i].Nu;
                        //tmp_size = array_yi[i].size;
                        //tmp_kek = array_yi[i].kek;
                        //tmp_number = array_yi[i].number;
                        //tmpi = array_yi[i].pi;
                        //tmp_prob = array_yi[i].prob_disc;

                        //array_yi[i].Nu = array_yi[j].Nu;
                        //array_yi[i].size = array_yi[j].size;
                        //array_yi[i].kek = array_yi[j].kek;
                        //array_yi[i].number = array_yi[j].number;
                        //array_yi[i].pi = array_yi[j].pi;
                        //array_yi[i].prob_disc = array_yi[j].prob_disc;

                        //array_yi[j].Nu = tmp_Nu;
                        //array_yi[j].size = tmp_size;
                        //array_yi[j].kek = tmp_kek;
                        //array_yi[j].number = tmp_number;
                        //array_yi[j].pi = tmpi;
                        //array_yi[j].prob_disc = tmp_prob;
                        tmp = array_yi[i];
                        array_yi[i] = array_yi[j];
                        array_yi[j] = tmp;
                    }
                }
            }
        }

        public void FilterOriginal() 
        {
            // Ищем оригинальные СВ
            SortFullArray();
            int new_size = 0;
            for (int i = 0; i < count; i++) 
            {
                if (!array_yi[i].original) continue;
                for (int j = 0; j < count; j++)
                {
                    if ((array_yi[i].Nu == array_yi[j].Nu) && i != j)
                    {
                        array_yi[j].original = false;
                        ++new_size;
                    }
                       
                }    
            }
            real_count = count - new_size;

            work_yi = new Y[real_count];
            work_yi.AsParallel();

            kek_all = 0;

            // создание новой структуры для оригинальных СВ
            for (int i = 0, j = 0; i < count; i++) 
            {
                if (!array_yi[i].original) continue;
                work_yi[j].kek = array_yi[i].kek;
                kek_all += work_yi[j].kek;
                work_yi[j].Nu = array_yi[i].Nu;
                work_yi[j].number = array_yi[i].number;
                work_yi[j].original = true;
                work_yi[j].pi = array_yi[i].pi;
                work_yi[j].size = array_yi[i].size;
                ++j;
            }
            // Сортируем
            
            //SortOriginal();
            // ищем невязку вероятности
            for (int i = 0; i < real_count; i++)
            {
                work_yi[i].prob_disc = Math.Abs(((double)work_yi[i].kek / (double)count) - work_yi[i].pi);
            }
            // ищем максимум
            prob_disc_max = work_yi[0].prob_disc;
            for (int i = 1; i < real_count; i++)
            {
                if (prob_disc_max < work_yi[i].prob_disc) prob_disc_max = work_yi[i].prob_disc;
            }
        }

        public int FindMaxNu() // нахождение максимального значения СВ
        {
            int value = 0;
            for (int i = 0; i < count; i++)
            {
                if (value < array_yi[i].Nu)
                    value = array_yi[i].Nu;
            }
            return value;
        }

        public void FindValueSettings() // распараллеленный поиск
        {
            // распараллеливание
            int tasks = 5; // количество задач для потоков
            Thread[] threads = new Thread[tasks];
            threads[0] = new Thread(() =>
            {
                CalcEn();
                CalcDn();
            });
            threads[1] = new Thread(() =>
            {
                FindXMedium();
                CalcMediumDn();
            });
            threads[2] = new Thread(() =>
            {
                CalcMe();
                CalcR();
            });
            threads[3] = new Thread(() =>
            {
                CalcFnReal(0.000001);
                CalcFnSelect();
                CalcD_outrun();
            });
            threads[4] = new Thread(() =>
            {
                CalcProb_disc_max();
            });
            for (int i = 0; i < tasks; i++) threads[i].Start();
            for (int i = 0; i < tasks; i++) threads[i].Join();
        }
        private void CalcDn() // Подсчёт дисперсии
        {
            Dn_calc = 0;
            double first = 0, second = 0;

            for (int i = 0; i< real_count; i++)
            {
                first += Math.Pow(work_yi[i].Nu, 2) * work_yi[i].pi;
                second += work_yi[i].Nu * work_yi[i].pi;
            }
            second *= second;
            Dn_calc = first - second; // практическое через машинный счёт проведенного эксперимента
            Dn = intensity * time; // теоретическое
        }
        private void CalcMe() // подсчёт медианы
        {
            int k;
            if (count % 2 > 0)
            {           // n = 2k + 1
                k = count / 2;
                Me = array_yi[k].Nu;
            }
            else
            {           // n = 2k
                k = count / 2;
                Me = Convert.ToDouble((array_yi[k - 1].Nu + array_yi[k].Nu)) / 2.0;
            }
        }
        private void CalcEn() // подсчёт матожидания
        {
            En_calc = 0;
            for (int i = 0; i < real_count; i++)
            {
                En_calc += work_yi[i].Nu * work_yi[i].pi; // машинный счёт на основе эксперимента
            }
            En = intensity * time; // теоретическое
        }

        private void CalcR() // размах выборки
        {
            R = work_yi[real_count-1].Nu - work_yi[0].Nu;
        }

        private void CalcMediumDn() //S* // выборочная дисперсия
        {
            double sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += Math.Pow(array_yi[i].Nu - x_medium, 2);
            }
            Dn_medium = sum / count;
        }
        private void FindXMedium() // нахождение среднего значения СВ
        {
            double sum = 0;
            for (int i = 0; i < count; i++) 
            {
                sum += array_yi[i].Nu;
            }
            x_medium = sum / count;
        }
        private void Check() // распараллелено
        {
            Parallel.For(0, count, (i) =>
            {
                for (int j = 0; j < count; j++)
                {
                    if ((array_yi[i].Nu == array_yi[j].Nu) && (i != j))
                    {
                        array_yi[i].kek++;
                    }
                }
            });
        } 

        private void CalcFnSelect() // считаем выборочную функцию распределения
        {
            // всё таки надо с нуля
            select_Fn = new Fn[real_count];
            double summ = 0;
            for (int i = 0; i < real_count; i++)
            {
                summ += work_yi[i].kek;
                select_Fn[i].left = work_yi[i].Nu;
                if (i < real_count - 1) select_Fn[i].right = work_yi[i + 1].Nu;
                else select_Fn[i].right = work_yi[i].Nu + 1;
                select_Fn[i].p = summ / kek_all;
            }
        }

        private void CalcFnReal(double decision) // считаем функцию распределения // decision = Eps-точность выхода до достижения 1
        {
            List<Fn> temp_list = new List<Fn>();
            Fn tmp_kek = new Fn();
            double summ = 0;
            param = intensity * time;
            double p1, p0 = Math.Exp(-1 * param);
            tmp_kek.left = 0;
            tmp_kek.right = 1;
            summ = tmp_kek.p = p0;
            temp_list.Add(tmp_kek);
            int i = 1;
            while (1.0 - summ > decision)
            {
                p1 = (param / i) * p0;
                p0 = p1;
                summ += p0;
                tmp_kek.left = i;
                tmp_kek.right = ++i;
                tmp_kek.p = summ;
                temp_list.Add(tmp_kek);
            }
            real_Fn = new Fn[temp_list.Count + 1];
            for (int j = 0; j < temp_list.Count; j++)
            {
                real_Fn[j] = temp_list[j];
            }
            real_Fn[real_Fn.Length - 1].left = real_Fn[real_Fn.Length - 2].right;
            real_Fn[real_Fn.Length - 1].p = 1;
            real_Fn[real_Fn.Length - 1].right = real_Fn[real_Fn.Length - 1].left + 1;
            temp_list.Clear();
        }

        private void CalcD_outrun() // считаем меру расхождения
        {
            double max = -1;// = Math.Abs(real_Fn[0].p - select_Fn[0].p);
            double max_iterator = Math.Max(real_count, real_Fn.Count());
            int j = 0;
            for (int i = 0; i < max_iterator; i++)
            {

                if (j == Math.Min(real_count, real_Fn.Count())) break;
                if (select_Fn[j].left == real_Fn[i].left)
                {
                    if (max < Math.Abs(real_Fn[i].p - select_Fn[j].p)) max = Math.Abs(real_Fn[i].p - select_Fn[j].p);
                    ++j;
                };
            }
            D_outrun = max;
        }

        private void CalcProb_disc_max() // поиск максимальной невязки вероятности и частоты
        {
            double max = work_yi[0].prob_disc;
            for (int i = 1; i < real_count; i++)
            {
                if (max < work_yi[i].prob_disc) max = work_yi[i].prob_disc;
            }
            prob_disc_max = max;
        }

        public bool CalcStat()
        {
            CalcR0();
            CalcXi2();
            bool check = CheckStatisticXi2();
            return check;
        }
        private void CalcR0()
        {
            // разбиваем выборку на интервалы
            //k_intervals--;
            
            bool bad = false;
            int len_select = select_Fn.Length - 1;
            //len_select = work_yi.Length;
            intervals = new FXi[k_intervals];
            int size_division = len_select / k_intervals;
            if (len_select % k_intervals != 0) bad = true;
            // ГУ инициализация

            intervals[0].left = 0;
            intervals[0].right = work_yi[size_division].Nu;

            for (int j = 0; j <= size_division; j++)
            {
                intervals[0].n += (double)work_yi[j].kek / (double)kek_all;
                intervals[0].count += work_yi[j].kek;
            }
            for (int j = 1; j < real_Fn[intervals[0].right].right; j++)
                intervals[0].p += real_Fn[j].p - real_Fn[j - 1].p;
            
            for (int i = 1; i < k_intervals; i++)
            {
                // считаем границы
                intervals[i].left = intervals[i - 1].right;
                // предусмотреть выход
                if (i == k_intervals - 1)
                {
                    intervals[i].right = work_yi[work_yi.Length - 1].Nu;
                }
                else if((i + 1) * size_division == work_yi.Length) intervals[i].right = work_yi[(i + 1) * size_division - 1].Nu;
                else intervals[i].right = work_yi[(i + 1) * size_division].Nu;
                //
                // cчитаем частоту
                if (i != k_intervals - 1)
                {
                    if ((i + 1) * size_division != work_yi.Length)
                    {
                        for (int j = (i * size_division) + 1; j <= (i + 1) * size_division; j++)
                        {
                            intervals[i].n += (double)work_yi[j].kek / (double)kek_all;
                            intervals[i].count += work_yi[j].kek;
                        }
                    }
                    else
                    {
                        for (int j = (i * size_division) + 1; j < (i + 1) * size_division; j++)
                        {
                            intervals[i].n += (double)work_yi[j].kek / (double)kek_all;
                            intervals[i].count += work_yi[j].kek;
                        }
                    }
                    for (int j = intervals[i].left + 1; j < real_Fn[intervals[i].right].right; j++)
                    {
                        intervals[i].p += real_Fn[j].p - real_Fn[j - 1].p;
                    }
                }
                else
                {
                    for (int j = (i * size_division) + 1; j < work_yi.Length; j++)
                    {
                        intervals[i].n += (double)work_yi[j].kek / (double)kek_all;
                        intervals[i].count += work_yi[j].kek;
                    }

                    if (!bad)
                    {
                        if ((i * size_division) + 1 == work_yi.Length)
                        {
                            intervals[i].n += (double)work_yi[work_yi.Length - 1].kek / (double)kek_all;
                            intervals[i].count += work_yi[work_yi.Length - 1].kek;
                        }
                        intervals[i].right += 4;
                    }

                    for (int j = intervals[i].left + 1; j < real_Fn.Length; j++)
                    {
                        intervals[i].p += real_Fn[j].p - real_Fn[j - 1].p;
                    }
                    //int s = intervals[i].left;
                    //while (s <= real_Fn.Length - 1)
                    //{ intervals[i].p += real_Fn[s].p; s++; }
                }

            }
            double summ = 0;
            for (int i = 0; i < k_intervals - 1; i++)
                summ += intervals[i].p;
            intervals[k_intervals - 1].p = 1 - summ;
            R0 = 0.0;
            for (int i = 0; i < k_intervals; i++)
                R0 += (Math.Pow(intervals[i].count - count * intervals[i].p, 2.0)) / (count * intervals[i].p);
            // проверим критерий
        }

        private bool CheckStatisticXi2()
        {
            // tckb F(R0) > a, то критерий принят, R0 имеет распределение Xi2
            if (Xi2 < a_level) return true;
            else return false;
        }

        private void CalcXi2() // F(R0) с крышкой интегрированием
        {
            Xi2 = 1 - I(1000); // считаем интеграл методом трапеций
        }

        private double f(double x) // плотность вероятности
        {
            double res = 0;
            if (x <= 0)
                res = 0;
            else
                
            res = (Math.Pow(2, -((double)k_intervals-1.0) / 2.0) / SpecialFunction.gamma(((double)k_intervals-1.0) / 2.0)) * Math.Pow(x, (((double)k_intervals-1.0) / 2.0) - 1.0) * Math.Exp(-x / 2.0);
            //res = (Math.Pow(2, -((double)k_intervals - 1.0) / 2.0) / SpecialFunction.gamma(((double)k_intervals - 1.0) / 2.0)) * Math.Pow(x, ((double)k_intervals - 1.0) / 2.0 - 1.0) * Math.Exp(-x / 2.0);
            return res;
        }
        // т.е. исходный интеграл из теории от 0 до R0, то можно сократить интеграл до аргумента R0
        private double I(int n) //интеграл методом трапеций суммированием от 1 до n
        {
            double summ = 0;

            for (int k = 1; k <= n; k++)
                summ += (f(R0 * (((double)k - 1.0) / (double)n)) + f(R0 * ((double)k / (double)n))) * (R0 / (2.0 * (double)n));

            return summ;
        }

        private double CalcGammaFunc(double p, int n)
        {
            double summ = 0;
            double znam = p;
            for (int i = 1; i < n; i++)
            {
                znam *= p + i;
            }
            summ = Math.Pow(n, p) * factorial(n-1);
            return summ;
        }
    }
}


/*
 
 if (k_intervals == 2) // делим на два если
            {
                // сначала частоты считаем
                intervals[1].left = intervals[0].right = work_yi[size_division].Nu;
                double summ = 0;
                for (int i = 0; i < size_division; i++)
                    summ += (double)work_yi[i].kek / (double)kek_all;
                intervals[0].n = summ; summ = 0;
                for (int i = size_division; i < work_yi.Length; i++)
                    summ += (double)work_yi[i].kek / (double)kek_all;
                intervals[1].n = summ;
                // вероятности
                summ = 0;
                for (int i = 1; i < intervals[0].right; i++)
                {
                    summ += (real_Fn[i].p - real_Fn[i - 1].p);
                }
                intervals[0].p = summ; summ = 0;
                for (int i = intervals[1].left; i < real_Fn.Length - 1; i++)
                {
                    summ += (real_Fn[i].p - real_Fn[i - 1].p);
                }
                intervals[1].p = summ;
                for (int i = 0; i < size_division; i++)
                    intervals[0].count += work_yi[i].kek;
                for (int i = size_division; i < work_yi.Length; i++)
                    intervals[1].count += work_yi[i].kek;
            }
            else
            {
                //if(bad) // если деление нецелочисленное
                //{
                intervals[0].left = -1000;
                intervals[0].right = work_yi[size_division - 1].Nu;
                for (int i = 0; i < size_division; i++)
                {
                    intervals[0].n += (double)work_yi[i].kek / (double)kek_all;
                    intervals[0].count += work_yi[i].kek;
                }

                for (int i = 1; i < intervals[0].right; i++)
                    intervals[0].p += (real_Fn[i].p - real_Fn[i - 1].p);
                for (int i = 1; i <= k_intervals; i++)
                {
                    intervals[i].left = intervals[i - 1].right;
                    if (i == k_intervals - 1 && !bad)
                        intervals[i].right = work_yi[(i + 1) * size_division - 1].Nu;
                    else intervals[i].right = work_yi[(i + 1) * size_division - 1].Nu;
                    //if(bad)
                    //{
                    //    int ka = i * size_division + 1;
                    //    int target = (i + 1) * size_division;
                    //    while (intervals[i].right <= work_yi[target].Nu)
                    //    {
                    //        intervals[i].n = (double)work_yi[ka].kek / (double)kek_all;
                    //        intervals[i].count += work_yi[ka].kek;
                    //        ka++;
                    //    }

                    //}
                    for (int j = (i * size_division); j < (i + 1) * size_division; j++)
                    {
                        if (j == work_yi.Length) break;
                        intervals[i].n += (double)work_yi[j+1].kek / (double)kek_all;
                        intervals[i].count += work_yi[j+1].kek;
                    }
                    if (i == k_intervals - 1 && bad)
                    {
                        intervals[i].n += (double)work_yi[work_yi.Length - 1].kek / (double)kek_all;
                        intervals[i].count += work_yi[work_yi.Length - 1].kek;
                    }
                    for (int j = intervals[i].left + 1; j <= intervals[i].right; j++)
                    {
                        intervals[i].p += (real_Fn[j].p - real_Fn[j - 1].p);
                    }
                    if (i == k_intervals - 1 && bad)
                    {
                        int j = intervals[i].left;
                        while (j != real_Fn[intervals[i].right].right)
                        {
                            intervals[i].p += (real_Fn[j].p - real_Fn[j - 1].p);
                            j++;
                        }
                    }
                }
                intervals[k_intervals - 1].right = 1000;
            }     
     
*/
