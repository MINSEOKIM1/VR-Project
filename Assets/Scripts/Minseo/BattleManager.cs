using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public Image hitPanel;
    
    public int hp;
    public int maxHp;

    public string restartScene;

    public GameObject alertUI;
    
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
        GameManager.Instance.currentHP = hp;
        StartCoroutine(HitPanelEffect());

        if (hp <= maxHp * 0.3f && !alertUI.activeSelf)
        {
            StartCoroutine(AlertUI());
        }

        if (hp <= 0)
        {
            SceneManager.LoadScene(restartScene);
        }
    }

    IEnumerator AlertUI()
    {
        var cg = alertUI.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        alertUI.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);
        
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            cg.alpha = alpha;
            yield return null;
        }
        alertUI.SetActive(false);
    }

    public void Attack(int damage, MonsterAI monster)
    {
        monster.hp -= damage;
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
