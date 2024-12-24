using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectGenetics
{
    public class GA //Класс, описывающий генетический алгоритм
    {
        List<Genome> population = new List<Genome>(); // популяция
        int populationLimit; // максимально допустимый размер популяции 
        double mutationChance = 0.7; // шанс мутации
        Point start; // Координаты начала пути
        Point finish; // Координаты конца пути
        Random rng = new Random();

        void sortByFitness() //вычисление приспособленности популяции
        {
            foreach (Genome g in population) // вычсиление приспособлености каждой хромосомы
            {
                if (g.fitness == 0)
                    g.calculateFitness(start, finish);
            }

            population.Sort((a, b) => (a.fitness.CompareTo(b.fitness))); // сортировка популяции по приспособленности
        }

        public double getBestFitness() //получение лучшей приспособленности
        {
            return population[0].fitness;
        }

        public void setPopulation(List<List<Point>> set, Point start, Point finish) //иницилизации популяции, начала и конца
        {
            foreach (List<Point> path in set)
                population.Add(new Genome(path));

            this.start = start;
            this.finish = finish;

            populationLimit = set.Count;

            sortByFitness();    // вычисление приспособленности начальной популяции
        }

        List<Genome> parentsSelection() //выбор родителей для скрещивания (турнирный способ)
        {
            List<Genome> parents = new List<Genome>();

            for (int i = 0; i < population.Count / 10 + 2; i++)
            {
                int ind = rng.Next(population.Count);
                if (parents.Contains(population[ind]))
                    i--;
                else
                    parents.Add(population[ind]);
            }

            parents.Sort((a, b) => (a.fitness.CompareTo(b.fitness)));
            return parents;
        }

        void crossover(List<Genome> parents) //скрещивание (рекомбинация по однйо точке)
        {
            List<Point> points1 = new List<Point>();
            List<Point> points2 = new List<Point>();

            int point = rng.Next(2, parents[0].genes.Count - 2); //parents[0].genes.Count / 2;

            for (int i = 0; i < parents[0].genes.Count; i++)
            {
                if (i < point)
                {
                    points1.Add(parents[0].genes[i]);
                    points2.Add(parents[1].genes[i]);
                }
                else
                {
                    points1.Add(parents[1].genes[i]);
                    points2.Add(parents[0].genes[i]);
                }
            }
            population.Add(new Genome(points1));
            population.Add(new Genome(points2));
        }

        public void nextGeneration() //смена поколения
        {
            int cross = rng.Next(1, population.Count / 2); //определение числа скрещиваний в поколении
            for (int i = 0; i < cross; i++)
                crossover(parentsSelection());          // выбор родителей и скрещивание

            for (int i = 0; i < population.Count; i++)
                if (rng.NextDouble() <= mutationChance)
                    population.Add(population[i].mutate(rng)); // мутация с определённым шансом

            sortByFitness();        // оценка приспособленности добавленных хромосом

            population.RemoveRange(populationLimit, population.Count - populationLimit); //сокращение поплуяции (основанное из приспособленности)
        }

        public List<List<Point>> getPopulation() // получение популяции в виде списка списков точек
        {
            List<List<Point>> set = new List<List<Point>>();

            foreach (Genome g in population)
            {
                set.Add(g.genes);
            }
            return set;
        }
    }
}
