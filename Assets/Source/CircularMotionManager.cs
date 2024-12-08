using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

public class CircularMotionManager : MonoBehaviour
{
    public GameObject prefab; // Префаб с загруженным мешем.
    public int spawnCount = 10; // Количество объектов.
    public float radius = 5f; // Радиус движения.
    public float speed = 2f; // Скорость движения.

    private TransformAccessArray transforms;

    void Start()
    {
        // Спавним объекты и добавляем их в TransformAccessArray.
        Transform[] spawnedTransforms = new Transform[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject instance = Instantiate(prefab, Random.insideUnitSphere * radius, Quaternion.identity);
            spawnedTransforms[i] = instance.transform;
        }
        transforms = new TransformAccessArray(spawnedTransforms);
    }

    void Update()
    {
        // Создаём задачу для движения по кругу.
        var circularJob = new CircularMotionJob
        {
            deltaTime = Time.deltaTime,
            speed = speed,
            radius = radius
        };

        JobHandle handle = circularJob.Schedule(transforms);
        handle.Complete();
    }

    void OnDestroy()
    {
        // Обязательно освобождаем ресурсы.
        transforms.Dispose();
    }

    struct CircularMotionJob : IJobParallelForTransform
    {
        public float deltaTime;
        public float speed;
        public float radius;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = transform.position;
            float angle = Mathf.Atan2(position.z, position.x);
            angle += speed * deltaTime;

            transform.position = new Vector3(
                Mathf.Cos(angle) * radius,
                position.y,
                Mathf.Sin(angle) * radius
            );
        }
    }
}

