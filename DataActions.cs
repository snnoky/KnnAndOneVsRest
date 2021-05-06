using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zadanie2_1MIW
{
    public class DataActions
    {
        public static int decisionClassColumnIndex = 0;
        public static bool SwapValues = false;
        public static bool betweenMinMax = true;
        public static void Menu(UserConfig userData, DataTable data)
        {
            if (userData.DataName != "australian" && userData.DataName != "breast-cancer-wisconsin")
            {

                DataActions.SwapValues = true;

                data = ChangeSymbolToNumeric(data, userData);

                //for (int i = 0; i < data.Rows.Count; i++)
                //{
                //    Console.WriteLine(string.Join(" ", data.Rows[i].ItemArray));
                //}

                //for (int i = 0; i < data.Columns.Count; i++)
                //{
                //    Console.WriteLine(data.Columns[i].DataType);
                //}
                var normalizedData = DataActions.NormalizeData(data,
                    double.Parse(userData.DataNormalizationFrom.ToString()),
                    double.Parse(userData.DataNormalizationTo.ToString()));

                //Console.WriteLine("\nZnormalizowane dane:\n");
                //for (int i = 0; i < normalizedData.Rows.Count; i++)
                //{
                //    Console.WriteLine(string.Join(" ",
                //        normalizedData.Rows[i].ItemArray.Select(x => string.Format("{0:0.##}", x))));
                //}
                Console.WriteLine("Knn czy jedenVsReszta? 1-knn 2-jedenVsReszta");
                var knnOrJvr = Console.ReadLine();

                if (knnOrJvr == "1")
                {
                    Console.WriteLine("Podaj próbkę testową");
                    var testSampleCounter = 1;
                    var testSample = new List<dynamic>();
                    while (testSampleCounter < normalizedData.Columns.Count)
                    {
                        Console.WriteLine($"a{testSampleCounter}: ");
                        var testSampleItem = Console.ReadLine();

                        testSample.Add(testSampleItem);
                        testSampleCounter++;
                    }

                    //for (int i = 0; i < testSample.Count; i++)
                    //{
                    //    Console.WriteLine(testSample[i]);
                    //}


                    for (int i = 0; i < testSample.Count; i++)
                    {
                        try
                        {
                            testSample[i] = Convert.ToDouble(testSample[i]);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    var testSampleTable = new DataTable();
                    for (int i = 0; i < testSample.Count; i++)
                    {
                        if (testSample[i] is double)
                        {
                            testSampleTable.Columns.Add().DataType = typeof(double);
                        }
                        else
                        {
                            testSampleTable.Columns.Add().DataType = typeof(string);
                        }

                    }


                    testSampleTable.Rows.Add();
                    //Console.WriteLine(testSample);
                    for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    {
                        testSampleTable.Rows[0][i] = testSample[i];
                    }
                    testSampleTable = ChangeTestSampleSymbolToNumeric(testSampleTable, userData);
                    //for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    //{
                    //    Console.WriteLine("---");
                    //    Console.WriteLine(testSampleTable.Rows[0][i]);
                    //    Console.WriteLine("-----");
                    //    Console.WriteLine(testSampleTable.Columns[i].DataType);
                    //}

                    //double oldMin = sizeof(double);
                    //double oldMax = 0;
                    //Console.WriteLine("----------------------");
                    for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    {
                        var items = testSampleTable.Select().Select(x => Convert.ToDouble(x[i]));
                        var itemsCast = items.Cast<double>().ToList();
                        //for (int j = 0; j < itemsCast.Count; j++)
                        //{
                        //    if (oldMin > itemsCast[j])
                        //    {
                        //        oldMin = itemsCast[j];
                        //    }

                        //    if (oldMax < itemsCast[j])
                        //    {
                        //        oldMax = itemsCast[j];
                        //    }
                        //}

                        //Console.WriteLine(testSampleTable.Rows[0][i]);
                    }

                    for (int i = 0; i < testSample.Count; i++)
                    {
                        testSample[i] = testSampleTable.Rows[0][i];
                        //Convert.ToDouble(testSample[i]);
                    }

                    //Console.WriteLine("----");
                    for (int i = 0; i < testSample.Count; i++)
                    {
                        if (testSample[i] > userData.Maxs[i].Value)
                        {
                            Console.WriteLine($"Liczba {testSample[i]} większa od max {userData.Maxs[i].Value}!");
                            betweenMinMax = false;
                            break;
                        }

                        if (testSample[i] < userData.Mins[i].Value)
                        {
                            Console.WriteLine($"Liczba {testSample[i]} mniejsza od min {userData.Mins[i].Value}!");
                            betweenMinMax = false;
                            break;
                        }
                    }

                    if (betweenMinMax == false)
                    {
                        return;
                    }

                    testSampleTable = NormalizeTestData(testSampleTable, userData.DataNormalizationFrom, userData.DataNormalizationTo, userData);

                    Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                    var chosenMetrics = Console.ReadLine();
                    //Dictionary<int, double> distance;
                    //if (chosenMetrics == "1")
                    //{
                    //    distance = Euklides(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "2")
                    //{
                    //    distance = Manhattan(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "3")
                    //{
                    //    distance = Czebyszew(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "4")
                    //{
                    //    distance = Minkowski(normalizedData, testSampleTable, 1);
                    //}
                    //else
                    //{
                    //    return;
                    //}

                    //for (int i = 0; i < distance.Count; i++)
                    //{
                    //    Console.WriteLine(distance[i]);
                    //}

                    //foreach (var item in distance)
                    //{
                    //    Console.WriteLine(item.Key);
                    //}
                    Console.WriteLine("Podaj k:");
                    var k = Convert.ToInt32(Console.ReadLine());
                    if (k < 0)
                    {
                        Console.WriteLine("Nie dozwolone k!");
                        return;
                    }
                    Console.WriteLine("Wybierz wersję Knn 1 lub 2.");
                    var knnVersion = Console.ReadLine();
                    var finalDecision = "";
                    if (knnVersion == "1")
                    {
                        finalDecision = KnnVersionOne(normalizedData, k, testSampleTable, chosenMetrics);
                        Console.WriteLine($"Decyzja: {finalDecision}");
                    }
                    else if (knnVersion == "2")
                    {
                        KnnVersionTwo(normalizedData, k, testSampleTable, chosenMetrics);
                    }
                    else
                    {
                        Console.WriteLine("koniec");
                    }
                }
                else if (knnOrJvr == "2")
                {
                    Console.WriteLine("Wybierz wersję knn: 1-K najbliższych 2-suma");
                    var chosenKnn = Console.ReadLine();

                    var k = 0;
                    var chosenMetrics = "";
                    if (chosenKnn == "1")
                    {
                        Console.WriteLine($"Podaj k max k to: {normalizedData.Rows.Count}");
                        k = Convert.ToInt32(Console.ReadLine());
                        if (k > normalizedData.Rows.Count || k <= 0)
                        {
                            Console.WriteLine("Niedozwolone k");
                            return;
                        }
                        Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                        chosenMetrics = Console.ReadLine();

                        //OneVersusRest(normalizedData, normalizedData.Rows.Count, k, chosenMetrics, chosenKnn);
                    }
                    else if (chosenKnn == "2")
                    {
                        var maxK = 999999999999999;
                        var classesGroup = new List<string>();
                        var classOne = new List<string>();
                        var classTwo = new List<string>();
                        for (int i = 0; i < normalizedData.Rows.Count; i++)
                        {
                            classesGroup.Add((string)normalizedData.Rows[i][decisionClassColumnIndex]);
                        }

                        var result = classesGroup.GroupBy(x => x)
                            .ToDictionary(y => y.Key, y => y.Count())
                            .OrderByDescending(z => z.Value);

                        foreach (var item in result)
                        {
                            if (item.Value < maxK)
                            {
                                maxK = item.Value;
                            }
                        }
                        //classesGroup.Sort();
                        //var count = 0;
                        //for (int i = 0; i < classesGroup.Count; i++)
                        //{
                        //    if (i > 0 && classesGroup[i] != classesGroup[i - 1])
                        //    {
                        //        break;
                        //    }

                        //    classOne.Add(classesGroup[i]);
                        //    count++;
                        //}

                        //for (int i = count; i < classesGroup.Count; i++)
                        //{
                        //    classTwo.Add(classesGroup[i]);
                        //}

                        //maxK = Math.Min(classOne.Count, classTwo.Count);
                        //if (maxK > 1)
                        //{
                        //    Console.WriteLine($"Podaj k max to: {maxK-1}");
                        //}
                        //else
                        //{
                        //    Console.WriteLine($"Podaj k max to: {maxK}");
                        //}
                        //k = Convert.ToInt32(Console.ReadLine());
                        if (maxK > 1)
                        {
                            Console.WriteLine($"Podaj k max to: {maxK - 1}");
                            k = Convert.ToInt32(Console.ReadLine());

                            if (k <= 0 || k >= maxK)
                            {
                                Console.WriteLine("Nie dozwolone k!");
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Podaj k max to: {maxK}");
                            k = Convert.ToInt32(Console.ReadLine());

                            if (k <= 0 || k > maxK)
                            {
                                Console.WriteLine("Nie dozwolone k!");
                                return;
                            }
                        }
                        if (k <= 0 || k > maxK)
                        {
                            Console.WriteLine("Nie dozwolone k!");
                            return;
                        }

                        Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                        chosenMetrics = Console.ReadLine();

                       // OneVersusRest(normalizedData, normalizedData.Rows.Count, k, chosenMetrics, chosenKnn);
                    }
                    else
                    {
                        Console.WriteLine("koniec");
                        return;
                    }
                    OneVersusRest(normalizedData, normalizedData.Rows.Count, k, chosenMetrics, chosenKnn);
                }
                else
                {
                    Console.WriteLine("Koniec.");
                    return;
                }
            }
            else
            {
                var normalizedData = DataActions.NormalizeData(data,
                    double.Parse(userData.DataNormalizationFrom.ToString()),
                    double.Parse(userData.DataNormalizationTo.ToString()));
                Console.WriteLine("Knn czy JedenVsReszta? 1-knn 2-jedenVsReszta");
                var knnOrJvr = Console.ReadLine();
                if (knnOrJvr == "1")
                {
                    Console.WriteLine("Podaj próbkę testową");
                    var testSampleCounter = 1;
                    var testSample = new List<dynamic>();
                    while (testSampleCounter < normalizedData.Columns.Count)
                    {
                        Console.WriteLine($"a{testSampleCounter}: ");
                        var testSampleItem = Console.ReadLine();

                        testSample.Add(testSampleItem);
                        testSampleCounter++;
                    }

                    //for (int i = 0; i < testSample.Count; i++)
                    //{
                    //    Console.WriteLine(testSample[i]);
                    //}


                    for (int i = 0; i < testSample.Count; i++)
                    {
                        try
                        {
                            testSample[i] = Convert.ToDouble(testSample[i]);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    var testSampleTable = new DataTable();
                    for (int i = 0; i < testSample.Count; i++)
                    {
                        if (testSample[i] is double)
                        {
                            testSampleTable.Columns.Add().DataType = typeof(double);
                        }
                        else
                        {
                            testSampleTable.Columns.Add().DataType = typeof(string);
                        }

                    }


                    testSampleTable.Rows.Add();
                    //Console.WriteLine(testSample);
                    for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    {
                        testSampleTable.Rows[0][i] = testSample[i];
                    }

                    //for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    //{
                    //    Console.WriteLine("---");
                    //    Console.WriteLine(testSampleTable.Rows[0][i]);
                    //    Console.WriteLine("-----");
                    //    Console.WriteLine(testSampleTable.Columns[i].DataType);
                    //}
                    testSampleTable = ChangeTestSampleSymbolToNumeric(testSampleTable, userData);
                    //double oldMin = sizeof(double);
                    //double oldMax = 0;
                    //Console.WriteLine("----------------------");
                    for (int i = 0; i < testSampleTable.Columns.Count; i++)
                    {
                        var items = testSampleTable.Select().Select(x => Convert.ToDouble(x[i]));
                        var itemsCast = items.Cast<double>().ToList();
                        //for (int j = 0; j < itemsCast.Count; j++)
                        //{
                        //    if (oldMin > itemsCast[j])
                        //    {
                        //        oldMin = itemsCast[j];
                        //    }

                        //    if (oldMax < itemsCast[j])
                        //    {
                        //        oldMax = itemsCast[j];
                        //    }
                        //}

                        // Console.WriteLine(testSampleTable.Rows[0][i]);
                    }

                    for (int i = 0; i < testSample.Count; i++)
                    {
                        testSample[i] = testSampleTable.Rows[0][i];
                    }

                    //Console.WriteLine("----");
                    for (int i = 0; i < testSample.Count; i++)
                    {
                        if (testSample[i] > userData.Maxs[i].Value)
                        {
                            Console.WriteLine($"Liczba {testSample[i]} większa od max {userData.Maxs[i].Value}!");
                            betweenMinMax = false;
                            break;
                        }

                        if (testSample[i] < userData.Mins[i].Value)
                        {
                            Console.WriteLine($"Liczba {testSample[i]} mniejsza od min {userData.Mins[i].Value}!");
                            betweenMinMax = false;
                            break;
                        }
                    }

                    if (betweenMinMax == false)
                    {
                        return;
                    }

                    testSampleTable = NormalizeTestData(testSampleTable, userData.DataNormalizationFrom, userData.DataNormalizationTo, userData);

                    Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                    var chosenMetrics = Console.ReadLine();
                    //Dictionary<int, double> distance;
                    //if (chosenMetrics == "1")
                    //{
                    //    distance = Euklides(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "2")
                    //{
                    //    distance = Manhattan(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "3")
                    //{
                    //    distance = Czebyszew(normalizedData, testSampleTable);
                    //}
                    //else if (chosenMetrics == "4")
                    //{
                    //    distance = Minkowski(normalizedData, testSampleTable, 1);
                    //}
                    //else
                    //{
                    //    return;
                    //}

                    //for (int i = 0; i < distance.Count; i++)
                    //{
                    //    Console.WriteLine(distance[i]);
                    //}

                    //foreach (var item in distance)
                    //{
                    //    Console.WriteLine(item.Key);
                    //}
                    Console.WriteLine("Podaj k:");
                    var k = Convert.ToInt32(Console.ReadLine());
                    if (k < 0)
                    {
                        Console.WriteLine("Nie dozwolone k!");
                        return;
                    }
                    Console.WriteLine("Wybierz wersję knn: 1-knn 2-knnV2");
                    var knnVersion = Console.ReadLine();
                    var finalDecision = "";
                    if (knnVersion == "1")
                    {
                        finalDecision = KnnVersionOne(normalizedData, k, testSampleTable, chosenMetrics);
                        Console.WriteLine($"Decyzja: {finalDecision}");
                    }
                    else if (knnVersion == "2")
                    {
                        finalDecision = KnnVersionTwo(normalizedData, k, testSampleTable, chosenMetrics);
                        Console.WriteLine($"Decyzja: {finalDecision}");
                    }
                    else
                    {
                        Console.WriteLine("koniec");
                        return;
                    }
                }
                else if (knnOrJvr == "2")
                {
                    Console.WriteLine("Wybierz wersję knn: 1-K najbliższych 2-suma");
                    var chosenKnn = Console.ReadLine();

                    var k = 0;
                    var chosenMetrics = "";
                    if (chosenKnn == "1")
                    {
                        Console.WriteLine($"Podaj k max k to: {normalizedData.Rows.Count}");
                        k = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                        chosenMetrics = Console.ReadLine();

                        OneVersusRest(normalizedData, normalizedData.Rows.Count, k, chosenMetrics, chosenKnn);
                    }
                    else if (chosenKnn == "2")
                    {
                        var maxK = 999999999999999;
                        var classesGroup = new List<string>();
                        var classOne = new List<string>();
                        var classTwo = new List<string>();
                        for (int i = 0; i < normalizedData.Rows.Count; i++)
                        {
                            classesGroup.Add((string) normalizedData.Rows[i][decisionClassColumnIndex]);
                        }

                        var result = classesGroup.GroupBy(x => x)
                            .ToDictionary(y => y.Key, y => y.Count())
                            .OrderByDescending(z => z.Value);

                        foreach (var item in result)
                        {
                            if (item.Value < maxK)
                            {
                                maxK = item.Value;
                            }
                        }

                        //classesGroup.Sort();
                        //var count = 0;
                        //for (int i = 0; i < classesGroup.Count; i++)
                        //{
                        //    if (i > 0 && classesGroup[i] != classesGroup[i - 1])
                        //    {
                        //        break;
                        //    }

                        //    classOne.Add(classesGroup[i]);
                        //    count++;
                        //}

                        //for (int i = count; i < classesGroup.Count; i++)
                        //{
                        //    classTwo.Add(classesGroup[i]);
                        //}

                        //maxK = Math.Min(classOne.Count, classTwo.Count);
                        if (maxK > 1)
                        {
                            Console.WriteLine($"Podaj k max to: {maxK-1}");
                            k = Convert.ToInt32(Console.ReadLine());

                            if (k <= 0 || k >= maxK)
                            {
                                Console.WriteLine("Nie dozwolone k!");
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Podaj k max to: {maxK}");
                            k = Convert.ToInt32(Console.ReadLine());

                            if (k <= 0 || k > maxK)
                            {
                                Console.WriteLine("Nie dozwolone k!");
                                return;
                            }
                        }
                        

                        Console.WriteLine("Wybierz metrykę: 1-Euklides, 2-Czebyszew, 3-Manhattan, 4-Minkowski");
                        chosenMetrics = Console.ReadLine();

                        OneVersusRest(normalizedData, normalizedData.Rows.Count, k, chosenMetrics, chosenKnn);
                    }
                    else
                    {
                        Console.WriteLine("koniec");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Koniec.");
                    return;
                }
                //Console.WriteLine("\nZnormalizowane dane:\n");
                //var trainSamples = new double[normalizedData.Columns.Count,normalizedData.Rows.Count];
                //var unknownSample = new double[normalizedData.Rows[0].ItemArray.Length];
                //var unknownSample = new Dictionary<int, DataRow>();
                //for (int i = 0; i < normalizedData.Rows.Count; i++)
                //{
                //    Console.WriteLine(string.Join(" ",
                //        normalizedData.Rows[i].ItemArray.Select(x => string.Format("{0:0.##}", x))));
                //    //for (int j = 0; j < normalizedData.Columns.Count - 1; j++)
                //    //{
                //    //    trainSamples[j,i] = Convert.ToDouble(normalizedData.Rows[i].ItemArray[j]);
                //    //}

                //}
                // Console.WriteLine(trainSamples[13,0]);
                //Console.WriteLine(normalizedData.Rows[0].ItemArray);

                //for (int i = 0; i < normalizedData.Rows[0].ItemArray.Length; i++)
                //{
                //    unknownSample[i] = Convert.ToDouble(normalizedData.Rows[i].ItemArray[i]);
                //}
                //for (int i = 0; i < normalizedData.Rows.Count; i++)
                //{
                //    unknownSample[i] = normalizedData.Rows[i];
                //}
                //unknownSample[0] = normalizedData.Rows[0];
                //Console.WriteLine(unknownSample[2].ItemArray[2]);
            }
        }

        public static DataTable GetValuesFromFile(string filePath, string fileTypePath, string separator)
        {

            var table = new DataTable();
            if (!File.Exists(filePath) || !File.Exists(fileTypePath))
            {
                Console.WriteLine("Podano złą ścieżkę!");
            }
            else
            {
                var lines = File.ReadAllLines(filePath);
                var linesOfTypes = File.ReadAllLines(fileTypePath);

                var counter = 0; // liczenie kolumn
                var lineCounter = 0; // liczenie linii do bloku try/catch

                foreach (var line in linesOfTypes)
                {

                    var type = line.Split(" ")[1];
                    if (type == "d")
                    {
                        decisionClassColumnIndex = counter;
                        table.Columns.Add(($"kolumna{++counter}(Class attr)").ToString(), typeof(string));
                    }
                    else
                    {
                        table.Columns.Add(($"kolumna{++counter}").ToString(), type == "s" ? typeof(string) : typeof(double));
                    }

                }

                foreach (var line in lines)
                {
                    lineCounter++;
                    var values = line.Split(separator);
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Replace(".", ",");
                    }

                    var row = table.NewRow();

                    if (values.Contains("?"))
                    {
                        Console.WriteLine("Pominięto wiersz " + lineCounter + " natrafiono na \"?\"");
                        continue;
                    }

                    row.ItemArray = values;

                    table.Rows.Add(row);
                }
            }
            return table;
        }
        public static DataTable ChangeSymbolToNumeric(DataTable table, UserConfig config)
        {
            var newTable = table;
            var result = new DataTable();

            for (int i = 0; i < table.Columns.Count - 1; i++)
            {
                result.Columns.Add().DataType = typeof(double);
            }

            result.Columns.Add();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                result.Rows.Add();
            }

            for (int i = 0; i < newTable.Columns.Count - 1; i++)
            {
                if (newTable.Columns[i].DataType != typeof(string)) continue;
                if (i == decisionClassColumnIndex) continue;

                var items = newTable.Select().Select(x => x[i]);
                var itemsCast = items.Cast<string>().ToList();

                for (int j = 0; j < itemsCast.Count; j++)
                {
                    var item = (string)newTable.Rows[j][i];

                    if (item == "?")
                    {
                        continue;
                    }

                    try
                    {
                        newTable.Rows[j][i] = config.DataSybmolicsToNumerics.First(x => x.From == item).To;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Brak informacji w UserConfig na temat zamiany elementu: {newTable.Rows[j][i]}");
                        throw;
                    }

                }
            }
            for (int i = 0; i < newTable.Columns.Count - 1; i++)
            {
                var items = newTable.Select().Select(x => Convert.ToDouble(x[i]));
                var itemsCast = items.Cast<double>().ToList();
                for (int j = 0; j < itemsCast.Count; j++)
                {
                    var item = newTable.Rows[j][i];

                    result.Rows[j][i] = Convert.ToDouble(item);
                }
            }

            for (int i = 0; i < result.Rows.Count; i++)
            {
                result.Rows[i][decisionClassColumnIndex] = table.Rows[i][decisionClassColumnIndex];
            }

            return result;
        }
        public static DataTable ChangeTestSampleSymbolToNumeric(DataTable table, UserConfig config)
        {
            var newTable = table;
            var result = new DataTable();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Columns.Add().DataType = typeof(double);
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                result.Rows.Add();
            }

            for (int i = 0; i < newTable.Columns.Count; i++)
            {
                if (newTable.Columns[i].DataType != typeof(string)) continue;
                if (i == decisionClassColumnIndex) continue;

                var items = newTable.Select().Select(x => x[i]);
                var itemsCast = items.Cast<string>().ToList();

                for (int j = 0; j < itemsCast.Count; j++)
                {
                    var item = (string)newTable.Rows[j][i];

                    if (item == "?")
                    {
                        continue;
                    }

                    try
                    {
                        newTable.Rows[j][i] = config.DataSybmolicsToNumerics.First(x => x.From == item).To;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Brak informacji w UserConfig na temat zamiany elementu: {newTable.Rows[j][i]}");
                        throw;
                    }

                }
            }

            for (int i = 0; i < newTable.Columns.Count; i++)
            {
                var items = newTable.Select().Select(x => Convert.ToDouble(x[i]));
                var itemsCast = items.Cast<double>().ToList();
                for (int j = 0; j < itemsCast.Count; j++)
                {
                    var item = newTable.Rows[j][i];

                    result.Rows[j][i] = item;
                }
            }

            return result;
        }
        public static DataTable NormalizeData(DataTable table, double fromNumber, double toNumber)
        {
            var normalizedTable = table;

            for (int j = 0; j < normalizedTable.Columns.Count; j++)
            {
                if (j == decisionClassColumnIndex) continue;
                if (!SwapValues && normalizedTable.Columns[j].DataType != typeof(double)) continue;

                var items = normalizedTable.Select().Select(x => Convert.ToDouble(x[j]));
                var itemsCast = items.Cast<double>().ToList();
                var min = itemsCast.Min();
                var max = itemsCast.Max();

                for (int i = 0; i < itemsCast.Count; i++)
                {
                    normalizedTable.Rows[i][j] = ((itemsCast[i] - min) / (max - min)) * ((toNumber - fromNumber)) + fromNumber;
                }
            }

            return normalizedTable;
        }

        public static DataTable NormalizeTestData(DataTable table, double fromNumber, double toNumber, UserConfig config)
        {
            var normalizedTable = table;

            for (int j = 0; j < normalizedTable.Columns.Count; j++)
            {
                if (j == decisionClassColumnIndex) continue;

                var items = normalizedTable.Select().Select(x => Convert.ToDouble(x[j]));
                var itemsCast = items.Cast<double>().ToList();


                for (int i = 0; i < itemsCast.Count; i++)
                {
                    normalizedTable.Rows[i][j] = (itemsCast[i] - config.Mins[j].Value) / (config.Maxs[j].Value - config.Mins[j].Value) * ((toNumber - fromNumber)) + fromNumber;
                }
            }

            return normalizedTable;
        }

        public static string KnnVersionOne(DataTable trainSample, int k, DataTable testSample, string metrics)
        {

            var distances = new Dictionary<int, double>();
            if (metrics == "1")
            {
                distances = Euklides(trainSample, testSample);
            }
            else if (metrics == "2")
            {
                distances = Czebyszew(trainSample, testSample);
            }
            else if (metrics == "3")
            {
                distances = Manhattan(trainSample, testSample);
            }
            else if (metrics == "4")
            {
                distances = Minkowski(trainSample, testSample, 1);
            }
            var finalDecision = "";
            if (k > trainSample.Rows.Count)
            {
                //Console.WriteLine("Nie dozwolone k!"); tta funkcja nie powinna nic wypisywać wtedy zwrócić null
                finalDecision = null;
                return finalDecision;
            }

            var decisionsRow = new List<int>();
            var decisions = new List<string>();
            var counter = 0;
            foreach (var item in distances)
            {
                if (counter == k)
                {
                    break;
                }

                decisionsRow.Add(item.Key);

                counter++;
            }

            for (int i = 0; i < decisionsRow.Count; i++)
            {
                var value = trainSample.Rows[decisionsRow[i]][decisionClassColumnIndex];
                decisions.Add(value.ToString());
            }

            //for (int i = 0; i < decisions.Count; i++)
            //{
            //    Console.WriteLine(decisions[i]);
            //}

            var groupedDecisions = decisions.GroupBy(i => i);
            var chosenDecision = new Dictionary<string, int>();
            foreach (var grp in groupedDecisions)
            {
                //Console.WriteLine("{0} {1}", grp.Key, grp.Count());
                chosenDecision.Add(grp.Key, grp.Count());
            }

            var countDecision = 0;

            foreach (var item in chosenDecision)
            {
                if (item.Value == countDecision)
                {
                    //Console.WriteLine("Brak odpowiedzi Remis!");
                    finalDecision = null;
                    return finalDecision;
                }
                if (item.Value > countDecision)
                {
                    countDecision = item.Value;
                    finalDecision = item.Key;
                }
            }

            //Console.WriteLine($"Decyzja: {finalDecision}");
            return finalDecision;
        }

        public static string KnnVersionTwo(DataTable trainSample, int k, DataTable testSample, string metrics)
        {
            var distances = new Dictionary<int, double>();
            if (metrics == "1")
            {
                distances = Euklides(trainSample, testSample);
            }
            else if (metrics == "2")
            {
                distances = Czebyszew(trainSample, testSample);
            }
            else if (metrics == "3")
            {
                distances = Manhattan(trainSample, testSample);
            }
            else if (metrics == "4")
            {
                distances = Minkowski(trainSample, testSample, 1);
            }

            var finalDecision = "";
            var decisionsRow = new List<int>();
            var decisions = new List<string>();
            //List<KeyValuePair<string, double>> sortedClasses = new List<KeyValuePair<string, double>>();
            List<KeyValuePair<string, double>> sortedClasses = new List<KeyValuePair<string, double>>();

            foreach (var item in distances)
            {
                decisionsRow.Add(item.Key);
            }

            for (int i = 0; i < decisionsRow.Count; i++)
            {
                var value = trainSample.Rows[decisionsRow[i]][decisionClassColumnIndex];
                decisions.Add(value.ToString());
                //sortedClasses.Add(decisions[i],distances[i]);
                sortedClasses.Add(new KeyValuePair<string, double>(decisions[i], distances[i]));
            }

            sortedClasses = sortedClasses.OrderBy(item => item.Key).ToList();
            //Console.WriteLine();

            //Console.WriteLine(sortedClasses[0].Value);

            var groupedDecisions = decisions.GroupBy(i => i);
            var countGroup = new List<int>();
            foreach (var grp in groupedDecisions)
            {
                //Console.WriteLine("{0} {1}", grp.Key, grp.Count());
                countGroup.Add(grp.Count());
            }
            var classes = new List<string>();
            foreach (var item in groupedDecisions)
            {
                classes.Add(item.Key);
            }
            var distancesValues = new List<double>();
            var sumOfClasses = new double[classes.Count];
            foreach (var item in distances)
            {
                distancesValues.Add(item.Value);
            }
            
            for (int i = 0; i < sumOfClasses.Length; i++)
            {
                var licznik = 0;
                var index = 0;
                while (licznik < k)
                {
                    if ((string)trainSample.Rows[decisionsRow[index]][decisionClassColumnIndex] == classes[i])
                    {
                        sumOfClasses[i] += distancesValues[index];
                        licznik++;
                    }

                    index++;
                }
            }

            var indexOfDecision = 0;
            var ties =  new List<string>();
            for (int i = 0; i < sumOfClasses.Length; i++)
            {
                if (sumOfClasses[i] < sumOfClasses[indexOfDecision])
                {
                    indexOfDecision = i;
                }
            }

            for (int i = 0; i < sumOfClasses.Length-1; i++)
            {
                if (sumOfClasses[i] == sumOfClasses[i+1])
                {
                    ties.Add($"tie{i}");
                }
            }

            if (ties.Count == sumOfClasses.Length-1)
            {
                finalDecision = null;
                return finalDecision;
            }
            finalDecision = classes[indexOfDecision];
            
            //var help = new List<KeyValuePair<string, double>>(); //firstclass
            //var help2 = new List<KeyValuePair<string, double>>(); //secondclass
            //int counter = 0; //count rows
            //foreach (var item in sortedClasses)
            //{
            //    if (counter == countGroup[0])
            //    {
            //        break;
            //    }
            //    help.Add(new KeyValuePair<string, double>(item.Key, item.Value));
            //    counter++;
            //}

            //counter = 0;
            //foreach (var item in sortedClasses)
            //{
            //    if (counter < countGroup[0])
            //    {
            //        counter++;
            //        continue;
            //    }
            //    help2.Add(new KeyValuePair<string, double>(item.Key, item.Value));
            //    counter++;
            //}

            //foreach (var item in help)
            //{
            //    help = help.OrderBy(item => item.Value).ToList();
            //}
            //foreach (var item in help)
            //{
            //    help2 = help2.OrderBy(item => item.Value).ToList();
            //}
            //help = help.OrderBy(item => item.Value).ToList();
            //help2 = help2.OrderBy(item => item.Value).ToList();

            //double classOneSum = 0;
            //double classTwoSum = 0;
            //counter = 0;
            //foreach (var item in help)
            //{
            //    if (counter == k)
            //    {
            //        break;
            //    }

            //    classOneSum += item.Value;
            //    counter++;
            //}

            //counter = 0;
            //foreach (var item in help2)
            //{
            //    if (counter == k)
            //    {
            //        break;
            //    }

            //    classTwoSum += item.Value;
            //    counter++;
            //}

            //if (classOneSum < classTwoSum)
            //{
            //    //Console.WriteLine($"Decyzja {sortedClasses[0].Key}");
            //    finalDecision = help[0].Key;
            //    return finalDecision;
            //}
            //else if (classOneSum > classTwoSum)
            //{
            //    //Console.WriteLine($"Decyzja {sortedClasses[sortedClasses.Count - 1].Key}");
            //    finalDecision = help2[0].Key;
            //    return finalDecision;
            //}
            //else
            //{
            //    finalDecision = null;
            //}
            //Console.WriteLine("Remis");
            
            return finalDecision;

            //Console.WriteLine("");

        }

        public static void OneVersusRest(DataTable trainData, int SamplesCount, int k, string metrics, string knnVersion)
        {
            var testSample = new DataTable();

            var decisions = new List<string>();
            var decision = "";
            double successful = 0;
            double failed = 0;
            double correct = 0;
            double covering = 0;
            double efectiveness = 0;
            for (int i = 0; i < trainData.Columns.Count - 1; i++)
            {
                testSample.Columns.Add().DataType = typeof(double);
            }

            testSample.Rows.Add();


            for (int i = 0; i < SamplesCount; i++)
            {
                var trainSample = new DataTable();
                for (int j = 0; j < trainData.Columns.Count - 1; j++)
                {
                    trainSample.Columns.Add().DataType = typeof(double);
                }

                trainSample.Columns.Add();
                for (int j = 0; j < trainData.Columns.Count - 1; j++)
                {
                    testSample.Rows[0][j] = trainData.Rows[i][j];
                }

                for (int j = 0; j < trainData.Rows.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    trainSample.Rows.Add(trainData.Rows[j].ItemArray);
                }
                //var distances = new Dictionary<int, double>();
                //distances = Euklides(trainSample, testSample);
                // decisions.Add(KnnVersionOne(trainSample, k, testSample, distances));
                //Console.WriteLine(i);
                if (knnVersion == "1")
                {
                    decision = KnnVersionOne(trainSample, k, testSample, metrics);
                    Console.WriteLine($"Wiersz:{i+1} Decyzja:{decision}");
                }
                else
                {
                    decision = KnnVersionTwo(trainSample, k, testSample, metrics);
                    Console.WriteLine($"Wiersz:{i+1} Decyzja:{decision}");

                }

                //Console.WriteLine();
                if (decision != null)
                {
                    successful++;
                    if (decision == (string)trainData.Rows[i][decisionClassColumnIndex])
                    {
                        correct++;
                    }
                }
                else
                {
                    failed++;
                }
                //distances.Clear();
                //Console.WriteLine();
            }

            covering = (successful / SamplesCount) * 100;
            efectiveness = (correct / successful) * 100;
            Console.WriteLine($"Ilość próbek: {SamplesCount}");
            Console.WriteLine($"Udane: {successful}");
            Console.WriteLine($"Poprawne: {correct}");
            Console.WriteLine($"Nie udane: {failed}");
            Console.WriteLine($"Pokrycie: {Math.Round(covering, 2)}%");
            Console.WriteLine($"Efektywnośc: {Math.Round(efectiveness, 2)}%");
        }

        public static Dictionary<int, double> Manhattan(DataTable trainSample, DataTable testSample)
        {
            var distances = new Dictionary<int, double>();
            double sum = 0;
            double value = 0;
            double value2 = 0;
            for (int i = 0; i < trainSample.Rows.Count; i++)
            {
                for (int j = 0; j < trainSample.Columns.Count - 1; j++)
                {
                    value = (double)trainSample.Rows[i][j];
                    value2 = (double)testSample.Rows[0][j];
                    sum += Math.Abs(value - value2);
                }
                distances.Add(i, sum);
                sum = 0;
            }
            //return posortowana od najmniejszej do najwiekszej lista odleglosci
            distances = new Dictionary<int, double>(distances.OrderBy(x => x.Value));
            return distances;
        }

        public static Dictionary<int, double> Euklides(DataTable trainSample, DataTable testSample)
        {
            var distances = new Dictionary<int, double>();
            double sum = 0;
            double value = 0;
            double value2 = 0;
            for (int i = 0; i < trainSample.Rows.Count; i++)
            {
                for (int j = 0; j < trainSample.Columns.Count - 1; j++)
                {
                    value = (double)trainSample.Rows[i][j];
                    value2 = (double)testSample.Rows[0][j];
                    sum += Math.Pow(value - value2, 2);
                }

                sum = Math.Sqrt(sum);
                distances.Add(i, sum);
                sum = 0;
            }
            //return posortowana od najmniejszej do najwiekszej lista odleglosci
            distances = new Dictionary<int, double>(distances.OrderBy(x => x.Value));
            return distances;
        }

        public static Dictionary<int, double> Czebyszew(DataTable trainSample, DataTable testSample)
        {
            var distances = new Dictionary<int, double>();

            double value = 0;
            double value2 = 0;
            for (int i = 0; i < trainSample.Rows.Count; i++)
            {
                var maxDistance = new List<double>();
                for (int j = 0; j < trainSample.Columns.Count - 1; j++)
                {
                    value = (double)trainSample.Rows[i][j];
                    value2 = (double)testSample.Rows[0][j];
                    maxDistance.Add(Math.Abs(value - value2));
                }
                distances.Add(i, maxDistance.Max());
            }
            //return posortowana od najmniejszej do najwiekszej lista odleglosci
            distances = new Dictionary<int, double>(distances.OrderBy(x => x.Value));
            return distances;
        }

        public static Dictionary<int, double> Minkowski(DataTable trainSample, DataTable testSample, int p)
        {
            var distances = new Dictionary<int, double>();
            double sum = 0;
            double value = 0;
            double value2 = 0;
            for (int i = 0; i < trainSample.Rows.Count; i++)
            {
                for (int j = 0; j < trainSample.Columns.Count - 1; j++)
                {
                    value = (double)trainSample.Rows[i][j];
                    value2 = (double)testSample.Rows[0][j];
                    sum += Math.Pow(Math.Abs(value - value2), p);
                }

                sum = Math.Pow(sum, 1 / p);
                distances.Add(i, sum);
                sum = 0;
            }
            //return posortowana od najmniejszej do najwiekszej lista odleglosci
            distances = new Dictionary<int, double>(distances.OrderBy(x => x.Value));
            return distances;
        }
    }
}
// Dopisać możliwość zmiany metryki i wersji knn x

//poprawic 4 parametry set/próbka(set)/jedna próbka/metryka(wybór jaka i niech dzieje się w knn) x
//w interfejsie mozliwosc 1vsReszta wybor parametrów metryka typ który wariant knn i k x
//narzędziowe funkcje milczące x