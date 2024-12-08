using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class LogarithmCalculator : MonoBehaviour
{
    public float calculationInterval = 2f; // Интервал вычислений.
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= calculationInterval)
        {
            timer = 0f;
            // Запуск задачи для вычисления логарифмов.
            var randomNumbers = new NativeArray<float>(10, Allocator.TempJob);
            for (int i = 0; i < randomNumbers.Length; i++)
            {
                randomNumbers[i] = Random.Range(1f, 100f);
            }

            var logJob = new LogarithmJob
            {
                numbers = randomNumbers,
                results = new NativeArray<float>(randomNumbers.Length, Allocator.TempJob)
            };

            JobHandle handle = logJob.Schedule(randomNumbers.Length, 1);
            handle.Complete();

            // Выводим результаты в консоль.
            for (int i = 0; i < logJob.results.Length; i++)
            {
                Debug.Log($"Log({logJob.numbers[i]}) = {logJob.results[i]}");
            }

            randomNumbers.Dispose();
            logJob.results.Dispose();
        }
    }

    [BurstCompile]
    struct LogarithmJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> numbers;
        public NativeArray<float> results;

        public void Execute(int index)
        {
            results[index] = Mathf.Log(numbers[index]);
        }
    }
}
