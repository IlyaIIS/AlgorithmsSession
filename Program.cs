using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlgorithmsSession
{
    class Program
    {
        static void Main()
        {
            Tree tree = new Tree();
            for (int i = 0; i < 4; i++)
                tree.AddNode();

            /*tree.AddLink(1, 1, 0, 1);
            tree.AddLink(5,5 ,1 ,2 );
            tree.AddLink(6,6 ,1 ,4 );
            tree.AddLink(7,7 ,2 ,5 );
            tree.AddLink(3,3 ,3 ,4 );
            tree.AddLink(4,4 ,4 ,5 );
            tree.AddLink(9,9 ,5 ,8 );
            tree.AddLink(8,8 ,4 ,7 );
            tree.AddLink(11,11 ,3 ,6 );
            tree.AddLink(10,10 ,6 ,7 );
            tree.AddLink(2, 2,7 ,8 );*/

            
            tree.AddLink(2,1,0,2);
            tree.AddLink(3,1,2,1);
            tree.AddLink(1, 10, 0, 1);
            tree.AddLink(4,1,1,3);

            List<double> links = tree.BreadthWaySearch(0);
            foreach (var node in links)
                Console.Write(node + " ");
        }

        static void DoSort()
        {
            Random rnd = new Random();

            int[] array = new int[1000];

            for (int i = 0; i < array.Length; i++)
                array[i] = rnd.Next(100);

            foreach (int el in array)
                Console.Write(el + " ");

            DoBiderectionalSort(ref array);

            Console.WriteLine("\n");
            foreach (int el in array)
                Console.Write(el + " ");
        }
        //Всё с графом
        #region
        class Tree
        {
            List<Node> Nodes = new List<Node>();
            List<Link> Links = new List<Link>();
            public void AddNode()
            {
                Nodes.Add(new Node(Nodes.Count));
            }
            public void AddLink(int id, double value, int nodeId1, int nodeId2)
            {
                Link link = new Link(id,value, Nodes[nodeId1], Nodes[nodeId2]);
                Links.Add(link);
                Nodes[nodeId1].Links.Add(link);
                Nodes[nodeId2].Links.Add(link);
            }
            public List<Link> PrimsTreeSearch()
            {
                List<Link> neededLinks = new List<Link>();
                List<Node> passedNodes = new List<Node>() { Nodes[0]};

                while (neededLinks.Count != Nodes.Count - 1)
                {
                    Link link = GetMinLink();
                    link.Flag = 1;

                    if (passedNodes.Contains(link.Node[0]))
                    {
                        if (!passedNodes.Contains(link.Node[1]))
                        {
                            neededLinks.Add(link);
                            passedNodes.Add(link.Node[1]);
                        }
                    }
                    else
                    {
                        neededLinks.Add(link);
                        passedNodes.Add(link.Node[0]);
                    }
                }

                return neededLinks;

                Link GetMinLink()
                {
                    Link minLink = new Link(-1, double.MaxValue, Nodes[0], Nodes[0]);
                    foreach (Node node in passedNodes)
                        foreach (Link link in node.Links)
                            if (link.Value < minLink.Value && link.Flag == 0)
                                minLink = link;

                    return minLink;
                }
            }
            public List<Link> KraskalsTreeSearch()
            {
                List<Link> neededLinks = new List<Link>();
                int subTreeNum = 0;
                while (neededLinks.Count != Nodes.Count - 1)
                {
                    Link link = GetMinLink();
                    link.Flag = 1;

                    if (link.Node[0].Flag == 0 && link.Node[1].Flag == 0)
                    {
                        subTreeNum++;
                        link.Node[0].Flag = subTreeNum;
                        link.Node[1].Flag = subTreeNum;
                        neededLinks.Add(link);
                    }
                    else if (link.Node[0].Flag == link.Node[1].Flag)
                    {
                        //цикл
                    }
                    else if (link.Node[0].Flag == 0)
                    {
                        link.Node[0].Flag = link.Node[1].Flag;
                        neededLinks.Add(link);
                    }
                    else if (link.Node[1].Flag == 0)
                    {
                        link.Node[1].Flag = link.Node[0].Flag;
                        neededLinks.Add(link);
                    }
                    else
                    {
                        int flag = link.Node[0].Flag;
                        foreach (Node node in Nodes)
                            if (node.Flag == flag)
                                node.Flag = link.Node[1].Flag;
                        neededLinks.Add(link);
                    }
                }

                return neededLinks;

                Link GetMinLink()
                {
                    Link minLink = new Link(0, double.MaxValue, Nodes[0], Nodes[0]);
                    foreach (Link link in Links)
                        if (link.Value < minLink.Value && link.Flag == 0)
                            minLink = link;

                    return minLink;
                }
            }
            public List<Node> DeepFirstSearch(int nodeId)
            {
                Node node = Nodes[nodeId];
                List<Node> result = new List<Node>();

                RecursiveSearch(node);

                return result;

                void RecursiveSearch(Node node)
                {
                    node.Flag = 1;
                    result.Add(node);
                    foreach (Link link in node.Links)
                    {
                        Node other = link.Node[0] != node ? link.Node[0] : link.Node[1];
                        if (other.Flag == 0)
                            RecursiveSearch(other);
                    }
                }
            }
            public List<Node> BreadthFirstSearch(int nodeId)
            {
                List<Node> passedNodes = new List<Node>();
                List<Node> currentNodes = new List<Node>() { Nodes[nodeId] };

                while(currentNodes.Count > 0)
                {
                    List<Node> nextNodes = new List<Node>();
                    foreach (Node node in currentNodes)
                    {
                        foreach (Link link in node.Links)
                        {
                            Node other = link.Node[0] != node ? link.Node[0] : link.Node[1];
                            if (other.Flag == 0)
                            {
                                nextNodes.Add(other);
                                other.Flag = 2;
                            }
                        }
                        node.Flag = 1;
                        passedNodes.Add(node);
                    }
                    currentNodes = nextNodes;
                }

                return passedNodes;
            }
            public List<double> BreadthWaySearch(int nodeId)
            {
                List<Node> currentNodes = new List<Node>() { Nodes[nodeId]};

                foreach (Node node in Nodes)
                    node.WayLength = double.MaxValue;
                Nodes[nodeId].WayLength = 0;

                while(currentNodes.Count > 0)
                {
                    while(currentNodes.Count > 0)
                    {
                        Node node = GetCloserNode(currentNodes);

                        foreach(Link link in node.Links)
                        {
                            Node other = link.Node[0] != node ? link.Node[0] : link.Node[1];
                            if (other.Flag != 2 && other.WayLength > node.WayLength + link.Value)
                            {
                                other.WayLength = node.WayLength + link.Value;
                                other.Flag = 1;
                            }
                        }
                        node.Flag = 2;
                    }

                    currentNodes.Clear();
                    foreach(Node node in Nodes)
                        if (node.Flag == 1)
                            currentNodes.Add(node);
                }

                List<double> output = new List<double>();
                foreach (Node node in Nodes)
                    output.Add(node.WayLength);

                return output;

                Node GetCloserNode(List<Node> nodes)
                {
                    int id = 0;
                    Node closerNode = nodes[0];
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (nodes[i].WayLength < closerNode.WayLength)
                        {
                            closerNode = nodes[i];
                            id = i;
                        }
                    }

                    nodes.RemoveAt(id);
                    return closerNode;
                }
            }
        }
        class Node
        {
            public int Id { get; }
            public List<Link> Links { get; } = new List<Link>();
            public int Flag { get; set; }
            public double WayLength { get; set; }
            public Node(int id)
            {
                Id = id;
            }
        }
        class Link
        {
            public int Id { get; }
            public double Value { get; set; }
            public Node[] Node { get; }
            public int Flag { get; set; }
            public Link(int id, double value, Node firstNode, Node secondNode)
            {
                Id = id;
                Value = value;
                Node = new Node[2] { firstNode, secondNode };
            }
        }
        #endregion
        //Очередь и стек
        #region
        class Queue<T>
        {
            BilinkedList<T> list = new BilinkedList<T>();
            public void Enqueue(T item)
            {
                list.AddLast(item);
            }
            public T Dequeue()
            {
                T output = list.First.Value;
                list.RemoveFirst();
                return output;
            }
            public T Peek()
            {
                return list.First.Value;
            }
        }
        class Stack<T>
        {
            BilinkedList<T> list = new BilinkedList<T>();
            public void Push(T item)
            {
                list.AddLast(item);
            }
            public T Pop()
            {
                T output = list.Last.Value;
                list.RemoveLast();
                return output;
            }
            public T Peek()
            {
                return list.Last.Value;
            }
        }
        class BilinkedList<T>
        {
            public ListNode First { get; private set; }
            public ListNode Last { get; private set; }
            public void AddLast(T item)
            {
                ListNode node = new ListNode(item);
                if (Last != null)
                {
                    Last.Next = node;
                    node.Prev = Last;
                    Last = node;
                }
                else
                {
                    Last = node;
                    First = node;
                }
            }
            public void FirstLast(T item)
            {
                ListNode node = new ListNode(item);
                if (First != null)
                {
                    First.Prev = node;
                    node.Next = First;
                    First = node;
                }
                else
                {
                    Last = node;
                    First = node;
                }
            }
            public bool Remove(T item)
            {
                ListNode node = First;
                while(node != null)
                {
                    if (node.Value.Equals(item))
                    {
                        Remove(node);
                        return true;
                    }
                    node = node.Next;
                }
                return false;
            }
            public void Remove(ListNode node)
            {
                if (node == First)
                {
                    RemoveFirst();
                }
                else if (node == Last)
                {
                    RemoveLast();
                }
                else
                {
                    node.Next = node.Prev;
                    node.Prev = node.Next;
                }
            }
            public void RemoveFirst()
            {
                if (First != null)
                {
                    if (Last != First)
                    {
                        First.Next.Prev = null;
                        First = First.Next;
                    }
                    else
                    {
                        Last = null;
                        First = null;
                    }
                }

            }
            public void RemoveLast()
            {
                if (Last != null)
                {
                    if (Last != First)
                    {
                        Last.Prev.Next = null;
                        Last = Last.Prev;
                    }
                    else
                    {
                        Last = null;
                        First = null;
                    }
                }
            }

            public class ListNode
            {
                public T Value;
                public ListNode Prev;
                public ListNode Next;

                public ListNode(T value)
                {
                    Value = value;
                }
            }
        }
        #endregion
        static string[] ABCSort(string[] array)
        {
            int?[] indexes = new int?[array.Length];
            List<int?[]> level = new List<int?[]> { new int?[26]};
            List<string> result = new List<string>();

            for (int i = 0; i < array.Length; i++)
            {
                int letter = char.ToUpper(array[i][0]) - 65;
                indexes[i] = level[0][letter];
                level[0][letter] = i;
            }
            ClearLevel(0);

            void ClearLevel(int depth)
            {
                if (level.Count == depth + 1)
                    level.Add(new int?[26]);

                for (int i = 0; i < 26; i++)
                {
                    if (level[depth][i] != null)
                    {
                        int pos = level[depth][i].GetValueOrDefault();
                        if (indexes[pos] == null)
                        {
                            result.Add(array[pos]);
                            level[depth][i] = null;
                        }
                        else
                        {
                            MarkChain(pos, depth);
                            level[depth][i] = null;
                            ClearLevel(depth + 1);
                        }
                    }
                }
            }
            void MarkChain(int pos, int depth)
            {
                while (true)
                {
                    int? nextPos = indexes[pos];

                    if (depth + 1 >= array[pos].Length)
                    {
                        result.Add(array[pos]);
                        indexes[pos] = null;
                    }
                    else
                    {
                        int letter = char.ToUpper(array[pos][depth + 1]) - 65;
                        indexes[pos] = level[depth + 1][letter];
                        level[depth + 1][letter] = pos;
                    }
                    if (nextPos == null)
                        break;
                    else
                        pos = nextPos.GetValueOrDefault();
                }
            }

            return result.ToArray();
        }
        class HashOpenCollection
        {
            public int Size;
            private int?[] table;
            private bool[] wasElRemoved;
            public HashOpenCollection(int size)
            {
                Size = size;
                table = new int?[size];
                wasElRemoved = new bool[size];
            }
            public bool Add(int item)
            {
                for (int i = 0; i < Size; i++)
                {
                    int hash = GetHash(item, i);
                    if (wasElRemoved[i] || table[hash] == null)
                    {
                        wasElRemoved[i] = false;
                        table[hash] = item;
                        return true;
                    }
                }

                return false;
            }
            public bool Contains(int item)
            {
                for (int i = 0; i < Size; i++)
                {
                    int hash = GetHash(item, i);
                    if (wasElRemoved[hash])
                    {
                        continue;
                    }
                    else
                    {
                        if (table[hash] == item)
                            return true;
                        if (table[hash] == null)
                            return false;
                    }
                }
                return false;
            }
            public bool Remove(int item)
            {
                for (int i = 0; i < Size; i++)
                {
                    int hash = GetHash(item, i);
                    if (wasElRemoved[hash])
                    {
                        continue;
                    }
                    else
                    {
                        if (table[hash] == item)
                        {
                            wasElRemoved[hash] = true;
                            return true;
                        }
                        if (table[hash] == null)
                            return false;
                    }
                }

                return false;
            }
            private int GetHash(int item, int iterationNum)
            {
                return (item + iterationNum) % Size;
            }

        }
        class HashChainCollection
        {
            public int Size;
            private LinkedList<int>[] table;
            public HashChainCollection(int size)
            {
                Size = size;
                table = new LinkedList<int>[size];
            }
            public void Add(int item)
            {
                int hash = GetHash(item);

                if (table[hash] == null)
                    table[hash] = new LinkedList<int>();
                    
                table[hash].AddLast(item);
            }
            public bool Contains(int item)
            {
                int hash = GetHash(item);

                if (table[hash] == null)
                    return false;

                foreach (int el in table[hash])
                    if (el == item)
                        return true;

                return false;
            }
            public bool Remove(int item)
            {
                int hash = GetHash(item);

                if (table[hash] == null)
                    return false;

                var curEl = table[hash].First;
                while(curEl != null)
                {
                    if (curEl.Value == item)
                    {
                        table[hash].Remove(curEl);
                        if (table[hash].Count == 0)
                            table[hash] = null;
                        return true;
                    }

                    curEl = curEl.Next;
                }

                return false;
            }
            private int GetHash(int item)
            {
                return item % Size;
            }
        }
        static int[] RadixSortLSD(int[] array)
        {
            List<Dictionary<int, LinkedList<int>>> levels = new List<Dictionary<int, LinkedList<int>>>();
            levels.Add(new Dictionary<int, LinkedList<int>>());

            int i = 0;

            for (int j = 0; j < 10; j++)
                levels[i].Add(j, new LinkedList<int>());
            foreach (int el in array)
                levels[i][GetDigitOfnumByPos(el, i).GetValueOrDefault()].AddLast(el);

            levels.Add(new Dictionary<int, LinkedList<int>>());
            i++;

            bool isEnd = false;
            while (!isEnd)
            {
                isEnd = true;
                for (int j = 0; j < 10; j++)
                    levels[i].Add(j, new LinkedList<int>());

                for (int k = 0; k < 10; k++)
                {
                    var list = levels[i - 1][k];
                    var el = list.First;
                    while(el != null)
                    {
                        int num = el.Value;
                        int? digit = GetDigitOfnumByPos(num, i);
                        var next = el.Next;
                        if (digit != null)
                        {
                            levels[i][digit.GetValueOrDefault()].AddLast(num);
                            list.Remove(el);
                            isEnd = false;
                        }
                        el = next;
                    }
                }

                levels.Add(new Dictionary<int, LinkedList<int>>());
                i++;
            }

            levels.RemoveAt(levels.Count - 1);

            List<int> result = new List<int>();
            foreach (var level in levels)
                foreach (var el in level)
                    foreach (int num in el.Value)
                        result.Add(num);

            return result.ToArray();

            int? GetDigitOfnumByPos(int num, int pos)
            {
                string str = num.ToString();
                pos = str.Length - 1 - pos;
                if (pos < 0)
                    return null;
                return str[pos] - 48;
            }
        }
        //Древовидная сортировка
        #region

        static int[] TreeSort(int[] array)
        {
            TSNode majorNode = new TSNode(null, array[0]);
            for (int i = 1; i < array.Length; i++)
                AddElInSubTree(majorNode, array[i]);

            List<int> result = new List<int>();
            GoDown(majorNode);

            return result.ToArray();

            void AddElInSubTree(TSNode node, int el)
            {
                if (el < node.Value)
                {
                    if (node.LB == null)
                        node.LB = new TSNode(node, el);
                    else
                        AddElInSubTree(node.LB, el);
                }
                else
                {
                    if (node.HB == null)
                        node.HB = new TSNode(node, el);
                    else
                        AddElInSubTree(node.HB, el);
                }
            }
            void GoDown(TSNode node)
            {
                if (node.LB != null)
                    GoDown(node.LB);
                result.Add(node.Value);
                if (node.HB != null)
                    GoDown(node.HB);
            }

        }
        class TSNode
        {
            public TSNode Father;
            public TSNode LB; //lowest branch
            public TSNode HB;
            public int Value;

            public TSNode(TSNode father, int value)
            {
                Father = father;
                Value = value;
            }
        }
        #endregion
        //Внешняя сортировка
        #region
        static void MakeFile(string path, int length)
        {
            Random rnd = new Random();

            StreamWriter file = new StreamWriter(path);

            for (int i = 0; i < length; i++)
                file.WriteLine(rnd.Next(100));

            file.Close();
        }
        static void SortFile(string file)
        {
            int i = 1;
            while(SplitFile(file, "A.txt", "B.txt", i))
            {
                MergeFiles("A.txt", "B.txt", file, i);
                i *= 2;
            }
            
        }
        static bool SplitFile(string originPath, string firstOutput, string secondOutput, int step)
        {
            if (File.Exists(firstOutput))
                File.Delete(firstOutput);
            if (File.Exists(secondOutput))
                File.Delete(secondOutput);

            StreamReader origin = new StreamReader(originPath);
            StreamWriter[] file = new StreamWriter[] { new StreamWriter(firstOutput), new StreamWriter(secondOutput) };

            string line;
            int i,j = 0;
            for (i = 0; true; i++)
            {
                line = origin.ReadLine();
                if (line == null)
                    break;

                file[(i / step) % 2].WriteLine(line);
                if ((i / step) % 2 == 0)
                    j++;
            }

            origin.Close();
            file[0].Close();
            file[1].Close();

            if (j >= i-1)
                return false;
            else
                return true;
        }
        static void MergeFiles(string firstFile, string secondFile, string resultFile, int step)
        {
            if (File.Exists(resultFile))
                File.Delete(resultFile);

            StreamWriter result = new StreamWriter(resultFile);
            StreamReader[] file = new StreamReader[] { new StreamReader(firstFile), new StreamReader(secondFile) };

            string[] line = new string[2] { file[0].ReadLine(), file[1].ReadLine() };
            int[] pos = new int[2];
            while(true)
            {
                if (pos[0] >= step || line[0] == null)
                {
                    while (pos[1] < step && line[1] != null)
                    {
                        result.WriteLine(line[1]);
                        line[1] = file[1].ReadLine();
                        pos[1]++;
                    }

                    pos = new int[2];
                    if (line[0] == null && line[1] == null)
                        break;
                }
                else if (pos[1] >= step || line[1] == null)
                {
                    while (pos[0] < step && line[0] != null)
                    {
                        result.WriteLine(line[0]);
                        line[0] = file[0].ReadLine();
                        pos[0]++;
                    }

                    pos = new int[2];
                }
                else
                {
                    if (Int32.Parse(line[0]) < Int32.Parse(line[1]))
                    {
                        result.WriteLine(line[0]);
                        line[0] = file[0].ReadLine();
                        pos[0]++;
                    }
                    else
                    {
                        result.WriteLine(line[1]);
                        line[1] = file[1].ReadLine();
                        pos[1]++;
                    }
                }
            }

            result.Close();
            file[0].Close();
            file[1].Close();
        }
        #endregion // Внешняя сортировка //внешняя сортировка
        //есть 2 каретки; двигать левую пока ни >=; двигать правую пока ни <=; менять местами если левая не правее правой, иначе рекурсивно отсортировать 
        static void DoQuickSort(ref int[] array, int start, int end)
        {
            if (end - start > 0)
            {
                int pivot = array[start + (end - start) / 2];
                int lo = start;
                int hi = end;

                while (true)
                {
                    while (array[lo] < pivot)
                        lo++;
                    while (array[hi] > pivot)
                        hi--;

                    if (lo <= hi)
                    {
                        int temp = array[lo];
                        array[lo] = array[hi];
                        array[hi] = temp;

                        lo++;
                        hi--;

                        if (lo > end || hi < start)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }

                DoQuickSort(ref array, start, hi);
                DoQuickSort(ref array, lo, end);
            }
        }
        static int DoBinarySearch(int[] array, int element)
        {
            int first = 0;
            int last = array.Length - 1;
            int i = first + (last - first) / 2;

            while(array[i] != element && last != first)
            {
                if (element < array[i])
                    last = i - 1;
                else
                    first = i + 1;

                i = first + (last - first) / 2;
            }

            if (array[i] == element)
                return i;
            else
                throw new Exception("Элемент не найден");
        }
        static void DoShallSort(ref int[] array)
        {
            int d = array.Length/2;
            while(d != 0)
            {
                for (int i = 0; i < d; i++)
                {
                    int j = i;
                    while(j < array.Length)
                    {
                        int k = j;
                        while(k-d >= 0 && array[k] < array[k-d])
                        {
                            int temp = array[k];
                            array[k] = array[k - d];
                            array[k - d] = temp;

                            k -= d;
                        }

                        j += d;
                    }
                }

                d /= 2;
            }
        }
        static void DoBiderectionalSort(ref int[] array)
        {
            for (int i = 0; i < array.Length/2; i++)
            {
                int sj = i;
                int ej = array.Length-i-1;
                while(sj < ej)
                {
                    if (array[sj] > array[sj+1])
                    {
                        int temp = array[sj];
                        array[sj] = array[sj + 1];
                        array[sj + 1] = temp;
                    }
                    sj++;
                }
                while (ej > i)
                {
                    if (array[ej] < array[ej - 1])
                    {
                        int temp = array[ej];
                        array[ej] = array[ej - 1];
                        array[ej - 1] = temp;
                    }
                    ej--;
                }

            }
        }
        static void DoChooseSort(ref int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int min = int.MaxValue;
                int minPos = -1;
                for (int j = i; j < array.Length; j++)
                {
                    if (array[j] < min)
                    {
                        min = array[j];
                        minPos = j;
                    }
                }

                int temp = array[i];
                array[i] = array[minPos];
                array[minPos] = temp;
            }
        }
        static void DoInsertSort(ref int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                int j = i;
                while (j > 0 && array[j] < array[j - 1])
                {
                    int temp = array[j];
                    array[j] = array[j - 1];
                    array[j - 1] = temp;
                    j--;
                }
            }
        }
        static void DoBubbleSort(ref int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 1; j < array.Length - i; j++)
                {
                    if (array[j] < array[j - 1])
                    {
                        int temp = array[j];
                        array[j] = array[j - 1];
                        array[j - 1] = temp;
                    }
                }
            }
        }
    }

    /*
     * Enumerable.Range(0, 100);
     * char - 48 = 0
     * char - 65 = A; букв 26; А + 26 - 1 = Z
     * StreamWriter|Reader
    */
}
