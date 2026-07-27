
using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public class HealthbarController : MonoBehaviour
    {
        Resource health;

        public TextMeshProUGUI HealthValue;
        public Image fill;

        public void AssignResource(Resource health)
        {
            this.health = health;
            health.OnValueChanged.AddListener(OnValueChange);
            OnValueChange();
        }
        private void OnEnable()
        {
            OnValueChange();
        }
        public void OnValueChange()
        {
            if (health == null)
                return;

            float hValue = health.GetValue();
            float hTotal = health.GetLimit(false) ;

            fill.fillAmount = hValue / hTotal;
            if (HealthValue != null)
                HealthValue.text = $"{Mathf.Ceil(hValue )}/{Mathf.Ceil(hTotal)}";

        }
        private void OnDisable()
        {
            if (health != null)
                health.OnValueChanged.RemoveListener(OnValueChange);
        }
    }
