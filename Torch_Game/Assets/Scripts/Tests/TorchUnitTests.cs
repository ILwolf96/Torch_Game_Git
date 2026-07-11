using NUnit.Framework;
using UnityEngine;

public class TorchUnitTests
{
    private GameObject torchObject;
    private Torch torch;
    private GameObject rendererObject;
    private Light torchLight;

    [SetUp]
    public void SetUp()
    {
        torchObject = new GameObject("Torch");
        torch = torchObject.AddComponent<Torch>();

        rendererObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rendererObject.name = "TorchVisual";
        rendererObject.transform.SetParent(torchObject.transform);
        torchLight = rendererObject.AddComponent<Light>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(torchObject);
        Object.DestroyImmediate(rendererObject);
    }

    [Test]
    public void LightTorch_SetsIsLitToTrue()
    {
        Assert.IsFalse(torch.IsLit);

        torch.LightTorch();

        Assert.IsTrue(torch.IsLit);
    }

    [Test]
    public void LightTorch_ChangesTorchColorToYellow()
    {
        Renderer renderer = rendererObject.GetComponent<Renderer>();
        Color litColor = Color.yellow;

        torch.LightTorch();

        Assert.AreEqual(litColor, renderer.material.color);
    }

    [Test]
    public void LightTorch_EnablesTorchLight()
    {
        Assert.IsFalse(torchLight.enabled);

        torch.LightTorch();

        Assert.IsTrue(torchLight.enabled);
    }
}