using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TorchIntegrationTests
{
    private GameObject playerObject;
    private TorchInteraction interaction;
    private GameObject torchObject;
    private Torch torch;
    private LayerMask torchLayer;

    [SetUp]
    public void SetUp()
    {
        torchLayer = LayerMask.NameToLayer("Torch");

        playerObject = new GameObject("Player");
        interaction = playerObject.AddComponent<TorchInteraction>();

        torchObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torchObject.name = "Torch";
        torchObject.layer = LayerMask.NameToLayer("Torch");

        torch = torchObject.AddComponent<Torch>();

        interaction.transform.position = Vector3.zero;
        torchObject.transform.position = new Vector3(1f, 0f, 0f);

        // Keep the overlap radius default-friendly
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(torchObject);
    }

    [Test]
    public void PressingInteraction_LightsNearbyTorch()
    {
        bool result = interaction.TryLightNearbyTorch();

        Assert.IsTrue(result);
        Assert.IsTrue(torch.IsLit);
    }
}