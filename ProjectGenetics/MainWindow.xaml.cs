using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjectGenetics
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start = new Point(150, 150); //начало пути
        Point finish = new Point(550, 150); // конец пути 
        double radius = 30; // радиус отображаемых точек начала и конца маршрута
        int waypoints = 9; // число точек маршрута
        int populationSize = 11; // размер популяции

        GA gen = new GA(); // генетический алгоритм
        int generation = 0; // счётчик поколений

        DispatcherTimer timer = new DispatcherTimer();
        Random rng = new Random();
        double optimalLength = 0; // кратчайший путь между точками


        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += Timer_Tick;
        }

        void drawEllipse(Point p, double r) //рисование точки в позиции p с радиусом r
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.Width = r;
            myEllipse.Height = r;
            myEllipse.RenderTransform = new TranslateTransform(p.X, p.Y);
            scene.Children.Add(myEllipse); //scene - объект типа Canvas
        }

        void drawLine(Point p1, Point p2, double r, bool shortest) // рисование отрезка из точки p1 в точку p2
        {
            Line myLine = new Line();
            if (shortest) // если треубется нарисовать кратчайший путь
                myLine.Stroke = Brushes.Black;
            else
                myLine.Stroke = Brushes.LightGray;

            myLine.X1 = p1.X + r / 2;
            myLine.Y1 = p1.Y + r / 2;
            myLine.X2 = p2.X + r / 2;
            myLine.Y2 = p2.Y + r / 2;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 1;
            scene.Children.Add(myLine);
        }

        void drawPath(List<Point> path, bool thic) // рисование маршрута
        {
            drawEllipse(start, radius); // рисование точки начало
            drawEllipse(finish, radius); // рисование точки конец
            drawLine(start, path[0], radius, thic); //рисование отрезка между началом и первой точкой маршрута

            for (int i = 0; i < path.Count - 1; i++)
            {
                drawLine(path[i], path[i+1], radius, thic); // рисование маршрута до последней точки
            }
            drawLine(path[path.Count - 1], finish, radius, thic); // рисование отрезка между последней точкой машрута и его концом
        }

        List<Point> randomPath(double maxX, double maxY, int len) // метод генерации случайного маршрута
        {
            List<Point> path = new List<Point>();
            for (int i = 0; i < len; i++)
                path.Add(new Point(rng.NextDouble() * (maxX - radius) + radius, rng.NextDouble() * (maxY - radius) + radius));
            return path;   
        }

        //void drawSet(List<List<Point>> population)
        //{
        //    scene.Children.Clear(); // Очищаем Canvas перед новым рисованием

        //    // Рисуем каждый маршрут из популяции
        //    foreach (var path in population)
        //    {
        //        drawPath(path, false); // Отображаем маршрут с тонкими линиями
        //    }

        //    //// Рисуем лучший маршрут
        //    drawPath(population[0], true); // Отображаем лучший маршрут с толстыми линиями
        //}


        private void Timer_Tick(object sender, EventArgs e)
        {
            generation++;
            LGen.Content = generation.ToString();
            gen.nextGeneration(); // смена поколения
            /*drawSet(gen.getPopulation());*/ //рисование текущего поколения 

            LLen.Content = (gen.getBestFitness() / optimalLength).ToString("0.00") + "%";
            if ((gen.getBestFitness() / optimalLength) <= 100.01) //остановка алгоритма при достижении заданной точности 
                timer.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //запуск алгоритм 
        {
            optimalLength = Point.Subtract(finish, start).Length / 100.0; // вычисление кратчайшего пути

            List<List<Point>> set = new List<List<Point>>();

            for(int i = 0; i < populationSize; i++)  // создание начальной популяции
                set.Add(randomPath(scene.Width - radius, scene.Height - radius, waypoints));

            gen.setPopulation(set, start, finish); // инициализация данных для генетического алгоритма

            timer.Start(); //запуск смены поколений
        }
    }
}
