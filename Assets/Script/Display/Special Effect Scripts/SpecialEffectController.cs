using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class SpecialEffectController : MonoBehaviour
    {
        protected ParticleSystem[] particles;
        protected Animator animator;
        protected float realScale = 1;
        protected virtual void Awake()
        {
            List<ParticleSystem> pSys = new List<ParticleSystem>();
            if (TryGetComponent(out ParticleSystem particle))
                pSys.Add(particle);
            pSys.AddRange(GetComponentsInChildren<ParticleSystem>());
            particles = pSys.ToArray();
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        public virtual void Emit(float delay, float scale = 1, bool kill = true)
        {
            realScale = scale;
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
            StartCoroutine(EmitOnce(delay, kill));
        }
        protected virtual IEnumerator EmitOnce(float delay, bool kill = true)
        {
            reps = 0;
            Scale(0,true);
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            Scale(realScale, false);
            Play();
        }
        public void Scale(float val, bool local)
        {
            if (local)
                transform.localScale = Vector3.one * val;
            else
                transform.localScale = Vector3.one * transform.lossyScale.x / val;
        }

        public virtual void Emit(float delay, int repeats, float scale = 1)
        {
            realScale = scale;
            float duration = 0;
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                duration = Mathf.Max(pmain.duration + pmain.startLifetime.constantMax);
            }
            StartCoroutine(EmitMultiple(delay, duration, repeats, scale));
        }
        public virtual void ChangeColor(Color color)
        {
            foreach (ParticleSystem p in particles)
            {
                ParticleSystem.MainModule pmain = p.main;
                pmain.startColor = color;
            }
        }
        int reps = 0;
        protected virtual IEnumerator EmitMultiple(float delay, float duration, int repeats, float scale)
        {
            realScale = scale;
            reps = repeats;
            Scale(0, true);
            yield return new WaitForSeconds(delay);
            Scale(realScale, false);
            while (reps > 0)
            {
                Play();
                reps--;
                yield return new WaitForSeconds(duration);
            }
        }
        protected void Play()
        {
            foreach (ParticleSystem p in particles)
            {
                p.Play();
            }
            if (animator != null)
                animator.SetTrigger("Play");
        }
        public ObjectPool assignedObjectPool;
        public void Stop()
        {
            foreach (ParticleSystem p in particles)
            {
                p.Stop();
            }
            assignedObjectPool?.DeactivateObject(gameObject);
        }
        private void OnParticleSystemStopped()
        {
            Kill();
        }
        protected virtual void Kill()
        {
            if (reps == 0)
            {
                Stop();
            }
        }

}