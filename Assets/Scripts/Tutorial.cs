using UnityEngine;

public class SpriteActivator : MonoBehaviour
{
    public Collider2D zoneA;                  // ���� ������ �������
    public Collider2D zoneB;                  // ���� ������
    public Collider2D character;                  // ���� ������
    public SpriteRenderer targetSprite;       // ������� ������
    public float requiredTimeInZoneA = 3f;    // ����� ��������

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
                // ����� �� ����� � ���� B ������� � �������� ������
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
