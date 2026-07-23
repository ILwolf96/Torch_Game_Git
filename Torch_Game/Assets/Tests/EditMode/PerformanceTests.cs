using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Performance Tests — EditMode.
///
/// This benchmarks the physics overlap query used by the torch interaction path.
/// It does not test feature correctness.
//it checks that the hot path stays fast
/// enough when the scene contains many torches.
///
/// for it to work, Have to Place in: Assets/Tests/EditMode/
/// </summary>
public class PerformanceTests
{
    private const int DenseTorchCount = 100;
    private const int OverlapIterations = 2000;
    private const int TimeBudgetMs = 1000;

    private readonly List<GameObject> _spawnedObjects = new();

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject obj in _spawnedObjects)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }

        _spawnedObjects.Clear();
    }

    [Test]
    public void PhysicsOverlap_UsedByTorchInteraction_RemainsWithinBudget()
    {
        CreateDenseTorchField();

        // This is only a sanity check that the torch interaction path is functional.
        GameObject player = CreatePlayer();
        player.transform.position = new Vector3(0.5f, 0f, 0.5f);

        TorchInteraction interaction = player.GetComponent<TorchInteraction>();
        Assert.IsTrue(interaction.TryLightNearbyTorch(), "Setup failed: nearby torch was not detected.");

        var stopwatch = Stopwatch.StartNew();
        int totalHits = 0;

        for (int i = 0; i < OverlapIterations; i++)
        {
            Collider[] hits = Physics.OverlapSphere(
                Vector3.zero,
                2.5f,
                ~0,
                QueryTriggerInteraction.Ignore);

            totalHits += hits.Length;
        }

        stopwatch.Stop();

        Assert.Greater(totalHits, 0, "The dense torch field should produce at least one physics hit.");
        Assert.Less(
            stopwatch.ElapsedMilliseconds,
            TimeBudgetMs,
            $"{OverlapIterations} overlap queries took {stopwatch.ElapsedMilliseconds}ms, over the {TimeBudgetMs}ms budget.");
    }

    private void CreateDenseTorchField()
    {
        // 10x10 grid near the origin so the physics query always has real work to do.
        const float spacing = 0.35f;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0f, z * spacing);
                Torch torch = CreateTorch(position);
                _spawnedObjects.Add(torch.gameObject);
            }
        }
    }

    private Torch CreateTorch(Vector3 position)
    {
        GameObject torchObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torchObject.name = $"PerfTorch_{_spawnedObjects.Count}";
        torchObject.transform.position = position;

        Torch torch = torchObject.AddComponent<Torch>();
        return torch;
    }

    private GameObject CreatePlayer()
    {
        GameObject player = new GameObject("PerformanceTestPlayer");
        player.AddComponent<TorchInteraction>();
        _spawnedObjects.Add(player);
        return player;
    }
}