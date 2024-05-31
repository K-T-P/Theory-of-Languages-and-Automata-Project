import json

originalStates = []
input_symbols = []
originalTransitions={} 
initial_state = ""
final_states = []
Trap="TRAP"

def init():
    ReadJSONFile()
    SetFinalStatesAsList()
    SetInitialStateAsString()
    SetTransitionsDictionary()
    SetStatesList()
    SetInputSymbolsList()

def ReadJSONFile():

    jsonFilePath = "input1.json"
    jsonFile = open(jsonFilePath)

    global jsonFileData
    jsonFileData = json.load(jsonFile)

def SetFinalStatesAsList():
    answer = []
    for item in eval(jsonFileData["final_states"]):
        answer.append(item)
    global final_states
    final_states = answer

def SetInitialStateAsString():
    global initial_state
    initial_state = jsonFileData["initial_state"]

def SetTransitionsDictionary():
    transitions = jsonFileData["transitions"]

    answer = {}

    for state in transitions.keys():
        t = {}
        for alphabet in transitions[state].keys():
            t[alphabet] = []
            try:
                for i in eval(transitions[state][alphabet]):
                    t[alphabet].append(i)
            except:
                t[alphabet].append(transitions[state][alphabet])
        answer[state] = t
    global originalTransitions
    originalTransitions = answer

def SetStatesList():
    global originalStates
    originalStates = eval(jsonFileData["states"])

def SetInputSymbolsList():
    global input_symbols
    input_symbols = eval(jsonFileData["input_symbols"])

def CheckString(stri,num,state):
    
    if(num==len(stri) and state in final_states):
        return "Accepted"
    elif(num==len(stri) and state not in final_states):
        return "Rejected"
    
    currentAlphabet=stri[num]
    if(currentAlphabet not in originalTransitions[state] and "" not in originalTransitions[state]):
        return "Rejected"
    else:
        if(currentAlphabet in originalTransitions[state]):
            for sattes in originalTransitions[state][currentAlphabet]:
                ans=CheckString(stri,num+1,sattes)
                if(ans=="Accepted"):
                    return "Accepted"
        if("" in originalTransitions[state]):
            for states in originalTransitions[state][""]:
                ans=CheckString(stri,num,states)
                if(ans=="Accepted"):
                    return "Accepted"
        return "Rejected"
    
init()
print(CheckString("bbb",0,initial_state))