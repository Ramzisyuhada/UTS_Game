using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnsCoin : MonoBehaviour
{
    public GameObject redCoinPrefab;
    public GameObject yellowCoinPrefab;
    public GameObject greenCoinPrefab;
    public GameObject blueCoinPrefab;
    public GameObject blackCoinPrefab;

    public Collider spawnArea;
    public Transform player;
    public float maxDistanceFromPlayer = 30f;

    public TMP_Text Waktu;

    public float waktu = 60f;

    public float minDistanceBetweenCoins = 2f; // jarak minimal antar coin

    private List<Vector3> spawnedPositions = new List<Vector3>(); // daftar posisi yang sudah spawn

    void Start()
    {
        SpawnCoin(redCoinPrefab, 15);
        SpawnCoin(yellowCoinPrefab, 3);
        SpawnCoin(greenCoinPrefab, 2);
        SpawnCoin(blueCoinPrefab, 10);
        SpawnCoin(blackCoinPrefab, 20);
    }

    void SpawnCoin(GameObject coinPrefab, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = GetNonOverlappingPosition();
            Instantiate(coinPrefab, spawnPos, Quaternion.Euler(90, 90, 0));
            spawnedPositions.Add(spawnPos); // simpan posisi coin yang sudah spawn
        }
    }

    Vector3 GetNonOverlappingPosition()
    {
        int maxAttempts = 50; // batas percobaan cari posisi
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 pos = GetRandomPositionNearPlayer();
            if (IsFarEnoughFromOtherCoins(pos))
            {
                return pos;
            }
        }
        // kalau gagal terus, kasih posisi default (boleh diatur mau gimana fallbacknya)
        Debug.LogWarning("Gagal cari posisi tanpa tubrukan, pakai posisi random.");
        return GetRandomPositionNearPlayer();
    }

    bool IsFarEnoughFromOtherCoins(Vector3 pos)
    {
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(pos, existingPos) < minDistanceBetweenCoins)
            {
                return false;
            }
        }
        return true;
    }

    Vector3 GetRandomPositionNearPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * maxDistanceFromPlayer;
        float randomX = player.position.x + randomCircle.x;
        float randomZ = player.position.z + randomCircle.y;

        Bounds bounds = spawnArea.bounds;
        randomX = Mathf.Clamp(randomX, bounds.min.x, bounds.max.x);
        randomZ = Mathf.Clamp(randomZ, bounds.min.z, bounds.max.z);

        Ray ray = new Ray(new Vector3(randomX, bounds.max.y + 10f, randomZ), Vector3.down);
        RaycastHit hit;
        if (spawnArea.Raycast(ray, out hit, 100f))
        {
            return hit.point + Vector3.up * 1f;
        }
        else
        {
            return new Vector3(randomX, bounds.center.y, randomZ);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);

    }

}
