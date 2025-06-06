using UnityEngine;

public class SpriteActivator : MonoBehaviour
{
    public Collider2D zoneA;                  // Зона начала отсчёта
    public Collider2D zoneB;                  // Зона отмены
    public Collider2D character;                  // Зона отмены
    public SpriteRenderer targetSprite;       // Целевой спрайт
    public float requiredTimeInZoneA = 3f;    // Время ожидания

    private bool timerRunning = false;
    private float timer = 0f;

    private bool playerInsideA = false;

    private void Update()
    {
        if (character.transform.position.x >= zoneA.transform.position.x)
        {
            timerRunning = true;
        }

        if (timerRunning)
        {
            timer += Time.deltaTime;

            if (timer >= requiredTimeInZoneA)
            {
                // Игрок НЕ вошёл в зону B вовремя — включаем спрайт
                targetSprite.enabled = true;
            }
        }

        if (character.transform.position.x >= zoneB.transform.position.x)
        {
            timerRunning = false;
            timer = 0;
        }
    }
}
