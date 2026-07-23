using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Stress Tests — PlayMode.
///
/// This deliberately pushes beyond normal usage:
/// - extreme player positions
/// - dense clusters of torches
/// - repeated interaction attempts
/// - object churn during runtime
///
/// The goal is not feature correctness.
/// The goal is to ensure the system fails gracefully and does not corrupt state.
///
/// Place in: Assets/Tests/PlayMode/
/// </summary>
public class StressTests
{
    private readonly List<GameObject> _spawnedObjects = new();
    private readonly List<Torch> _stableTorches = new();

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
        _stableTorches.Clear();

        yield return null;
    }

    [UnityTest]
    public IEnumerator RepeatedInteraction_WithExtremePositions_AndObjectChurn_RemainsStable()
    {
        GameObject player = CreatePlayer();
        TorchInteraction interaction = player.GetComponent<TorchInteraction>();

        // Dense torch cluster near origin.
        for (int x = 0; x < 15; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                Vector3 position = new Vector3(x * 0.35f, 0f, z * 0.35f);
                Torch torch = CreateTorch(position);
                _stableTorches.Add(torch);
            }
        }

        // Far-away torches to increase physics workload and spatial range.
        for (int i = 0; i < 50; i++)
        {
            Torch torch = CreateTorch(new Vector3(5000f + i, 0f, -5000f - i));
            _stableTorches.Add(torch);
        }

        yield return null;

        Vector3[] stressPositions =
        {
            Vector3.zero,
            new Vector3(10000f, 0f, 10000f),
            new Vector3(-9999f, 0f, 9999f),
            new Vector3(1.9f, 0f, 1.9f)
        };

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 500; i++)
        {
            player.transform.position = stressPositions[i % stressPositions.Length];

            Assert.DoesNotThrow(
                () => interaction.TryLightNearbyTorch(),
                $"Interaction threw an exception at iteration {i}.");

            // Object churn: create and remove a temporary torch during the stress loop.
            if (i % 50 == 0)
            {
                Torch transientTorch = CreateTorch(player.transform.position + new Vector3(0.5f, 0f, 0.5f));
                _spawnedObjects.Add(transientTorch.gameObject);

                Object.Destroy(transientTorch.gameObject);
                yield return null;
            }
        }

        stopwatch.Stop();

        Assert.Less(
            stopwatch.ElapsedMilliseconds,
            5000,
            $"Stress loop took {stopwatch.ElapsedMilliseconds}ms, which is too slow for the stress test.");

        Assert.IsFalse(float.IsNaN(player.transform.position.x));
        Assert.IsFalse(float.IsNaN(player.transform.position.y));
        Assert.IsFalse(float.IsNaN(player.transform.position.z));

        Assert.IsFalse(float.IsInfinity(player.transform.position.x));
        Assert.IsFalse(float.IsInfinity(player.transform.position.y));
        Assert.IsFalse(float.IsInfinity(player.transform.position.z));

        foreach (Torch torch in _stableTorches)
        {
            Assert.IsFalse(float.IsNaN(torch.transform.position.x));
            Assert.IsFalse(float.IsNaN(torch.transform.position.y));
            Assert.IsFalse(float.IsNaN(torch.transform.position.z));
        }
    }

    private GameObject CreatePlayer()
    {
        GameObject player = new GameObject("StressTestPlayer");
        player.AddComponent<TorchInteraction>();
        _spawnedObjects.Add(player);
        return player;
    }

    private Torch CreateTorch(Vector3 position)
    {
        GameObject torchObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torchObject.name = $"StressTorch_{_spawnedObjects.Count}";
        torchObject.transform.position = position;

        Torch torch = torchObject.AddComponent<Torch>();
        return torch;
    }
}