﻿using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] int initialPoolSize = 20;
    [SerializeField][Tooltip("Allow pool to grow as needed (if checked)")] bool dynamicPool = false;
    [SerializeField][Range(1, 50)] int poolGrowthRate = 5;
    [SerializeField] GameObject explosion;
    [SerializeField] float effectDuration = 1;

    private int poolPosition;
    private int dynamicPoolSize;

    struct Effect
    {
        public GameObject gameObject;
        public float onTime;
        public bool on;
    }

    Effect[] effects;

    private void Start()
    {
        poolPosition = initialPoolSize;
        dynamicPoolSize = initialPoolSize;
        effects = new Effect[initialPoolSize];

        for (int i = 0; i < initialPoolSize; i++) CreateEffect(i);
    }

    private void CreateEffect(int i)
    {
        effects[i].gameObject = Instantiate(explosion, transform.position, Quaternion.identity, transform);
        effects[i].onTime = 0;
        effects[i].on = false;
        effects[i].gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int i = 0; i < dynamicPoolSize; i++)
        {
            if (effects[i].on)
            {
                if (Time.time > effects[i].onTime + effectDuration)
                {
                    effects[i].on = false;
                    effects[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void GrowPool()
    {
        Effect[] temp = new Effect[dynamicPoolSize];
        for (int i = 0; i < dynamicPoolSize; i++)
        {
            temp[i] = effects[i];
        }
        dynamicPoolSize += poolGrowthRate;

        effects = new Effect[dynamicPoolSize];
        for (int i = 0; i < dynamicPoolSize - poolGrowthRate; i++)
        {
            effects[i] = temp[i];
        }
        for (int i = 0; i < poolGrowthRate; i++)
        {
            CreateEffect(i + dynamicPoolSize - poolGrowthRate);
        }
    }

    public void PopEffect(Transform transform)
    {
        poolPosition++;
        if (poolPosition >= dynamicPoolSize) poolPosition = 0;

        if (effects[poolPosition].on && dynamicPool)
        {
            GrowPool();
            poolPosition++;
        }
        RecycleEffect(transform, poolPosition);
    }

    private void RecycleEffect(Transform transform, int position)
    {
        effects[position].on = true;
        effects[position].onTime = Time.time;
        effects[position].gameObject.transform.position = transform.position;
        effects[position].gameObject.SetActive(true);
    }
}
