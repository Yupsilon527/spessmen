using System;
using System.Collections;
using UnityEngine;

public class Toon : MonoBehaviour
{
    public Animator animator;
    public CharacterResolver character;
    public PlayerStats overlay;
    public Countdown nextAlertTime = new();
    public enum AlertImportance
    {
        Always,
        Announcements,//miss, etc
        Damage,//modifiers, damage
        Optional,//gray damage, etc
    }
    public bool IsAnimating()
    {
        return !animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
    }
    int animPrio = 0;
    public void ResetAnimation()
    {
        currentAnimation = "Idle";
        animPrio = -1;
    }
    string currentAnimation = "";
    public void PlayAnimation(string animName, int priority = 0, float fadeTime = .1f, float delay = 0, bool forced = false)
    {
        if (animPrio > 0 && priority <= animPrio)
        {
            return;
        }
        if (!forced && currentAnimation == animName) return;
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        currentAnimation = animName;
        Debug.Log($"Toon {name} play animation {animName} at priority {priority}, current {currentAnimation}");
        if (delay > 0)
        {
            animationCoroutine = StartCoroutine(PlayAnimationDelayed(delay, animName, fadeTime));
        }
        else
        {
            animPrio = priority;
            Animate(animName, fadeTime);
        }
    }
    Coroutine animationCoroutine;
    IEnumerator PlayAnimationDelayed(float delay, string animName, float fadeTime)
    {
        yield return new WaitForSeconds(delay);
        Animate(animName, fadeTime);
    }
    void Animate(string anim, float fadetime)
    {
        Debug.Log($"{name} plays animation {anim}");
        for (int i = 0; i < animator.layerCount; i++)
            animator.CrossFadeInFixedTime(anim, fadetime, i);
        character.Recenter();
    }
    public void Alert(string value, Color color, string attach = "chest", float delay = 0, string animation = "Float Up", float scale = 1, AlertImportance importance = AlertImportance.Always)
    {
        if (ArenaController.main.gameObject.activeSelf)
        {
            float fireTime = Mathf.Max(delay, nextAlertTime.GetTimeRemaining());
            var ef = ArenaController.main.epool.TextEffect(value, character.FindAttachPoint(attach).position, color: color, delay: fireTime, animation: animation, scale: scale * .1f);
            ef.transform.rotation = (ArenaController.main.camera.transform.rotation);
            nextAlertTime.Set(.2f);
        }
    }
    public void ResetAlertTimes()
    {
        nextAlertTime.Stop();
    }
    public void OverlayColor(Color color)
    {
        foreach (var kvp in character.SpriteRenderers)
        {
            if (!kvp.Value.CompareTag("Shadow") && kvp.Value is SpriteRenderer r)
            {
                r.color = color;
            }
        }
    }
    public void FlashColor(Color color, float delay = 0)
    {
        OverlayColor(color);
        if (delay == 0)
            animator.SetTrigger("Flash");
        else
            StartCoroutine(FlashColorDelay(delay));
    }
    IEnumerator FlashColorDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Flash");
    }
    public void ResetColor()
    {
        OverlayColor(new Color(1, 1, 1, 0));
    }
}
