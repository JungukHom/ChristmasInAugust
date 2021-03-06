﻿using UniRx;
using System;
using Utility;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Manager
{
    public class ItemManager : Manager
    {
        private Player.Player player;

        private CalculateManager calculateManager;
        private EffectManager effectManager;
        private GameManager gameManager;

        private readonly string ITEM_PATH = "Prefabs/Item/";
        private readonly float spawnHeight = 5.0f;

        private bool isCloudDisplaying = false;
        private bool isWatchDisplaying = false;

        public GameObject cloudSpawnParticle;
        public GameObject cloudItem;
        private ParticleSystem cloudItemKeepParticle;

        private readonly float cloudDisplayTime = 3.0f;
        private readonly float cloudFadeOutTime = 1.0f;
        private readonly float cloudSpawnDelayTime = 3.0f;

        private GameObject cloud;
        private Image cloudImage;

        private GameObject whiteWall;
        private Image whiteWallImage;

        public GameObject watchSpawnParticle;
        public GameObject watchItem;

        private Button watchButton;

        private readonly float watchDisplayTime = 3.5f;
        private readonly float watchFadeOutTime = 1.5f;
        private readonly float watchSpawnDelayTime = 3.0f;

        public float GetRandomDelayTime(float min = 5.0f, float max = 10.0f)
        {
            return UnityEngine.Random.Range(min, max);
        }

        private void Awake()
        {
            player = FindComponent<Player.Player>("Snowman");
            watchButton = FindComponent<Button>("WatchButton");
            calculateManager = GetOrCreateManager<CalculateManager>();
            effectManager = GetOrCreateManager<EffectManager>();
            gameManager = GetGameManager();

            cloudItemKeepParticle = FindComponent<ParticleSystem>("CloudItemKeepParticle");
            cloudItemKeepParticle.Stop();
        }

        private void Start()
        {
            // cloudSpawnParticle = Resources.Load($"{ITEM_PATH}CloudParticle") as GameObject;
            // cloudItem = Resources.Load($"{ITEM_PATH}CloudItem") as GameObject;

            InitCloud();
            InitWatch();
        }

        private void InitCloud()
        {
            cloud = GameObject.Find("Cloud");
            cloudImage = cloud.GetComponent<Image>();
            Color color = cloudImage.color;
            color.a = 0;
            cloudImage.color = color;
        }

        private void InitWatch()
        {
            whiteWall = GameObject.Find("WhiteWall");
            whiteWallImage = whiteWall.GetComponent<Image>();
            Color color = whiteWallImage.color;
            color.a = 0;
            whiteWallImage.color = color;

            watchButton.onClick
                .AsObservable()
                .Subscribe((x) =>
                {
                    UseWatch();
                });
        }

        public void SpawnCloud(float delayTime, Action<object> cb = null)
        {
            StartCoroutine(ESpawnCloud(delayTime, myCB => { cb(null); }));
        }

        public void SpawnSlowWatch(float delayTime, Action<object> cb = null)
        {
            StartCoroutine(ESpawnSlowWatch(delayTime, myCB => { cb(null); }));
        }

        private IEnumerator ESpawnCloud(float delayTime, Action<object> callback = null)
        {
            yield return new WaitForSeconds(delayTime);

            int randomAngle = UnityEngine.Random.Range(0, 360);
            //Vector3 position = calculateManager.GetPosition(randomAngle, spawnHeight);

            //GameObject particle = Instantiate(cloudSpawnParticle, position, Quaternion.identity); // rotation
            //yield return new WaitForSeconds(cloudSpawnDelayTime);
            //Destroy(particle);
            //Instantiate(cloudItem, position, Quaternion.identity);

            GameObject particle = Instantiate(cloudSpawnParticle, Vector2.zero, Quaternion.Euler(new Vector3(0, 0, randomAngle))); // rotation
            yield return new WaitForSeconds(cloudSpawnDelayTime);
            Destroy(particle);
            
            Instantiate(cloudItem, Vector2.zero, Quaternion.Euler(new Vector3(0, 0, randomAngle)));

            callback(null);
        }

        private IEnumerator ESpawnSlowWatch(float delayTime, Action<object> callback = null)
        {
            yield return new WaitForSeconds(delayTime);

            int randomAngle = UnityEngine.Random.Range(0, 360);
            //Vector3 position = calculateManager.GetPosition(randomAngle, spawnHeight);

            //GameObject particle = Instantiate(watchSpawnParticle, position, Quaternion.identity); // rotation
            //yield return new WaitForSeconds(watchSpawnDelayTime);
            //Destroy(particle);

            //Instantiate(watchItem, position, Quaternion.identity);

            GameObject particle = Instantiate(watchSpawnParticle, Vector2.zero, Quaternion.Euler(new Vector3(0, 0, randomAngle))); // rotation
            yield return new WaitForSeconds(watchSpawnDelayTime);
            Destroy(particle);

            Instantiate(watchItem, Vector2.zero, Quaternion.Euler(new Vector3(0, 0, randomAngle)));

            callback(null);
        }

        public void UseCloud()
        {
            if (!isCloudDisplaying)
            {
                StartCoroutine(EUseCloud(cloudDisplayTime, cloudFadeOutTime));
            }
        }

        public void UseWatch()
        {
            if (Data.Item.SlowWatch >= 1 && !isWatchDisplaying)
            {
                Data.Item.SlowWatch--;
                StartCoroutine(EUseWatch(watchDisplayTime, watchFadeOutTime));
            }
        }

        private IEnumerator EUseCloud(float displayTime, float fadeOutTime)
        {
            isCloudDisplaying = true;
            cloudItemKeepParticle.Play();

            Color color = cloudImage.color;
            color.a = 1;
            cloudImage.color = color;
            cloud.GetComponent<BoxCollider2D>().enabled = true;
            yield return new WaitForSeconds(displayTime);
            cloudItemKeepParticle.Stop();
            effectManager.FadeOut(cloudImage, fadeOutTime, (x) =>
            {
                isCloudDisplaying = false;
                cloud.GetComponent<BoxCollider2D>().enabled = false;
            });
        }

        private IEnumerator EUseWatch(float displayTime, float fadeOutTime)
        {
            isWatchDisplaying = true;
            gameManager.SetTimeScale(0.5f);
            player.Speed *= 2;

            Color color = whiteWallImage.color;
            color.a = 1f;
            whiteWallImage.color = color;
            yield return new WaitForSecondsRealtime(displayTime);
            effectManager.FadeOut(whiteWallImage, fadeOutTime, (x) =>
            {
                gameManager.SetTimeScale(1.0f);
                player.Speed /= 2;
                isWatchDisplaying = false;
            });
        }
    }
}