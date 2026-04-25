using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Китайский_почтальон
{
    public partial class Form1 : Form
    {
        private List<Edge> edges = new List<Edge>();

        // Элементы управления
        private NumericUpDown numNodes;
        private TextBox txtFrom;
        private TextBox txtTo;
        private TextBox txtWeight;
        private ListBox lstEdges;
        private RichTextBox rtbResult;

        public Form1()
        {
            // Устанавливаем базовый шрифт
            this.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SetupFixedUI();
        }

        private void SetupFixedUI()
        {
            this.SuspendLayout();

            this.Text = "Решение задачи о китайском почтальоне";
            this.Size = new Size(1000, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // --- ПАНЕЛЬ ВВОДА ---
            GroupBox gbInput = new GroupBox()
            {
                Text = "Ввод параметров графа",
                Left = 20,
                Top = 20,
                Width = 940,
                Height = 200,
                Padding = new Padding(10)
            };

            // Строка 1: Количество вершин
            Label lblNodes = new Label()
            {
                Text = "1. Укажите общее кол-во вершин:",
                Left = 20,
                Top = 45,
                AutoSize = true
            };
            numNodes = new NumericUpDown()
            {
                Left = 430,
                Top = 42,
                Width = 80,
                Minimum = 2,
                Value = 4
            };

            // Строка 2: Инструкция
            Label lblEdgeHint = new Label()
            {
                Text = "2. Добавьте рёбра (соединения между вершинами):",
                Left = 20,
                Top = 90,
                AutoSize = true,
            };

            // Строка 3: Поля ввода ребер
            Label lblFrom = new Label() { Text = "От узла:", Left = 20, Top = 140, AutoSize = true };
            txtFrom = new TextBox() { Left = 120, Top = 137, Width = 50 };

            Label lblTo = new Label() { Text = "До узла:", Left = 180, Top = 140, AutoSize = true };
            txtTo = new TextBox() { Left = 285, Top = 137, Width = 60 };

            Label lblW = new Label() { Text = "Вес:", Left = 350, Top = 140, AutoSize = true };
            txtWeight = new TextBox() { Left = 405, Top = 137, Width = 60 };

            // ИСПРАВЛЕНА ВЫСОТА И ПОЗИЦИЯ
            Button btnAdd = new Button()
            {
                Text = "ДОБАВИТЬ",
                Left = 500,
                Top = 130,
                Width = 160,
                Height = 45,
                BackColor = Color.LightSkyBlue,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.Click += (s, e) => AddEdge();

            // ИСПРАВЛЕНЫ ПОЗИЦИИ ТЕСТОВ (чтобы не налезали на синюю кнопку)
            Button btnTestEuler = new Button()
            {
                Text = "ТЕСТ 1",
                Left = 680,
                Top = 130,
                Width = 110,
                Height = 45,
                BackColor = Color.Silver,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            };
            btnTestEuler.Click += (s, e) => RunTestEuler();

            Button btnTestPostman = new Button()
            {
                Text = "ТЕСТ 2",
                Left = 800,
                Top = 130,
                Width = 110,
                Height = 45,
                BackColor = Color.Silver,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            };
            btnTestPostman.Click += (s, e) => RunTestPostman();

            gbInput.Controls.AddRange(new Control[] {
        lblNodes, numNodes, lblEdgeHint, lblFrom, txtFrom,
        lblTo, txtTo, lblW, txtWeight, btnAdd, btnTestEuler, btnTestPostman
    });

            // --- СПИСОК РЕБЕР ---
            Label lblList = new Label()
            {
                Text = "Список добавленных путей:",
                Left = 25,
                Top = 235,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            lstEdges = new ListBox()
            {
                Left = 20,
                Top = 270,
                Width = 400,
                Height = 390,
                BorderStyle = BorderStyle.FixedSingle
            };

            // --- РЕЗУЛЬТАТ ---
            Label lblRes = new Label()
            {
                Text = "Результат расчёта:",
                Left = 455,
                Top = 235,
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            rtbResult = new RichTextBox()
            {
                Left = 450,
                Top = 270,
                Width = 510,
                Height = 380,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 13F)
            };

            // --- КНОПКИ ДЕЙСТВИЯ ---
            Button btnSolve = new Button()
            {
                Text = "ВЫЧИСЛИТЬ КРАТЧАЙШИЙ МАРШРУТ",
                Left = 20,
                Top = 670,
                Width = 400,
                Height = 80,
                BackColor = Color.PaleGreen,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSolve.Click += (s, e) => Solve();

            Button btnClear = new Button()
            {
                Text = "СБРОСИТЬ ВСЁ",
                Left = 450,
                Top = 670,
                Width = 250,
                Height = 80,
                BackColor = Color.LightCoral,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.Click += (s, e) => { edges.Clear(); lstEdges.Items.Clear(); rtbResult.Clear(); };

            this.Controls.AddRange(new Control[] {
        gbInput, lblList, lstEdges, lblRes, rtbResult, btnSolve, btnClear
    });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ClearAll() { edges.Clear(); lstEdges.Items.Clear(); rtbResult.Clear(); }

        // Помощник для программного добавления ребра
        private void AddEdgeManual(int f, int t, int w)
        {
            edges.Add(new Edge(f, t, w));
            lstEdges.Items.Add($" [{f}] <---> [{t}] | Вес: {w}");
        }

        private void RunTestEuler()
        {
            ClearAll();
            numNodes.Value = 4;
            AddEdgeManual(1, 2, 10);
            AddEdgeManual(2, 3, 10);
            AddEdgeManual(3, 4, 10);
            AddEdgeManual(4, 1, 10);
            rtbResult.AppendText("Загружен Тест 1 (Квадрат): все вершины чётные (степень 2).\nНажмите 'Вычислить' для проверки.");
        }

        private void RunTestPostman()
        {
            ClearAll();
            numNodes.Value = 4;
            AddEdgeManual(1, 2, 10);
            AddEdgeManual(2, 3, 10);
            AddEdgeManual(3, 1, 10);
            AddEdgeManual(3, 4, 5);
            rtbResult.AppendText("Загружен Тест 2 (Треугольник с хвостом): узлы 3 и 4 нечётные.\nНажмите 'Вычислить' для проверки.");
        }

        private void AddEdge()
        {
            if (int.TryParse(txtFrom.Text, out int from) &&
                int.TryParse(txtTo.Text, out int to) &&
                int.TryParse(txtWeight.Text, out int weight))
            {
                edges.Add(new Edge(from, to, weight));
                lstEdges.Items.Add($" [{from}] <---> [{to}] | Вес: {weight}");
                txtFrom.Clear(); txtTo.Clear(); txtWeight.Clear();
                txtFrom.Focus();
            }
            else MessageBox.Show("Ошибка: Введите номера узлов и вес цифрами!");
        }

        private void Solve()
        {
            int n = (int)numNodes.Value;
            rtbResult.Clear();

            if (edges.Count == 0)
            {
                rtbResult.SelectionColor = Color.Red;
                rtbResult.AppendText("Ошибка: Сначала постройте граф (добавьте рёбра)!");
                return;
            }

            int[] degree = new int[n + 1];
            foreach (var edge in edges)
            {
                if (edge.U > n || edge.V > n)
                {
                    MessageBox.Show($"Ошибка: Введён узел {Math.Max(edge.U, edge.V)}, но в графе всего {n} вершин!");
                    return;
                }
                degree[edge.U]++;
                degree[edge.V]++;
            }

            for (int i = 1; i <= n; i++)
            {
                if (degree[i] == 0)
                {
                    MessageBox.Show($"Ошибка: Вершина {i} не задействована. Вы указали общее количество вершин {n}, но не использовали все из них в построении графа!");
                    return;
                }
            }

            List<int> oddNodes = new List<int>();
            for (int i = 1; i <= n; i++)
                if (degree[i] % 2 != 0) oddNodes.Add(i);

            int totalWeight = edges.Sum(e => e.Weight);

            rtbResult.AppendText($" Сумма весов всех рёбер: {totalWeight}\n");
            rtbResult.AppendText($" ----------------------------------\n");

            if (oddNodes.Count == 0)
            {
                rtbResult.SelectionColor = Color.DarkGreen;
                rtbResult.AppendText(" Все вершины чётные.\n Граф содержит Эйлеров цикл.\n");
                rtbResult.AppendText($" Минимальный маршрут равен сумме рёбер: {totalWeight}\n");
            }
            else
            {
                rtbResult.AppendText($" Найдено нечётных вершин: {oddNodes.Count}\n");
                rtbResult.AppendText($" (Узлы: {string.Join(", ", oddNodes)})\n\n");

                int[,] dist = CalculateDistances(n);
                int additionalWeight = FindMinMatching(oddNodes, dist);

                if (additionalWeight >= 1000000)
                {
                    rtbResult.SelectionColor = Color.Red;
                    rtbResult.AppendText(" Ошибка: Граф не является связным!\n Невозможно пройти по всем ребрам.");
                }
                else
                {
                    rtbResult.AppendText($" Добавочное расстояние для\n обеспечения чётности: {additionalWeight}\n");
                    rtbResult.AppendText($" ----------------------------------\n");
                    rtbResult.SelectionFont = new Font("Segoe UI", 16F, FontStyle.Bold);
                    rtbResult.SelectionColor = Color.DarkBlue;
                    rtbResult.AppendText($" ОБЩИЙ ПУТЬ: {totalWeight + additionalWeight}");
                }
            }
        }

        private int[,] CalculateDistances(int n)
        {
            int[,] dist = new int[n + 1, n + 1];
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    dist[i, j] = (i == j) ? 0 : 1000000;

            foreach (var e in edges)
            {
                if (e.Weight < dist[e.U, e.V])
                {
                    dist[e.U, e.V] = e.Weight;
                    dist[e.V, e.U] = e.Weight;
                }
            }

            for (int k = 1; k <= n; k++)
                for (int i = 1; i <= n; i++)
                    for (int j = 1; j <= n; j++)
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];
            return dist;
        }

        private int FindMinMatching(List<int> oddNodes, int[,] dists)
        {
            return RecursiveMatch(0, new bool[oddNodes.Count], oddNodes, dists);
        }

        private int RecursiveMatch(int index, bool[] used, List<int> oddNodes, int[,] dists)
        {
            if (index == oddNodes.Count) return 0;
            if (used[index]) return RecursiveMatch(index + 1, used, oddNodes, dists);

            int res = 1000000;
            used[index] = true;
            for (int i = index + 1; i < oddNodes.Count; i++)
            {
                if (!used[i])
                {
                    used[i] = true;
                    int cost = dists[oddNodes[index], oddNodes[i]] + RecursiveMatch(index + 1, used, oddNodes, dists);
                    if (cost < res) res = cost;
                    used[i] = false;
                }
            }
            used[index] = false;
            return res;
        }
    }

    public class Edge
    {
        public int U, V, Weight;
        public Edge(int u, int v, int w) { U = u; V = v; Weight = w; }
    }
}