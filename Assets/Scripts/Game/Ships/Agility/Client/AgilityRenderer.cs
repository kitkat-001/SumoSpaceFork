﻿using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Ships.Agility.Client
{
    public class AgilityRenderer: ShipRenderer
    {

        private float lrTilt;
        
        public void PrimaryMuzzleFlash()
        {
            transform.Find("../Sphere/BulletSpawn/MuzzlePrefab").GetComponent<VisualEffect>().Play();
        }

        public void Tilt(float lrMovement)
        {
            lrTilt += lrMovement * Time.deltaTime;
            lrTilt = Mathf.Clamp(lrTilt, -1, 1);
        }

        public void DodgeEffect()
        {
            var dodgeVFX = transform.Find("../Sphere/AgileShip/DashVFX").GetComponent<VisualEffect>();
            dodgeVFX.SetVector3("Rotation", gameObject.GetComponentInParent<Transform>().eulerAngles);
            dodgeVFX.Play();
            StartCoroutine(StopDash());
        }

        private IEnumerator StopDash()
        {
            yield return new WaitForSeconds(.2f);
            transform.Find("../Sphere/AgileShip/DashVFX").GetComponent<VisualEffect>().Stop();
        }
    }
}