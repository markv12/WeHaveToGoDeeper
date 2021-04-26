using System.Collections;
using UnityEngine;

public class CircleFish : MonoBehaviour {

    public Transform centerT;
    public SpriteRenderer fishRenderer;

    private int spriteIndex = 0;
    public Sprite[] sprites;

    public float speed = 50;

    private void Update() {
        centerT.Rotate(0, 0, speed * Time.deltaTime);
    }

    private void OnEnable() {
        fishRenderer.flipX = speed < 0;
        StartCoroutine(SpriteRoutine());
        IEnumerator SpriteRoutine() {
            float timeSinceLastSprite = 0;
            while (true) {
                if (timeSinceLastSprite >= StraightFish.TIME_PER_SPRITE) {
                    spriteIndex = (spriteIndex + 1) % sprites.Length;
                    fishRenderer.sprite = sprites[spriteIndex];
                    timeSinceLastSprite -= StraightFish.TIME_PER_SPRITE;
                }
                timeSinceLastSprite += Time.deltaTime;
                yield return null;
            }
        }
    }
}
