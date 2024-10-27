# nullable enable
using UnityEngine;

namespace MultiPlayerGame.Game
{

    public class Bullet : MonoBehaviour
    {
        [SerializeField] float speed = 5;

        [SerializeField] float hitPower = 2f;

        [SerializeField] float lifeTime = 10;

        public int ID { get; set; }

        private void FixedUpdate() {
            transform.position += transform.right * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            if(lifeTime < 0) {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.tag == "Player" && collision.TryGetComponent<Rigidbody2D>(out var rb)) {
                rb.AddForce(transform.right * hitPower, ForceMode2D.Impulse);
                Destroy(gameObject);
            }
        }
    }
}
