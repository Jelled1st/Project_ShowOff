using System;
using System.Collections;
using StateMachine;
using UnityEngine;

namespace Factory
{
    public class PackageManager : LazySingleton<PackageManager>
    {
        private static FactoryStageSettings FactoryStageSettings =>
            GameStageManager.GetStageSettings<FactoryStageSettings>();

        private void Start()
        {
            SpawnPackage();
        }

        private void OnEnable()
        {
            PackageController.PackageStuck += OnPackageStuck;
            PackageController.PackageWentUnderGround += OnPackageWentUnderGround;
        }

        private void OnDisable()
        {
            PackageController.PackageStuck -= OnPackageStuck;
            PackageController.PackageWentUnderGround -= OnPackageWentUnderGround;
        }

        private void OnPackageWentUnderGround(PackageController obj)
        {
            RespawnPackageDelayed(obj);
        }

        private void OnPackageStuck(PackageController obj)
        {
            RespawnPackageDelayed(obj);
        }

        private void RespawnPackageDelayed(PackageController packageController)
        {
            StartCoroutine(WaitAndExecute(FactoryStageSettings.RespawnTime,
                () => { RespawnPackageImmediate(packageController); }));
        }

        private static IEnumerator WaitAndExecute(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        private static void RespawnPackageImmediate(PackageController packageController)
        {
            Destroy(packageController.gameObject);
            SpawnPackage();
        }

        public static void SpawnPackage()
        {
            var startPosition = GameObject.FindWithTag("StartPosition");

            if (startPosition == null)
            {
                throw new Exception(
                    "Can't find the start position! (Create and position an empty gameobject with tag 'StartPosition')");
            }

            var newPackage = Instantiate(FactoryStageSettings.PackagePrefab, startPosition.transform.position,
                FactoryStageSettings.PackagePrefab.transform.rotation);

            newPackage.GetComponentInChildren<Rigidbody>()
                .AddForce(newPackage.transform.forward * FactoryStageSettings.PushForce);
        }
    }
}