using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class SpecialEffectPool : ObjectPool
    {
        public GameObject textEffectPrefab;
        GameObject PoolEffect(GameObject prefab, float delay = 0, float scale = 1)
        {
            GameObject effect = PoolItem(prefab);
            if (effect == null)
                return null;

            ActivateObject(effect);
            if (effect.TryGetComponent(out SpecialEffectController sec))
            {
                sec.assignedObjectPool = this;
                sec.Emit(delay, scale);
            }

            return effect;
        }

        public GameObject EffectFromPrefab(GameObject prefab, Vector3 pos, float delay = 0, float scale = 1)
        {
            GameObject effect = PoolEffect(prefab, delay, scale);
            if (effect == null)
                return null;

            effect.transform.position = pos;
            effect.transform.localScale = Vector3.one ;
            return effect;
        }

        public GameObject AttachEffectFromPrefab(GameObject parent, GameObject prefab, float delay = 0, float scale = 1)
        {
            GameObject effect = PoolEffect(prefab, delay, scale);
            if (effect == null)
                return null;

            effect.transform.parent = parent.transform;

            effect.transform.localPosition = Vector3.zero;
            effect.transform.localRotation = Quaternion.identity;
            effect.transform.localScale = Vector3.one ;

            return effect;
        }

        public TextEffectController TextEffect(float number, Vector3 position, Color color, float delay = 0, float scale = .1f, string animation = "Float Up")
        {
            return TextEffect(textEffectPrefab, (Mathf.Ceil(number * 10) / 10).ToString(), position, color, delay, scale, animation);
        }
        public TextEffectController TextEffect(string text, Vector3 position, Color color, float delay = 0, float scale = .1f, string animation = "Float Up")
        {
            return TextEffect(textEffectPrefab, text, position, color, delay, scale, animation);
        }
        public TextEffectController TextEffect(GameObject prefab, string text, Vector3 position, Color color, float delay = 0, float scale = .1f, string animation = "Float Up")
        {
            GameObject effect = PoolEffect(prefab, delay, scale);
            if (effect == null)
                return null;
            /* if (effect.TryGetComponent(out RectTransform rectT))
             {
                 rectT.anchoredPosition = position;
             }
             else*/
            {
                effect.transform.position = position;
            }
            if (effect.TryGetComponent(out TextEffectController tefX))
            {
                tefX.ChangeTextValue(text);
                tefX.ChangeColor(color);
                tefX.Emit(animation, delay, scale);
                return tefX;
            }
            return null;
        }
}