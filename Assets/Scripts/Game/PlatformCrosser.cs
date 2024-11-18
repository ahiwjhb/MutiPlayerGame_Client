# nullable enable
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace MultiPlayerGame.Game
{
    public class PlatformCrosser : MonoBehaviour
    {
        private Collider2D _selfCollier = null!;

        private Collider2D? _locatedPlatformCollier;

        private void Awake() {
            _selfCollier = GetComponent<Collider2D>();
        }

        public async void CrossPlatform() {
            if (_locatedPlatformCollier != null) {
                var colliderCahce = _locatedPlatformCollier;

                Physics2D.IgnoreCollision(_selfCollier, colliderCahce);
                await UniTask.Delay(TimeSpan.FromSeconds(0.25));
                Physics2D.IgnoreCollision(_selfCollier, colliderCahce, false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent<PlatformEffector2D>(out var _)) {
                _locatedPlatformCollier = collision.collider;
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent<PlatformEffector2D>(out var _)) {
                _locatedPlatformCollier = null;
            }
        }
    }
}
