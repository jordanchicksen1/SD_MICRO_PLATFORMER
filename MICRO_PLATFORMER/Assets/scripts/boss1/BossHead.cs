using UnityEngine;

public class BossHead : MonoBehaviour
{
    BossController boss;
    bool vulnerable;
    [SerializeField] AudioSource gruntSFX;

    public void SetBoss(BossController b)
    {
        boss = b;
    }

    public void SetVulnerable(bool value)
    {
        vulnerable = value;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!vulnerable) return;

        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (!player) return;

        if (player.IsGroundPounding())
        {
            if (gruntSFX) gruntSFX.Play();

            boss.DamageBoss();
            CameraShake.Shake(0.25f, 0.15f);
        }
    }


}