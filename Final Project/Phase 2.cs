using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Phase2_Question2
{
    class Program
    {
        static void Main()
        {
            Stack n = new Stack();
            n.Push("$");
            State initialState = Initialize_PDA();

            if (initialState.ReadString(input, 0, n))
                System.Console.WriteLine("Accepted");
            else
                System.Console.WriteLine("Rejected");
        }
        static string input;
        static void read_Data(
                        out string input_states,
                        out string input_PDA_Alphabet,
                        out string input_StackAlphabet,
                        out string input_finalStates,
                        out string[] input_transitions)
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
            input = Console.ReadLine();
        }
        static void Reformat_Input_Data(
            out string[] reformatted_states,
            out string[] reformatted_PDA_Alphabets,
            out string[] reformatted_Stack_Alphabet,
            out string[] reformatted_final_States,
            out string[] reformatted_transitions,
            string input_states,
            string input_PDA_Alphabet,
            string input_Stack_Alphabet,
            string input_Final_States,
            string[] input_Transitions
        )
        {
            reformatted_states = input_states.Replace("{", "").Replace("}", "").Split(",");
            reformatted_PDA_Alphabets = input_PDA_Alphabet.Replace("{", "").Replace("}", "").Split(",");
            reformatted_Stack_Alphabet = input_Stack_Alphabet.Replace("{", "").Replace("}", "").Split(",");
            reformatted_final_States = input_Final_States.Replace("{", "").Replace("}", "").Split(",");
            reformatted_transitions = input_Transitions
                                    .Select(x => x.Replace("),(", ",").Replace("(", "").Replace(")", "")).ToArray();
        }
        static State Initialize_PDA()
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
            string[] reformatted_States;
            string[] reformatted_PDA_Alphabets;
            string[] reformatted_Stack_Alphabet;
            string[] reformatted_Final_States;
            string[] reformatted_Transitions;
            Reformat_Input_Data(
                out reformatted_States,
                out reformatted_PDA_Alphabets,
                out reformatted_Stack_Alphabet,
                out reformatted_Final_States,
                out reformatted_Transitions,
                input_states,
                input_PDA_Alphabet,
                input_Stack_Alphabet,
                input_Final_States,
                input_Transitions
            );
            Dictionary<string, State> m = new Dictionary<string, State>();
            State[] s = new State[reformatted_States.Count()];
            for (int i = 0; i < reformatted_States.Count(); i++)
            {
                s[i] = new State(reformatted_States[i]);
                m.Add(reformatted_States[i], s[i]);
                if (reformatted_Final_States.Contains(reformatted_States[i]))
                    s[i].finalState = true;
            }
            for (int i = 0; i < reformatted_Transitions.Count(); i++)
            {
                string[] temp = reformatted_Transitions[i].Split(",");
                string currentStateName = temp[0];
                char inputAlphabet = temp[1][0] == '#' ? '#' : (char)temp[1][0];
                char popAlphabet = (char)temp[2][0] == '#' ? '#' : (char)temp[2][0];
                string pushAlphabet = temp[3][0] == '#' ? "#" : temp[3];
                m[temp[0]].StateTransitions.Add(new transition(inputAlphabet, popAlphabet, pushAlphabet, m[temp[4]]));
            }

            return m[reformatted_States[0]];
        }
    }
    class State
    {
        string stateName;
        public bool finalState = false;
        public List<transition> StateTransitions;
        public State(string stateName)
        {
            this.stateName = stateName;
            this.StateTransitions = new List<transition>();
        }
        public bool ReadString(string inputString, int index, Stack currentStack)
        {
            if (inputString != "#")
            {
                inputString = inputString.Length == index ? "#" : inputString;
            }
            if (finalState && inputString == "#")
            {
                return true;
            }
            for (int i = 0; i < StateTransitions.Count(); i++)
            {
                try
                {
                    Stack copyStack = (Stack)currentStack.Clone();
                    int indexAdder = 0;
                    transition t = StateTransitions[i];

                    if (t.inputAlphabet != '#')
                    {
                        if (inputString == "#")
                            continue;
                        if (t.inputAlphabet != inputString[index])
                            continue;
                        indexAdder = 1;
                    }
                    if (t.Stack_Pop_Alphabet != '#')
                    {
                        if (t.Stack_Pop_Alphabet != (char)copyStack.Peek().ToString()[0])
                            continue;
                        copyStack.Pop();
                    }
                    if (t.Stack_Push_Alphabet != "#")
                    {
                        if (t.Stack_Push_Alphabet.Length == 1)
                            copyStack.Push(t.Stack_Push_Alphabet);
                        else
                        {
                            for (int j = t.Stack_Push_Alphabet.Length - 1; j >= 0; j--)
                            {
                                copyStack.Push(t.Stack_Push_Alphabet[j]);
                            }
                        }
                    }
                    if (t.NextState.ReadString(inputString, index + indexAdder, copyStack))
                        return true;
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }
    }

    struct transition
    {
        private char _input_alphabet;
        private char _stack_pop_alphabet;
        private string _stack_push_alphabet;
        private State _nextState;
        public char inputAlphabet
        {
            get => _input_alphabet;
        }
        public string Stack_Push_Alphabet
        {
            get => _stack_push_alphabet;
        }
        public char Stack_Pop_Alphabet
        {
            get => _stack_pop_alphabet;
        }
        public State NextState
        {
            get => _nextState;
        }
        public transition(char in_alp, char stack_pop_a, string stack_push, State nextState)
        {
            this._input_alphabet = in_alp;
            this._stack_pop_alphabet = stack_pop_a;
            this._stack_push_alphabet = stack_push;
            this._nextState = nextState;
        }
    }
}