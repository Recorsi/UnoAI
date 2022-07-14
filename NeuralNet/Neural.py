from sympy import Range
import torch
import torch.nn as nn
import matplotlib.pyplot as plt

#the inital setup
n_input, n_hidden, n_out, batch_size, learning_rate = 7, 15, 2, 7, 0.01
accDataAmount = 10
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
        newArrayStr = [value for key, value in enumerate(parts) if float(key) < 7] #[parts[0],parts[1],parts[2],parts[3],parts[4],parts[5],parts[6]]
        newArrayOutStr = [value for key, value in enumerate(parts) if float(key) >= 7]
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
                      nn.Linear(n_hidden-6, n_out),
                      nn.Sigmoid())

if(debug):
    print(model)

#define how we calc loss
loss_function = nn.MSELoss()
optimizer = torch.optim.SGD(model.parameters(), lr=learning_rate)

#train the network

losses = []
for epoch in range(3):
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
if(debug):
    print("Finding accuracy")

for key, value in enumerate(data_inputArratAcc):
    pred_y = model(torch.tensor(value))
    outP1 = torch.tensor(pred_y)[0]
    outP2 = torch.tensor(pred_y)[1]
    
    if(outP1 < data_outputArratAcc[key][0]):
        accuracyProp += data_outputArratAcc[key][0] - outP1
    else:
        accuracyProp += outP1 - data_outputArratAcc[key][0]

    if(outP2 < data_outputArratAcc[key][1]):
        accuracyProp += data_outputArratAcc[key][1] - outP2
    else:
        accuracyProp += outP2 - data_outputArratAcc[key][1]

accuracyProp = (accuracyProp/(accDataAmount*2))

#torch.save(model, "C:\My programs\Git\MyNameIsJeff");

#debug stuff
if(debug):
    #print ("loss was: ")
    #print (float(accuracyProp))
    print ("accuracy was: " + str(float(accuracyProp)))

    plt.plot(losses)
    plt.ylabel('loss')
    plt.xlabel('iterations')
    plt.title("Learning rate %f"%(learning_rate))
    plt.show()    

torch.onnx.export(model,                       # model being run
                  torch.tensor([float(1),float(1),float(1),float(1),float(1),float(1),float(1)]),                         # model input (or a tuple for multiple inputs)
                  pathHere + "model.onnx",            # where to save the model (can be a file or file-like object)
                  export_params=True,        # store the trained parameter weights inside the model file
                  opset_version=9,           # the ONNX version to export the model to
                  do_constant_folding=True,  # whether to execute constant folding for optimization
                  input_names = ['X'],       # the model's input names
                  output_names = ['Y']       # the model's output names
                  )


#torch.save(model.state_dict(), pathHere + 'model.pt')