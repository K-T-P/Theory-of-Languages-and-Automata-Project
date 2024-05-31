//این برنامه اطلاعات را از فایل 
//input.txt
//می‌خواند و جواب را در فایل 
//finalAnswer.txt
//می‌نویسد 
//اطلاعات را به همان فرمتی که در کوئرا اشاره شده است
//در فایل
//input.txt
//وارد کنید
//اگر خواستید دستی از کنسول وارد کنید
//به تابع
//read_Data
//بروید. مابقی موارد در آنجا نوشته شده است.
//اگر بخواهید خروجی را در کنسول چاپ کنید، به تابع
//PrintAnswer
//بروید. مابقی موارد در آنجا نوشته شده است.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ThirdQuestion
{
    class Program
    {
        static void Main()
        {
            string input_states;
            string input_PDA_Alphabet;
            string input_Stack_Alphabet;
            string input_Final_States;
            string[] input_Transitions;
            read_Data(
                out input_states,
                out input_PDA_Alphabet,
                out input_Stack_Alphabet,
                out input_Final_States,
                out input_Transitions);
            Reformat_Input_Data(
                input_states,
                input_PDA_Alphabet,
                input_Stack_Alphabet,
                input_Final_States,
                input_Transitions
            );
            // بررسی می‌کند آیا نیاز به تغییر فاینال استیت‌ها هست یا خیر
            if (AddStackEmptyTransitionOrNot())
                finalStateModifier();

            ModifyLambdaPopTransitions();
            ModifyImproperPushTransitions();

            //برای حذف ترنزیشن‌های تکراری که ممکت است تولید شوند
            RemoveDuplicate();
            GenerateProductions();

            //متغیر شروع را پیدا می‌کند که در اول فایل چاپ کند
            FindStartVariable();
            FormatToPrint();
            PrintAnswer();
        }
        static bool AddStackEmptyTransitionOrNot()
        {
            try
            {
                if (reformatted_final_States.Count != 1)
                    return true;
                var transitions = reformatted_transitions
                                .Where(x => x.Split(",")[4] == reformatted_final_States[0]);
                foreach (string transition in transitions)
                {
                    string[] tr = transition.Split(",");
                    if (tr[2] != "$" || tr[3] != "#")
                        return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
        static void read_Data(
                        out string input_states,
                        out string input_PDA_Alphabet,
                        out string input_StackAlphabet,
                        out string input_finalStates,
                        out string[] input_transitions)
        {
            //اگر خواستید ورودی‌هارا از کنسول وارد کنید
            //false
            //را به
            //true
            //تغییر دهید
            if (false)
            {
                input_states = Console.ReadLine().Trim();
                input_PDA_Alphabet = Console.ReadLine().Trim();
                input_StackAlphabet = Console.ReadLine().Trim();
                input_finalStates = Console.ReadLine().Trim();
                int j = int.Parse(Console.ReadLine().Trim());
                input_transitions = new string[j];
                for (int i = 0; i < j; i++)
                {
                    input_transitions[i] = Console.ReadLine().Trim();
                }
            }
            else
            {
                StreamReader reader = new StreamReader("input.txt");
                input_states = reader.ReadLine().Trim();
                input_PDA_Alphabet = reader.ReadLine().Trim();
                input_StackAlphabet = reader.ReadLine().Trim();
                input_finalStates = reader.ReadLine().Trim();
                int j = int.Parse(reader.ReadLine().Trim());
                input_transitions = new string[j];
                for (int i = 0; i < j; i++)
                {
                    input_transitions[i] = reader.ReadLine().Trim();
                }
                reader.Close();
            }
        }
        static void RemoveDuplicate()
        {
            reformatted_transitions = reformatted_transitions.Distinct().ToList();
        }
        static void PrintAnswer()
        {
            //اگر خواستید اطلاعات در کنسول چاپ شود 
            //true
            //را به
            //false
            //تغییر دهید.
            if (true)
            {
                StreamWriter writer = new StreamWriter("finalAnswer.txt");
                foreach (string production in transitions)
                    writer.WriteLine(production);
                writer.Close();
            }
            else
            {
                foreach (string production in transitions)
                    Console.WriteLine(production);
            }
        }
        static string startVariable;
        static string[] transitions;
        static void FormatToPrint()
        {
            transitions = new string[Productions.Keys.Count];
            transitions[0] = startVariable + " -> " + string.Join(" | ", Productions[startVariable]);
            Productions.Remove(startVariable);
            for (int i = 0; i < Productions.Keys.Count; i++)
            {
                string variable = Productions.Keys.ToList()[i];
                transitions[i + 1] = variable + " -> " + string.Join(" | ", Productions[variable]);
            }
        }
        static void FindStartVariable()
        {
            string startState = reformatted_states[0];
            string finalState = reformatted_final_States[0];
            startVariable = "(" + startState + "$" + finalState + ")";
            if (!Productions.ContainsKey(startVariable))
                throw new Exception("Start variable not found!");
        }
        static void GenerateProductions()
        {
            string newVariable;
            for (int i = 0; i < reformatted_transitions.Count; i++)
            {
                string[] transition = reformatted_transitions[i].Split(",");
                if (transition[3] == "#")
                {
                    newVariable = "("
                                    + transition[0] + transition[2] + transition[4]
                                    + ")";
                    if (Productions.ContainsKey(newVariable))
                        Productions[newVariable].Add(transition[1]);
                    else
                    {
                        Productions.Add(newVariable, new List<string>() { transition[1] });
                    }
                }
                else
                {
                    string newProduct;
                    for (int j = 0; j < reformatted_states.Count; j++)
                    {
                        for (int k = 0; k < reformatted_states.Count; k++)
                        {
                            string firstState = reformatted_states[j];
                            string secondState = reformatted_states[k];
                            string v = transition[3].Length == 1 ? transition[3] : transition[3][0].ToString();
                            newVariable = "("
                            + transition[0] + transition[2] + firstState + ")";
                            newProduct = transition[1] + "(" + transition[4] + v
                            + secondState + ")(" + secondState + transition[3][1].ToString() +
                            firstState + ")";
                            if (Productions.ContainsKey(newVariable))
                                Productions[newVariable].Add(newProduct);
                            else
                                Productions.Add(newVariable, new List<string>() { newProduct });
                        }
                    }
                }
            }
        }
        static void ModifyImproperPushTransitions()
        {
            for (int i = 0; i < reformatted_transitions.Count; i++)
            {
                string[] transition = reformatted_transitions[i].Split(",");
                if (transition[3].Length == 1)
                {
                    if (transition[3] != "#")
                    {
                        ModifyOneAlphabetPushTransition(transition);
                        i = 0;
                    }
                }
                if (transition[3].Length >= 3)
                {
                    ModifyLongAlphabetPushTransition(transition);
                    i = 0;
                }
            }
        }
        static void ModifyLongAlphabetPushTransition(string[] transition)
        {
            if (transition[3].Length == 2)
            {
                reformatted_transitions.Add(string.Join(",", transition));
                return;
            }
            reformatted_transitions.Remove(string.Join(",", transition));
            string newTransition;
            string newState = StateGenerator();
            reformatted_states.Add(newState);
            string pushAlphabet = transition[3][0].ToString();
            foreach (string stackAlphabet in reformatted_Stack_Alphabet)
            {
                if (stackAlphabet == "$")
                    continue;
                newTransition = newState + ",#," + stackAlphabet + "," + pushAlphabet +
                stackAlphabet + "," + transition[4];
                reformatted_transitions.Add(newTransition);
            }
            string[] convertRecursivelyTransition = new string[5];
            convertRecursivelyTransition[0] = transition[0];
            convertRecursivelyTransition[1] = transition[1];
            convertRecursivelyTransition[2] = transition[2];
            convertRecursivelyTransition[3] = transition[3].Substring(1);
            convertRecursivelyTransition[4] = newState;
            ModifyLongAlphabetPushTransition(convertRecursivelyTransition);
        }
        static void ModifyOneAlphabetPushTransition(string[] transition)
        {
            reformatted_transitions.Remove(string.Join(",", transition));
            string randomAlphabet = reformatted_Stack_Alphabet[0];
            string newState = StateGenerator();
            string newTransition1 = transition[0]
                                        + ","
                                        + transition[1]
                                        + ","
                                        + transition[2]
                                        + ","
                                        + randomAlphabet
                                        + transition[3]
                                        + ","
                                        + newState;
            string newTransition2 = newState
                                        + ","
                                        + "#"
                                        + ","
                                        + randomAlphabet
                                        + ","
                                        + "#"
                                        + ","
                                        + transition[4];
            reformatted_transitions.Add(newTransition1);
            reformatted_transitions.Add(newTransition2);
            reformatted_states.Add(newState);
        }
        static void ModifyLambdaPopTransitions()
        {
            string[] temp;
            for (int i = 0; i < reformatted_transitions.Count; i++)
            {
                string[] transition = reformatted_transitions[i].Split(",");
                if (transition[2] != "#")
                    continue;
                foreach (string stackAlphabet in reformatted_Stack_Alphabet)
                {
                    string pushAlphabet = transition[3] == "#"
                                            ? stackAlphabet
                                            : transition[3] + stackAlphabet;
                    temp = new string[5]
                        {transition[0],transition[1],stackAlphabet,pushAlphabet,transition[4]};
                    reformatted_transitions.Add(string.Join(",", temp));
                    reformatted_transitions.Remove(reformatted_transitions[i]);
                }
                i = 0;
            }
        }
        static string StateGenerator()
        {
            string newState;
            do
            {
                newState = "q" + numberOfStates.ToString();
                numberOfStates++;
            } while (reformatted_states.Contains(newState));
            reformatted_states.Add(newState);
            return newState;
        }
        static void finalStateModifier()
        {
            string newNonFinalState = StateGenerator();
            string newFinalState = StateGenerator();
            string newTransition;
            for (int i = 0; i < reformatted_final_States.Count; i++)
            {
                string finalState = reformatted_final_States[i];
                newTransition = finalState + ",#,#,#," + newNonFinalState;
                reformatted_transitions.Add(newTransition);
                reformatted_final_States.Remove(finalState);
                i = 0;
            }
            foreach (string stackAlphabet in reformatted_Stack_Alphabet)
            {
                if (stackAlphabet == "$")
                    continue;
                newTransition = newNonFinalState
                                + ",#,"
                                + stackAlphabet
                                + ",#,"
                                + newNonFinalState;
                reformatted_transitions.Add(newTransition);
            }
            newTransition = newNonFinalState + ",#,$,#," + newFinalState;
            reformatted_transitions.Add(newTransition);
            reformatted_final_States.Add(newFinalState);
        }
        static int numberOfStates = 0;
        static void Reformat_Input_Data(

            string input_states,
            string input_PDA_Alphabet,
            string input_Stack_Alphabet,
            string input_Final_States,
            string[] input_Transitions
        )
        {
            reformatted_states = input_states.Replace("{", "").Replace("}", "").Split(",").ToList();
            numberOfStates = reformatted_states.Count + 1;
            reformatted_PDA_Alphabets = input_PDA_Alphabet.Replace("{", "").Replace("}", "").Split(",").ToList();
            reformatted_Stack_Alphabet = input_Stack_Alphabet.Replace("{", "").Replace("}", "").Split(",").ToList();
            reformatted_final_States = input_Final_States.Replace("{", "").Replace("}", "").Split(",").ToList();
            reformatted_transitions = input_Transitions
                                    .Select(x => x.Replace("),(", ",").Replace("(", "").Replace(")", "")).ToList();
        }
        static List<string> reformatted_states;
        static List<string> reformatted_PDA_Alphabets;
        static List<string> reformatted_Stack_Alphabet;
        static List<string> reformatted_final_States;
        static List<string> reformatted_transitions;
        static Dictionary<string, List<string>> Productions = new Dictionary<string, List<string>>();
    }
}