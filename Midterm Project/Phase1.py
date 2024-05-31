import collections
import json

global jsonFileData 

completedStates = []
discoveredButNotCompletedStates = []
finalStates=[]
originalStates = []
input_symbols = []
originalTransitions={} 
initial_state = ""
final_states = []
Trap="TRAP"

def IsItFinalState(state):
    for finalState in final_states:
        for item in state:
            if finalState==item:
                return True
    return False

def WriteToJSONFile():
    finalAnswer = {}
    finalAnswer["states"] = str(ReturnStatesNames())
    finalAnswer["input_symbols"]=str(ReturnInputsForJsonFile())
    finalAnswer["transitions"]=ReturnTransitions()
    finalAnswer["initial_state"]="q0"
    i=[]
    i.append("q0")
    finalAnswer["final_states"]=ReturnFinalStates()
    print(finalAnswer["final_states"])
    json_object=json.dumps(finalAnswer,indent=4)
    with open("output.json","w") as outfile:
        outfile.write(json_object)

def ReturnFinalStates():
    return "{"+",".join(finalStates)+"}"          

def ReturnInputsForJsonFile():
    i=[]
    for item in input_symbols:
        i.append('\''+item+'\'')
    i="{"+",".join(i)+"}"
    return i

def ReturnTransitions():
    finalAnswer={}
    for item in completedStates:
        name="".join(item["states"])
        tran={}
        for alphabet in item["Transition"]:
            i=[]
            i.append(''.join(item["Transition"][alphabet]))
            tran[alphabet]=str(i[0])
        finalAnswer[name]=tran
    return finalAnswer


def ReturnStatesNames():
    names = []
    for item in completedStates:
        n='\''+"".join(item["states"])+'\''
        if(IsItFinalState(item["states"])):
            finalStates.append(n)
        names.append(n)
   # names = set(names)
    a="{"+",".join(names)+"}"
    return a


#open and read contents of input.json 
def ReadJSONFile():

    jsonFilePath = "input2.json"
    jsonFile = open(jsonFilePath)
    global jsonFileData
    jsonFileData = json.load(jsonFile)
    print(jsonFileData)

#seperate final states
def ReturnFinalStatesList():
    answer = []
    for finalState in eval(jsonFileData["final_states"]):
        answer.append(finalState)
    global final_states
    final_states = answer

#seperate initial state
def ReturnInitialStateString():
    global initial_state
    initial_state = jsonFileData["initial_state"]

# an example of return answer :
# {
#   q0 :
#   {
#       "a" : { "q1" , "q2 "}
#    }
# }

#seperate transitions
def ReturnTransitionsDictionary():
    transitions = jsonFileData["transitions"]

    answer = {}

    for state in transitions.keys():
        t = {}
        for alphabet in transitions[state].keys():
          #      t[alphabet] = eval(transitions[state][alphabet])
            t[alphabet] = []
            for i in eval(transitions[state][alphabet]):
                t[alphabet].append(i)
        answer[state] = t
    global originalTransitions
    originalTransitions = answer

#seperate all states
def ReturnFinalAndNonfinalStatesList():
    global originalStates
    originalStates = eval(jsonFileData["states"])

#seperate input symbols
def ReturnInputSymbolsList():
    global input_symbols
    input_symbols = eval(jsonFileData["input_symbols"])

#initialize original data
def init():
    ReadJSONFile()
    ReturnFinalStatesList()
    ReturnInitialStateString()
    ReturnTransitionsDictionary()
    ReturnFinalAndNonfinalStatesList()
    ReturnInputSymbolsList()


def CheckIsThisStateDiscoveredOrBuilt(state):
    if(IsItBuiltOrNot(state)):
        return False
    if(IsItDiscoveredOrNot(state)):
        return False
    return True

def IsItBuiltOrNot(state):
    for item in completedStates:
        if collections.Counter( list(set(item["states"]))) ==collections.Counter( list(set(state))):
            return True
    return False

def IsItDiscoveredOrNot(state):
    for item in discoveredButNotCompletedStates:
        if collections.Counter( list(set(item))) ==collections.Counter( list(set(state))):
            return True
    return False
def CheckStateExistence(
        allCreatedStates,
        stateToBeChecked):
    for item in allCreatedStates.keys():
        if CheckListsEquality(item, stateToBeChecked):
            return True

    return False


def CheckListsEquality(list1, list2):
    list1.sort()
    list2.sort()
    if list1 == list2:
        return True
    else:
        return False


def ToWhichStatesDoesThisInputStateGoWithDifferentTransitions(
        inputState,
        input_symbols
):

    destinationStatesWithDifferentTransitions = {}
    for alphabet in input_symbols:
        destinationStatesWithDifferentTransitions[alphabet] = []
        for item in inputState["Transitions"]:
            destinationStatesWithDifferentTransitions[alphabet].append(item)
    return destinationStatesWithDifferentTransitions


def CreateNewInitialState(
        states,
        transitions,
        intial_state,
        final_states
):
    newInitialStates = FindLambdaTransitions(intial_state)
    newInitialStates.append(intial_state)
    newTransitions = {}

    for alphabet in input_symbols:
        newTransitions[alphabet] = []

        for state in newInitialStates:
            if alphabet in transitions[state]:
                for obj in transitions[state][alphabet]:
                    newTransitions[alphabet].append(obj)
                    for i in FindLambdaTransitions(obj):
                        newTransitions[alphabet].append(i)
        if (len(newTransitions[alphabet]) == 0):
            newTransitions[alphabet].append(Trap)
        newTransitions[alphabet] = list(set(newTransitions[alphabet]))
        EnqueueState(list(set(newTransitions[alphabet])))

    newInitialState = {
        "states": newInitialStates,
        "Transition": newTransitions
    }
    completedStates.append(newInitialState)
    return newInitialState


def FindLambdaTransitions(
        inputState
):
    if inputState == Trap:
        return []
    states = []
    lambda_symbol = ''
    if lambda_symbol in originalTransitions[inputState]:
        for item in originalTransitions[inputState][lambda_symbol]:
            states += FindLambdaTransitions(item)
            states.append(item)
    return states



def CreateNewNonInitialState(
        states,
        transitions,
        initial_state,
        final_states):
    CreateNewInitialState(states, 
                          transitions, initial_state, final_states)
    while len(discoveredButNotCompletedStates) != 0:
        newState = discoveredButNotCompletedStates.pop()
        e = []
        for k in newState:
            e.append(k)
        for s in newState:
            for item in FindLambdaTransitions(s):
                e.append(item)
        if (CheckIsThisStateDiscoveredOrBuilt(e)):
            it = {}
            it["states"] = list(set(e))
            it["Transition"] = ReturnTransitionsForMultipleInputSymbolAndMultipleStates(
                e)
            if(IsItFinalState(it["states"])):
                it["finalState"]="Yes"
            else:
                it["finalState"]="No"
            completedStates.append(it)
            for item in it["Transition"].keys():
                if it["Transition"][item][0]== Trap:
                    continue
                if(CheckIsThisStateDiscoveredOrBuilt(it["Transition"][item])):
                    discoveredButNotCompletedStates.append(it["Transition"][item])
    WriteToJSONFile()

def EnqueueState(state):
    if not IsItBuiltOrNot(state):
        if not IsItDiscoveredOrNot(state):
            discoveredButNotCompletedStates.append(state)

def ReturnTransitionsForOneInputSymbolAndOneInputState(
        inputState_string,
        inputSymbol_string
):
    answer = []
    if(inputState_string in originalTransitions):
        if(inputSymbol_string in originalTransitions[inputState_string]):
            for item in set(originalTransitions[inputState_string][inputSymbol_string]):
                answer.append(item)
                for i in FindLambdaTransitions(item):
                    answer.append(i)
    return answer

def ReturnReturnTransitionsForOneInputSymbolAndMultipleState(
        inputStates_List,
        inputSymbol_String
):
    answer = []
    for state in inputStates_List:
        list1=ReturnTransitionsForOneInputSymbolAndOneInputState(state,inputSymbol_String)
        for item in list1:
            answer.append(item)
    #to make states unique
    answer= (list(set(answer)))
    EnqueueState(answer)
    return answer

def ReturnTransitionsForMultipleInputSymbolAndMultipleStates(
    inputStates
    ):

    answer={}
    for alphabet in input_symbols:
        answer[alphabet]=ReturnReturnTransitionsForOneInputSymbolAndMultipleState(inputStates,alphabet)
        if(len(answer[alphabet])==0):
            answer[alphabet].append(Trap)
    return answer


init()
print(len(initial_state))
CreateNewNonInitialState(
    originalStates,
    originalTransitions,
    initial_state,
    final_states
)
WriteToJSONFile()