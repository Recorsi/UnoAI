using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class NNImport : MonoBehaviour
{
    public NNModel modelSource;

    IWorker worker;

    void Start()
    {
        var model = ModelLoader.Load(modelSource);
        //var model = ModelLoader.LoadFromStreamingAssets("model" + ".nn");

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        
        //print("This is the output: " + (output[0] < 0.5 ? 0 : 1));
    }

    public double[] CalcNNOutput(float[] inputVal)
    {
        if (inputVal.Length != 7)
            throw new System.Exception();

        var inputTensor = new Tensor(1, 7, inputVal);
        worker.Execute(inputTensor);

        var output = worker.PeekOutput();

        //print output
        //for (int i = 0; i < output.length; i++)
        //    print(output[i]);

        return new double[] { output[0], output[1] };
    }

    //TODO: find way to dispose after turn
    //void OnApplicationQuit()
    //{
    //    inputTensor.Dispose();
    //    output.Dispose();
    //    worker.Dispose();
    //}
}