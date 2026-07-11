using NUnit.Framework;
using UnityEngine;

public class TorchSmokeTests
{
    [Test]
    public void MainSceneComponents_ArePresent()
    {
        GameObject player = GameObject.Find("Player");
        GameObject torch1 = GameObject.Find("Torch_01");
        GameObject torch2 = GameObject.Find("Torch_02");
        GameObject torch3 = GameObject.Find("Torch_03");

        Assert.IsNotNull(player, "Player object is missing.");
        Assert.IsNotNull(torch1, "Torch_01 object is missing.");
        Assert.IsNotNull(torch2, "Torch_02 object is missing.");
        Assert.IsNotNull(torch3, "Torch_03 object is missing.");

        Assert.IsNotNull(player.GetComponent<FPSController>(), "Player is missing FPSController.");
        Assert.IsNotNull(player.GetComponent<TorchInteraction>(), "Player is missing TorchInteraction.");
        Assert.IsNotNull(torch1.GetComponent<Torch>(), "Torch_01 is missing Torch.");
        Assert.IsNotNull(torch2.GetComponent<Torch>(), "Torch_02 is missing Torch.");
        Assert.IsNotNull(torch3.GetComponent<Torch>(), "Torch_03 is missing Torch.");
    }
}