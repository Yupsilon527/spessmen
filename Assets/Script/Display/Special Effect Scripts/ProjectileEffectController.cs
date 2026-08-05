using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ProjectileEffectController : MonoBehaviour
    {
        #region Rotation
        public enum RotationBehavior
        {
            nothing,
            arrow,
            lobbed
        }
        public float lingerDuration = 0;
        public RotationBehavior rotationBehavior;
        public GameObject explodeEffect;
        void AdjustRotation()
        {
            switch (rotationBehavior)
            {
                case RotationBehavior.arrow:
                    transform.right = (Vector2)velocity;
                    break;
                case RotationBehavior.lobbed:
                    float fwdAngle = Mathf.Atan2(velocity.y, (velocity.x > 0 ? -1 : 1) * velocity.x) * Mathf.Rad2Deg;
                    transform.eulerAngles = Vector3.forward * fwdAngle;
                    break;
            }
        }
        #endregion
        ParticleSystem[] particles;

        protected virtual void Awake()
        {
            List<ParticleSystem> pSys = new List<ParticleSystem>();
            if (TryGetComponent<ParticleSystem>(out ParticleSystem mine))
                pSys.Add(mine);
            pSys.AddRange(GetComponentsInChildren<ParticleSystem>());
            particles = pSys.ToArray();
        }
        void Recolor(Color color)
        {
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                pmain.startColor = color;
            }
        }
        #region Fire Point
        public virtual void LaunchTowardsPosition(Vector3 start, Vector3 destination, Color color, float delay = 0, float travelSpeed = 1, bool rotating = true)
        {
            gameObject.SetActive(true);
            Recolor(color);
            transform.position = start;
            StartCoroutine(FirePosition(delay, travelSpeed, destination));
        }
        Vector3 velocity;
        protected virtual IEnumerator FirePosition(float delay, float projectileSpeed, Vector3 destination)
        {
            Vector3 origin = transform.position;
            velocity = (destination - origin);

            ChangeVisibility(false);
            yield return new WaitForSeconds(delay);
            ChangeVisibility(true);
            for (float t = 0; t <= 1; t += Time.fixedDeltaTime / projectileSpeed)
            {
                transform.position = Vector3.Lerp(origin, destination, t);
                AdjustRotation();
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(lingerDuration);
            Die();
        }
        #endregion
        #region Fire Target
        public virtual void LaunchAtTarget(Vector3 start, Transform target, Color color, float delay = 0, float travelSpeed = 1, bool rotating = true)
        {
            gameObject.SetActive(true);
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                pmain.startColor = color;
            }
            transform.position = start;
            StartCoroutine(FireTarget(delay, travelSpeed, target));
        }
        protected virtual IEnumerator FireTarget(float delay, float projectileSpeed, Transform target)
        {
            Vector3 origin = transform.position;
            velocity = (target.transform.position - origin);

            ChangeVisibility(false);
            yield return new WaitForSeconds(delay);
            ChangeVisibility(true);

            for (float t = 0; t <= 1; t += Time.fixedDeltaTime / projectileSpeed)
            {
                transform.position = Vector3.Lerp(origin, target.position, t);
                AdjustRotation();
                yield return new WaitForEndOfFrame();
            }
            AttachToObject(target);
            yield return new WaitForSeconds(lingerDuration);
            Die();
        }
        #endregion
        void AttachToObject(Transform target)
        {
            transform.SetParent(target);
            transform.localScale = Vector3.one;
        }
        bool Visible = false;
        void ChangeVisibility(bool visible)
        {
            Visible = visible;

            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer mSprite))
                mSprite.enabled = visible;
            foreach (ParticleSystem p in particles)
            {
                if (!visible)
                    p.Stop();
                else
                    p.Play();
            }
        }
        public void Die()
        {
            ChangeVisibility(false);
            if (explodeEffect != null)
                ArenaController.main.epool.EffectFromPrefab(explodeEffect, transform.position, 0);
            ArenaController.main.epool.DeactivateObject(gameObject);
        }
    }
