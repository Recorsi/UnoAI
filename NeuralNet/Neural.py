from sympy import Range
import torch
import torch.nn as nn
import matplotlib.pyplot as plt

#the inital setup
n_input, n_hidden, n_out, batch_size, learning_rate = 10, 20, 4, 7, 0.01
accDataAmount = 100
debug = 1
pathHere = 'C:/My programs/Git/UnoAI/NeuralNet/'

#input array
data_x = torch.randn(batch_size, n_input)
data_inputArrat = []
data_inputArratAcc = []
#output array
data_y = (torch.rand(size=(batch_size, n_out)) < 0.5).float()
data_outputArrat = []
data_outputArratAcc = []

if(debug):
    print(data_x.size())
    print(data_y.size())

def loadData():
    file1 = open(pathHere + 'DataSaved.txt', 'r')
    Lines = file1.readlines()
    count = 0
    for line in Lines:
        count += 1
        line = line.replace(",", ".")
        parts = line.split("-;")
        parts[len(parts)-1] = parts[len(parts)-1].rstrip('\n')
        sCount = 0
        newArrayStr = [value for key, value in enumerate(parts) if float(key) < 10] #[parts[0],parts[1],parts[2],parts[3],parts[4],parts[5],parts[6]]
        newArrayOutStr = [value for key, value in enumerate(parts) if float(key) >= 10]
        newArray = []
        newArrayOut = []

        for key in Range(len(newArrayStr)):
            newArray.append(float(newArrayStr[key]))
        
        for key in Range(len(newArrayOutStr)):
            newArrayOut.append(float(newArrayOutStr[key]))

        if(count + accDataAmount < len(Lines)):
            data_inputArrat.append(newArray)
            data_outputArrat.append(newArrayOut)
        
        else:
            data_inputArratAcc.append(newArray)
            data_outputArratAcc.append(newArrayOut)
        
        if(debug):
            print(newArray)
            print(newArrayOut)
                

    #code to load our data into datax and datay
    return

loadData()

#create the network
model = nn.Sequential(nn.Linear(n_input, n_hidden),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden, n_hidden-2),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden-2, n_hidden-4),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden-4, n_hidden-6),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden-6, n_hidden-8),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden-8, n_hidden-10),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden-10, n_out),
                      nn.Sigmoid())

if(debug):
    print(model)

#define how we calc loss
loss_function = nn.MSELoss()
optimizer = torch.optim.SGD(model.parameters(), lr=learning_rate)

#train the network

losses = []
for epoch in range(5000):
    for key, value in enumerate(data_inputArrat):
        pred_y = model(torch.tensor(value))
        loss = loss_function(pred_y, torch.tensor(data_outputArrat[key]))
        losses.append(loss.item())
        print(loss)
        print(epoch)

        model.zero_grad()
        loss.backward()

        optimizer.step()
    
accuracyProp = 0

accuracyProp1 = 0
accuracyProp2 = 0
accuracyProp3 = 0
accuracyProp4 = 0

diviationProp1 = 0
diviations1 = []
diviationProp2 = 0
diviations2 = []
diviationProp3 = 0
diviations3 = []
diviationProp4 = 0
diviations4 = []

if(debug):
    print("Finding accuracy")

def sumDiviation(outP, num, key):
    if(0.5 < data_outputArratAcc[key][num]):
        if(0.5 > outP):
            return sumUpAccuracy(outP, num, key)
    if(0.5 > data_outputArratAcc[key][num]):
        if(0.5 < outP):
            return sumUpAccuracy(outP, num, key)
    
    return float(0.5)
        

def sumUpAccuracy(outP, num, key):
    if(outP < data_outputArratAcc[key][num]):
        return data_outputArratAcc[key][num] - outP
    else:
        return outP - data_outputArratAcc[key][num]

for key, value in enumerate(data_inputArratAcc):
    pred_y = model(torch.tensor(value))
    outP1 = torch.tensor(pred_y)[0]
    outP2 = torch.tensor(pred_y)[1]
    outP3 = torch.tensor(pred_y)[2]
    outP4 = torch.tensor(pred_y)[3]
    
    accuracyProp1 += sumUpAccuracy(outP1, 0, key)
    diviationProp1 += sumDiviation(outP1, 0, key)
    diviations1.append(sumDiviation(outP1, 0, key))
    
    accuracyProp2 += sumUpAccuracy(outP2, 1, key)
    diviationProp2 += sumDiviation(outP2, 1, key)
    diviations2.append(sumDiviation(outP2, 1, key))

    accuracyProp3 += sumUpAccuracy(outP3, 2, key)
    diviationProp3 += sumDiviation(outP3, 2, key)
    diviations3.append(sumDiviation(outP3, 2, key))

    accuracyProp4 += sumUpAccuracy(outP4, 3, key)
    diviationProp4 += sumDiviation(outP4, 3, key)
    diviations4.append(sumDiviation(outP4, 3, key))

    accuracyProp = accuracyProp1 + accuracyProp2 + accuracyProp3 + accuracyProp4


accuracyProp = (accuracyProp/(accDataAmount*4))
accuracyProp1 = (accuracyProp1/(accDataAmount))
accuracyProp2 = (accuracyProp2/(accDataAmount))
accuracyProp3 = (accuracyProp3/(accDataAmount))
accuracyProp4 = (accuracyProp4/(accDataAmount))

diviationProp1 = (diviationProp1/(accDataAmount))
diviationProp2 = (diviationProp2/(accDataAmount))
diviationProp3 = (diviationProp3/(accDataAmount))
diviationProp4 = (diviationProp4/(accDataAmount))

#torch.save(model, "C:\My programs\Git\MyNameIsJeff");

#debug stuff
if(debug):
    #print ("loss was: ")
    #print (float(accuracyProp))
    print ("accuracy was: " + str(float(accuracyProp)))
    print ("en turn accuracy was: " + str(float(accuracyProp1)))
    print(diviationProp1)
    print ("ai turn accuracy was: " + str(float(accuracyProp2)))
    print(diviationProp2)
    print ("ai win accuracy was: " + str(float(accuracyProp3)))
    print(diviationProp3)
    print ("en win accuracy was: " + str(float(accuracyProp4)))
    print(diviationProp4)

    plt.plot(diviationProp3)
    plt.ylabel('diviation')
    plt.xlabel('iterations')
    plt.title("Average diviation rate3: %f"%(diviationProp3))
    plt.show()   

    plt.plot(diviationProp4)
    plt.ylabel('diviation')
    plt.xlabel('iterations')
    plt.title("Average diviation rate4: %f"%(diviationProp4))
    plt.show()   
    
    plt.plot(losses)
    plt.ylabel('loss')
    plt.xlabel('iterations')
    plt.title("Learning rate %f"%(learning_rate))
    plt.show()    

torch.onnx.export(model,                       # model being run
                  torch.tensor([float(1),float(1),float(1),float(1),float(1),float(1),float(1),float(1),float(1),float(1)]),                         # model input (or a tuple for multiple inputs)
                  pathHere + "model.onnx",            # where to save the model (can be a file or file-like object)
                  export_params=True,        # store the trained parameter weights inside the model file
                  opset_version=9,           # the ONNX version to export the model to
                  do_constant_folding=True,  # whether to execute constant folding for optimization
                  input_names = ['X'],       # the model's input names
                  output_names = ['Y']       # the model's output names
                  )


#torch.save(model.state_dict(), pathHere + 'model.pt')