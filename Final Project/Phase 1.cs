using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TLA_Project_FirstQuestion
{
    class Program
    {
        static void Main()
        {
            ImportData();
            removeNull();
            removeUnitProduction();
            removeUselessProduction();
            CYK();
            Reverser();

            if (input == "#")
            {
                if (acceptNullInput)
                    System.Console.WriteLine("Accepted");
                else
                    System.Console.WriteLine("Rejected");
            }
            else
            {
                if (CheckString(input))
                    System.Console.WriteLine("Accepted");
                else
                    System.Console.WriteLine("Rejected");
            }
        }
        static bool CheckString(string input)
        {
            try
            {
                for (int subStringSize = 1; subStringSize <= input.Length; subStringSize++)
                {
                    for (int firstIndex = 0; firstIndex <= input.Length - subStringSize; firstIndex++)
                    {
                        string subString = input.Substring(firstIndex, subStringSize);
                        if (p.ContainsKey(subString))
                            continue;
                        if (subString.Length == 1)
                            p.Add(subString, Productions[subString]);
                        else
                        {
                            p.Add(subString, new List<string>());
                            for (int subSubStringLength = 1; subSubStringLength < subString.Length; subSubStringLength++)
                            {
                                string firtPart = subString.Substring(0, subSubStringLength);
                                string secondPart = subString.Substring(subSubStringLength);

                                if (p.ContainsKey(firtPart) && p.ContainsKey(secondPart))
                                {
                                    foreach (string f in p[firtPart])
                                    {
                                        foreach (string c in p[secondPart])
                                        {
                                            if (Productions.ContainsKey(f + c))
                                            {
                                                p[subString].AddRange(Productions[f + c]);
                                            }
                                        }
                                    }
                                }
                            }
                            if (p[subString].Count == 0)
                                p.Remove(subString);
                        }
                    }
                }
                if (p.ContainsKey(input))
                {
                    if (p[input].Contains(startVariable))
                        return true;
                }
                return false;
            }
            //when our input string contains a terminal which grammer doesn't generate.
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        static void Reverser()
        {
            string key;
            string prod;
            for (int i = 0; i < Products.Keys.Count; i++)
            {
                key = Products.Keys.ToList()[i];
                for (int j = 0; j < Products[key].Count; j++)
                {
                    prod = Products[key][j];
                    if (Productions.Keys.Contains(prod))
                    {
                        Productions[prod].Add(key);
                    }
                    else
                    {
                        Productions.Add(prod, new List<string>() { key });
                    }
                }
            }
        }
        static void CYK()
        {
            int num = Products.Keys.Count() + 1;
            List<string> variables = new List<string>();
            variables.AddRange(Products.Keys);

            //this loop replaces all terminals with variables
            //except single terminals such as <S> -> a
            for (int k = 0; k < Products.Keys.Count; k++)
            {
                string key = Products.Keys.ToList()[k];
                for (int index = 0; index < Products[key].Count; index++)
                {
                    if (Products[key][index].Length == 1)
                        continue;
                    string temp = Products[key][index];
                    foreach (string v in variables)
                    {
                        temp = temp.Replace(v, "");
                    }
                    if (temp == "")
                        continue;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        char character = temp[i];
                        if (madeProducts.Keys.Contains(character))
                        {
                            Products[key][index] = Products[key][index].Replace(character.ToString(), madeProducts[character]);
                        }
                        else
                        {
                            string encode = Encoder(ref num);

                            Products.Add(encode, new List<string>() { character.ToString() });

                            Products[key][index] = Products[key][index].Replace(character.ToString(), encode);
                            madeProducts.Add(character, encode);
                        }
                    }
                }
            }

            //<A> -> <A><B><C>
            //is converted to
            //<A> -> <A><X>
            //<X> -> <B><C>
            for (int j = 0; j < Products.Keys.Count; j++)
            {
                string key = Products.Keys.ToList()[j];
                for (int i = 0; i < Products[key].Count(); i++)
                {
                    string product = Products[key][i];
                    if (product.Length == 1)
                        continue;
                    int index = product.IndexOf('<', 1);
                    index = product.IndexOf('<', index + 1);
                    if (index == -1)
                        continue;

                    string newVariable = Encoder(ref num);
                    Products.Add(newVariable, new List<string>()
                                    {product.Substring(product.IndexOf('<',1))}
                    );
                    Products[key][i] = Products[key][i].Remove(product.IndexOf('<', 1)) + newVariable;
                }
            }
        }
        static string Encoder(ref int num)
        {
            string result;
            do
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('<');
                string number = num.ToString();
                foreach (char i in number)
                {
                    sb.Append((char)(int.Parse(i.ToString()) + 65));
                }
                sb.Append('>');
                result = sb.ToString();
                num++;
            } while (Products.Keys.Contains(result));
            return result;
        }
        static void removeUnitProduction()
        {
            bool flagSpecial = false;
            if (!flagSpecial)
            {
                for (int y = 0; y < Products.Keys.Count; y++)
                {
                    string k = Products.Keys.ToList()[y];
                    List<string> addedVariables = new List<string>();
                    for (int i = 0; i < Products[k].Count; i++)
                    {
                        string sentence = Products[k][i];
                        if (addedVariables.Contains(sentence))
                            continue;
                        if (Products.Keys.Contains(sentence))
                        {
                            addedVariables.Add(sentence);
                            Products[k].AddRange(Products[sentence]);
                        }
                    }
                    List<string> temp = new List<string>();
                    foreach (string i in Products[k])
                    {
                        if (!Products.Keys.Contains(i))
                            temp.Add(i);
                    }
                    Products[k] = temp.Distinct().ToList();
                }
            }
            bool flag = false;
            if (flagSpecial)
            {
                do
                {
                    flag = false;
                    foreach (string key in Products.Keys)
                    {
                        int oldCount = Products[key].Where(x => !Products.Keys.Contains(x)).Count();
                        while (Products[key].Contains(key))
                            Products[key].Remove(key);
                        if (Products[key].All(x => x.Contains(key)))
                        {
                            Products.Remove(key);
                        }

                        if (Products.Keys.Contains(key))
                        {
                            foreach (string k in Products.Keys)
                            {
                                if (Products[key].Contains(k))
                                {
                                    Products[key].Remove(k);
                                    Products[key].AddRange(Products[k]);
                                }
                            }
                        }
                        if (Products.Keys.Contains(key))
                        {
                            if (oldCount != Products[key].Where(x => !Products.Keys.Contains(x)).Count())
                            {
                                flag = true;
                            }
                        }
                        else
                            flag = true;
                    }
                } while (flag);
            }
        }
        static void removeNull()
        {
            foreach (string k in Products.Keys)
            {
                if (Products[k].Contains("#"))
                {
                    foreach (string key in Products.Keys)
                    {
                        for (int i = 0; i < Products[key].Count; i++)
                        {
                            string sentence = Products[key][i];
                            if (sentence.Contains(k))
                            {
                                int index = sentence.IndexOf(k);
                                while (index != -1)
                                {
                                    string temp = sentence;
                                    if (temp.Remove(index, k.Length) == "")
                                    {
                                        Products[key].Add("#");
                                    }
                                    else
                                    {
                                        Products[key].Add(temp.Remove(index, k.Length));

                                    }
                                    index = sentence.IndexOf(k, index + 1);
                                }
                            }
                        }
                    }
                }
                if (Products[startVariable].Contains("#"))
                    acceptNullInput = true;
            }
            if (true)
            {
                List<string> temp = new List<string>();
                for (int i = 0; i < Products.Keys.Count; i++)
                {
                    temp.Clear();
                    string key = Products.Keys.ToList()[i];
                    foreach (string j in Products[key])
                    {
                        if (j != "#")
                        {
                            temp.Add(j);
                        }
                    }
                    Products[key] = temp.Distinct().ToList();
                }
            }
        }
        static void removeUselessProduction()
        {
            //removes unreachable Variables
            List<string> ProductsKey = Products.Keys.ToList();
            int ProductsKeyCount = ProductsKey.Count;
            for (int i = 0; i < ProductsKeyCount; i++)
            {
                string key = ProductsKey[i];

                //start variable is always reachable
                if (key == startVariable)
                    continue;

                //determine whether it is non reachable or not
                bool IsItNonReachableVariable = true;
                foreach (string k in ProductsKey)
                {
                    if (k == key)
                        continue;
                    foreach (string prod in Products[k])
                        if (prod.Contains(key))
                        {
                            IsItNonReachableVariable = false;
                            break;
                        }
                    if (!IsItNonReachableVariable)
                        break;
                }

                //removes if it is non reachable
                if (IsItNonReachableVariable)
                {
                    Products.Remove(key);
                }
            }

            //removes unstoppable variables
            for (int i = 0; i < ProductsKeyCount; i++)
            {
                string currentKey = ProductsKey[i];
                if (Products[currentKey].All(x => x.Contains(currentKey)))
                {
                    Products.Remove(currentKey);
                    foreach (string key in Products.Keys)
                    {
                        for (int j = 0; j < Products[key].Count; j++)
                        {
                            if (Products[key][j].Contains(currentKey))
                            {
                                Products[key].Remove(Products[key][j]);
                                j = 0;
                            }
                        }

                    }
                }
            }
        }
        static void ImportData()
        {
            int numberOfLoops = int.Parse(Console.ReadLine());
            for (int i = 0; i < numberOfLoops; i++)
            {
                string[] data = Console.ReadLine().Split(" -> ");
                Products.Add(data[0].Trim(), data[1].Split(" | ").Select(x => x.Trim()).ToList());

                //first variable is assigned as start variable
                if (i == 0)
                    startVariable = data[0].Trim();
            }
            input = Console.ReadLine();
        }
        static Dictionary<string, List<string>> Products = new Dictionary<string, List<string>>();
        static Dictionary<char, string> madeProducts = new Dictionary<char, string>();
        static string input;
        static bool acceptNullInput = false;
        static string startVariable;
        static Dictionary<string, List<string>> Productions = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> p = new Dictionary<string, List<string>>();
    }
}