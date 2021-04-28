using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
namespace Cal
{
    public partial class MainWindow : Window
    {
        string primer = "";
        Dictionary<string, int> prioryti = new Dictionary<string, int> { { "+", 1 }, { "-", 1 }, { "*", 2 }, { "/", 2 }, { "^", 3 }, { "√", 3 }, { "(", -1 }, { ")", -1 }, { "!", 3 } };
        Dictionary<string, int> arrayNumAbc = new Dictionary<string, int> {
            { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 }, { "0", 0 },
            { "A", 10 }, { "B", 11 }, { "C", 12 }, { "D", 13 }, { "E", 14 } , { "F", 15 }
        };
        string[] arrayNum = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ",", "A", "B", "C", "D", "E", "F" };
        string[] arrayOperathions = { "+", "-", "*", "/", "^", "√", ")", "(", "!" };
        static string[] arrayTrigon = { "sin", "cos", "tg(", "ctg", "ln(", "log", "|x|" };
        public MainWindow()
        {
            InitializeComponent();
        }
        //Прочее
        private void Grid_Mouse(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //Удаление символов строчек или всего содержимого
        private void Button_Del_Char(object sender, RoutedEventArgs e)
        {
            try { primer = primer.Remove(primer.Length - 1, 1); } catch (Exception) { }
            label10.Text = primer;
        }
        private void Button_Click_FullClear(object sender, RoutedEventArgs e)
        {
            primer = "";
            label10.Text = primer;
        }
        //Нажата кнопка 
        private void Butt_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            primer += b.Content;
            label10.Text = primer;

        }
        private void Button_Click_StartOperation(object sender, RoutedEventArgs e)
        {
            primer = Reshenie(primer);
            label10.Text = primer;
        }




        private string getEl(string str, int i)
        {
            string el = "", elt = "";
            bool trig = false;
            for (; i < str.Length; i++)
            {
                if (!trig)
                {
                    if (arrayOperathions.Any(str2 => str2 == str[i].ToString()))
                    {
                        if (el.Length > 0) return el;
                        else return str[i].ToString();
                    }
                    else if (arrayNum.Any(str2 => str2 == str[i].ToString()))
                        el += str[i];
                    else
                        trig = true;
                }
                if (trig)
                {
                    elt += str[i];
                    if (str[i] == 'π') return Convert.ToString(Math.PI);
                    if (str[i] == 'e') return Convert.ToString(Math.E);
                    if (str[i] == ')') return elt;
                    if (el.Length > 0) return el;
                }
            }
            return el;
        }

        public string getPrimerInSkobki(string trigon)
        {
            string primer = ""; bool start = false;
            for (int i = 0; i < trigon.Length; i++)
            {
                if (trigon[i] == '(')
                    start = true;
                if (start) primer += trigon[i];
            }
            primer = primer.Substring(1, primer.Length - 2);
            return primer;
        }
        private string Reshenie(string primer)
        {
            Stack<String> stackNum = new Stack<string>();
            Stack<String> stackOperation = new Stack<string>();
            if (primer.Length > 0)
            {
                try
                {
                    for (int i = 0; i < primer.Length; i++)
                    {
                        string el = getEl(primer, i);
                        i = i + el.Length - 1;
                        if (arrayNum.Any(str2 => str2 == el[0].ToString()))
                            stackNum.Push(el);
                        /*if (checkZ(el))
                            stackNum.Push(el);
                        else
                            stackNum.Push(getDes(el));*/
                        else
                        {
                            if (el.Length >= 3 && arrayTrigon.Any(str2 => str2 == el.Substring(0, 3)))
                            {
                                string pp = getPrimerInSkobki(el);
                                string res = Reshenie(pp);
                                double p = Convert.ToDouble(res);
                                stackNum.Push(mathAction(p, el.Substring(0, 3)).ToString());
                            }
                            else if (stackOperation.Count > 0)
                            {
                                string lastOp = stackOperation.Peek();
                                if (el.Equals("(") || lastOp.Equals("("))
                                {
                                    stackOperation.Push(el);
                                    continue;
                                }
                                if (el.Equals(")"))
                                {
                                    while (!lastOp.Equals("("))
                                    {
                                        TwoLastPoschitazz(stackNum, stackOperation);
                                        if (stackOperation.Count > 0)
                                            lastOp = stackOperation.Peek();
                                        else break;
                                    }
                                    stackOperation.Pop();
                                    continue;
                                }
                                if (prioryti[lastOp] < prioryti[el])
                                    stackOperation.Push(el);
                                else
                                {
                                    while (prioryti[lastOp] >= prioryti[el])
                                    {
                                        TwoLastPoschitazz(stackNum, stackOperation);
                                        if (stackOperation.Count > 0)
                                            lastOp = stackOperation.Peek();
                                        else break;
                                    }
                                    stackOperation.Push(el);
                                }
                            }
                            else
                                stackOperation.Push(el);
                        }
                    }
                    while (stackOperation.Count > 0)
                        TwoLastPoschitazz(stackNum, stackOperation);
                    return stackNum.Pop();
                }
                catch (Exception e) { }
            }
            return "Error";
        }
        public void TwoLastPoschitazz(Stack<string> stackNum, Stack<string> stackOperation)
        {
            double x = Convert.ToDouble(stackNum.Pop()), y = 0;
            if (stackOperation.Peek().Equals("!"))
                stackNum.Push(mathAction(x, stackOperation.Pop()).ToString());
            else
            {
                try { y = Convert.ToDouble(stackNum.Pop()); } catch (Exception e) { }
                stackNum.Push(mathAction(y, x, stackOperation.Pop()).ToString());
            }
        }
        private double mathAction(double x, double y, string op)
        {
            switch (op)
            {
                case "+":
                    return x + y;
                    break;
                case "-":
                    return x - y;
                    break;
                case "*":
                    return x * y;
                    break;
                case "/":
                    return x / y;
                    break;
                case "^":
                    return Math.Pow(x, y);
                    break;
                case "√":
                    return Math.Pow(x, 1 / y);
                    break;
            }
            return 0;
        }
        private double mathAction(double x, string op)
        {
            try
            {
                switch (op)
                {
                    case "cos":
                        return Math.Cos(x);
                        break;
                    case "sin":
                        return Math.Sin(x);
                        break;
                    case "tg(":
                        return Math.Tan(x);
                        break;
                    case "ctg":
                        return 1 / Math.Tan(x);
                        break;
                    case "!":
                        int fuck = Convert.ToInt32(x);
                        return Factorial(fuck);
                        break;
                    case "ln(":
                        return Math.Log(x);
                        break;
                    case "log":
                        return Math.Log10(x);
                        break;
                }
            }
            catch (Exception ee) { }
            
            return 0;
        }

        static double Factorial(double x)
        {
            if(x < 10000000)
            {
                double sum = 1;
                for (int i = 1; i <= x; i++)
                    sum *= i;
                return sum;
            }
            else
            {
                Random r = new Random();
                
                return r.Next(100, 1000) ^ r.Next(100, 1000);
            }
        }

       /* 
        private void ShowCloseMenu(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = sp.ActualWidth;
            if (sp.ActualWidth == 0)
                animation.To = 210;
            else
                animation.To = 0;
            animation.AccelerationRatio = 1;
            animation.Duration = TimeSpan.FromSeconds(0.1);
            sp.BeginAnimation(StackPanel.WidthProperty, animation);
        }*/
        //Показать или не показать окошки
        private void ShowJustCal(object sender, RoutedEventArgs e)
        {

            label2.Visibility = Visibility.Hidden;
            label8.Visibility = Visibility.Hidden;
            label10.Height = 120;
            label16.Visibility = Visibility.Hidden;
            GridInfo.Width = 0;


            IngeneerMathOp.Visibility = Visibility.Collapsed;
            ProgrammerMathOp.Visibility = Visibility.Collapsed;
            ProgramerTools.Visibility = Visibility.Collapsed;
            MathTools.Visibility = Visibility.Visible;
            titleCal.Content = "Обычный";
           // ShowCloseMenu(null, null);
        }
        private void ShowIngCal(object sender, RoutedEventArgs e)
        {

            label2.Visibility = Visibility.Hidden;
            label8.Visibility = Visibility.Hidden;
            label10.Height = 120;
            label16.Visibility = Visibility.Hidden;
            GridInfo.Width = 0;



            IngeneerMathOp.Visibility = Visibility.Visible;
            ProgrammerMathOp.Visibility = Visibility.Collapsed;
            ProgramerTools.Visibility = Visibility.Collapsed;
            MathTools.Visibility = Visibility.Visible;
            titleCal.Content = "Инженерный";
           // ShowCloseMenu(null, null);
        }
        private void ShowProgCal(object sender, RoutedEventArgs e)
        {
            label2.Visibility = Visibility.Visible;
            label8.Visibility = Visibility.Visible;
            label10.Height = 30;
            label16.Visibility = Visibility.Visible;
            GridInfo.Width = 30;




            IngeneerMathOp.Visibility = Visibility.Collapsed;
            ProgrammerMathOp.Visibility = Visibility.Visible;
            ProgramerTools.Visibility = Visibility.Visible;
            MathTools.Visibility = Visibility.Collapsed;
            titleCal.Content = "Програмист";
           // ShowCloseMenu(null, null);
        }
        private void Butt_Modul_Click(object sender, RoutedEventArgs e)
        {
            primer = Reshenie(primer);
            if (primer.Length > 0 && primer[0] == '-') primer = primer.Remove(0, 1);
            label10.Text = primer;
        }
        private void _q_Click(object sender, RoutedEventArgs e)
        {
            if (primer.Length > 0 && primer[0] == '-') primer = primer.Remove(0, 1); else primer = "-" + primer;
            label10.Text = primer;
        }
        private void _z_Click(object sender, RoutedEventArgs e)
        {
            if (!checkZ(primer))
                if (primer.Length > 0) primer += ","; else primer = "0,";
            label10.Text = primer;
        }
        private bool checkZ(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == ',')
                    return true;
            return false;
        }
        private void _Programer_Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            label10.Text = Convert.ToString(Convert.ToInt32(Reshenie(primer)), Convert.ToInt32(b.Content));
        }
        private string getDes(string shestnadzat)
        {
            double itog = 0; int pow = shestnadzat.Length - 1;
            for (int i = 0; i < shestnadzat.Length; i++, pow--)
                itog += (Convert.ToInt32(arrayNumAbc[shestnadzat[i].ToString()]) * Math.Pow(16, pow));
            return itog.ToString();
        }

        private void label10_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            switch (tb.Name)
            {
                case "label10":
                    {
                        int i = Convert.ToInt32(tb.Text);
                        label2.Text = Convert.ToString(i, 2);
                        label8.Text = Convert.ToString(i, 8);
                        label10.Text = Convert.ToString(i, 10);
                        label16.Text = Convert.ToString(i, 16);
                    }
                    break;
                case "label2":
                    {

                        break;
                    }
                case "label8":
                    {

                        break;
                    }
                case "label16":
                    {

                        break;
                    }
            }
        }

          
    }
}
