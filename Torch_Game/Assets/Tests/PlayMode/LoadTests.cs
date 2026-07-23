using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Load Tests — PlayMode.
///
/// This tests realistic high-volume use of the torch system:
/// many torches, many interaction attempts, and repeated successful lighting.
/// It checks that the system remains stable under heavier normal usage.
///
/// Place in: Assets/Tests/PlayMode/
/// </summary>
public class LoadTests
{
    private readonly List<GameObject> _spawnedObjects = new();
    private readonly List<Torch> _torches = new();

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (GameObject obj in _spawnedObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }

        _spawnedObjects.Clear();
        _torches.Clear();

        yield return null;
    }

    [UnityTest]
    public IEnumerator ManyTorches_CanBeLitSequentially_UnderRealisticLoad()
    {
        GameObject player = CreatePlayer();
        TorchInteraction interaction = player.GetComponent<TorchInteraction>();

        const int torchCount = 100;
        const float spacing = 5f;

        for (int i = 0; i < torchCount; i++)
        {
            Torch torch = CreateTorch(new Vector3(i * spacing, 0f, 0f));
            _torches.Add(torch);
        }

        yield return null;

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < _torches.Count; i++)
        {
            Torch torch = _torches[i];
            player.transform.position = torch.transform.position + new Vector3(0f, 0f, 1f);

            bool lit = interaction.TryLightNearbyTorch();
            Assert.IsTrue(lit, $"Torch {i} was not lit during the load test.");
        }

        stopwatch.Stop();

        Assert.Less(
            stopwatch.ElapsedMilliseconds,
            2000,
            $"Lighting {torchCount} torches took {stopwatch.ElapsedMilliseconds}ms, which is too slow for the load test.");

        foreach (Torch torch in _torches)
        {
            Assert.IsTrue(torch.IsLit, "Every torch should be lit after the load test.");
        }
    }

    private GameObject CreatePlayer()
    {
        GameObject player = new GameObject("LoadTestPlayer");
        player.AddComponent<TorchInteraction>();
        _spawnedObjects.Add(player);
        return player;
    }

    private Torch CreateTorch(Vector3 position)
    {
        GameObject torchObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torchObject.name = $"LoadTorch_{_spawnedObjects.Count}";
        torchObject.transform.position = position;

        Torch torch = torchObject.AddComponent<Torch>();
        _spawnedObjects.Add(torchObject);

        return torch;
    }
}