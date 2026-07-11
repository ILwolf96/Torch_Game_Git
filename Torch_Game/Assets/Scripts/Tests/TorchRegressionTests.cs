using NUnit.Framework;
using UnityEngine;

public class TorchRegressionTests
{
    private GameObject torchObject;
    private Torch torch;

    [SetUp]
    public void SetUp()
    {
        torchObject = new GameObject("Torch");
        torch = torchObject.AddComponent<Torch>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(torchObject);
    }

    [Test]
    public void LightTorch_CalledMultipleTimes_DoesNotTurnTorchOff()
    {
        torch.LightTorch();
        torch.LightTorch();
        torch.LightTorch();

        Assert.IsTrue(torch.IsLit);
    }
}