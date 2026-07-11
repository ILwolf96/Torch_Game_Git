using NUnit.Framework;
using UnityEngine;

public class TorchFunctionalTests
{
    private GameObject playerObject;
    private TorchInteraction interaction;
    private GameObject torchObject;
    private Torch torch;
    private Camera cameraObject;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject("Player");
        playerObject.AddComponent<CharacterController>();
        playerObject.AddComponent<FPSController>();
        interaction = playerObject.AddComponent<TorchInteraction>();

        cameraObject = new GameObject("Main Camera").AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(playerObject.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);

        torchObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torchObject.name = "Torch";
        torchObject.layer = LayerMask.NameToLayer("Torch");
        torchObject.transform.position = new Vector3(1f, 0f, 0f);
        torch = torchObject.AddComponent<Torch>();

        interaction.transform.position = Vector3.zero;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(torchObject);
        if (cameraObject != null)
            Object.DestroyImmediate(cameraObject.gameObject);
    }

    [Test]
    public void FullTorchInteractionFlow_Works()
    {
        bool lit = interaction.TryLightNearbyTorch();

        Assert.IsTrue(lit);
        Assert.IsTrue(torch.IsLit);
    }
}