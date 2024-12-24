using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectGenetics
{
    public class Genome //класс, описыващий набор генов (хромосому)
    {
        public List<Point> genes; //В данной задаче ген - набор точек в 2D пространстве
        public double fitness = 0; //значение приспособленности (меньше - лучше)

        public Genome(List<Point> path)
        {
            genes = path;
        }
        public Genome mutate(Random rng) // Функция мутации хромосомы (мутация значения)
        {
            List<Point> path = new List<Point>(genes);

            int ind = rng.Next(0, path.Count); //Случайно выбранная точка из списка
            double x = path[ind].X;
            double y = path[ind].Y;

            x += rng.NextDouble() * (100) - 50; //смещается на случайное значение в диапазоне от -50 до 50 
            y += rng.NextDouble() * (100) - 50; //по X и по Y

            path[ind] = new Point(x, y);
            return new Genome(path);
        }

        public void calculateFitness(Point start, Point finish) //Вычисление приспособленности хромосомы
        {
            double distance = 0;
            distance += Point.Subtract(genes[0], start).Length;

            for (int i = 0; i < genes.Count - 1; i++)
            {
                distance += Point.Subtract(genes[i + 1], genes[i]).Length; // вычисления длины пути
            }

            distance += Point.Subtract(finish, genes[genes.Count - 1]).Length;

            fitness = distance;
        }
    }
}
