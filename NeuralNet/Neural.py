from sympy import Range
import torch
import torch.nn as nn
import matplotlib.pyplot as plt

device = "cude" if torch.cuda.is_available() else "cpu"

#the inital setup
n_input, n_hidden, n_out, batch_size, learning_rate = 7, 15, 2, 7, 0.01
debug = 1

#input array
data_x = torch.randn(batch_size, n_input)
data_inputArrat = []
#output array
data_y = (torch.rand(size=(batch_size, n_out)) < 0.5).float()
data_outputArrat = []

if(debug):
    print(data_x.size())
    print(data_y.size())

def loadData():
    file1 = open('C:/My programs/Git/UnoAI/NeuralNet/DataSaved.txt', 'r')
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

        data_inputArrat.append(newArray)
        data_outputArrat.append(newArrayOut)
        if(debug):
            print(newArray)
            print(newArrayOut)
                

    #code to load our data into datax and datay
    return

loadData()

#create the network
model = nn.Sequential(nn.Linear(n_input, n_hidden),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden, n_hidden),
                      nn.Sigmoid(),
                      nn.Linear(n_hidden, n_out),
                      nn.Sigmoid())
model.to(device)

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
        print(device)

        model.zero_grad()
        loss.backward()

        optimizer.step()
    

#torch.save(model, "C:\My programs\Git\MyNameIsJeff");

#debug stuff
if(debug):
    plt.plot(losses)
    plt.ylabel('loss')
    plt.xlabel('iterations')
    plt.title("Learning rate %f"%(learning_rate))
    plt.show()
