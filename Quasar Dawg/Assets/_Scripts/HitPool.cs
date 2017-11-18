﻿using UnityEngine;

internal class HitPool : MonoBehaviour
{
    static HitPool instance = null;

    public int poolSize = 100;
    public float effectDuration = 1;
    public GameObject explosion;

    private int poolPosition;

    struct Effect
    {
        public GameObject gameObject;
        public float onTime;
        public bool on;
    }

    Effect[] effects;

    private void Start()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; GameObject.DontDestroyOnLoad(gameObject); }

        poolPosition = poolSize;
        effects = new Effect[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            effects[i].gameObject = Instantiate(explosion, transform.position, Quaternion.identity, transform);
            effects[i].onTime = 0;
            effects[i].on = false;
            effects[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < poolSize; i++)
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

    public GameObject HitEffect(Transform transform)
    {
        poolPosition++;
        if (poolPosition > poolSize) poolPosition = 0;

        if (effects[poolPosition].on) return null;
        else
        {
            effects[poolPosition].on = true;
            effects[poolPosition].onTime = Time.time;
            effects[poolPosition].gameObject.transform.position = transform.position;
            return effects[poolPosition].gameObject;
        }
    }
}
