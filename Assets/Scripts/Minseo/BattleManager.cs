using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public Image hitPanel;
    
    public int hp;
    public int maxHp;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(HitPanelEffect());
    }

    public void Attack(int damage)
    {
        StartCoroutine(AttackPanelEffect());
    }

    private IEnumerator HitPanelEffect()
    {
        hitPanel.enabled = true;
        hitPanel.color = new Color(1, 0, 0, 0.5f);
        float a = 0.5f;
        while (hitPanel.color.a > 0)
        {
            a -= Time.deltaTime;
            a = Mathf.Clamp01(a);
            hitPanel.color = new Color(1, 0, 0, a);
            yield return null;
        }
        
        hitPanel.enabled = false;
    }
    
    private IEnumerator AttackPanelEffect()
    {
        hitPanel.enabled = true;
        hitPanel.color = new Color(0, 0, 1, 0.5f);
        float a = 0.5f;
        while (hitPanel.color.a > 0)
        {
            a -= Time.deltaTime;
            a = Mathf.Clamp01(a);
            hitPanel.color = new Color(0, 0, 1, a);
            yield return null;
        }
        
        hitPanel.enabled = false;
    }
}
