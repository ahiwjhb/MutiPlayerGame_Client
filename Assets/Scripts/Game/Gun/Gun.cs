# nullable enable
using Helper;
using UnityEngine;

namespace MultiPlayerGame.Game
{

    public class Gun : MonoBehaviour
    {
        [SerializeField] GameObject _bulletPrefab;

        [SerializeField] Transform _firePoint;

        [SerializeField] float fireInterval = 0.2f;

        private CDTimer fireCdTimer = new();

        public void Fire() {
            if (fireCdTimer.IsTimeUp()) {
                var ob = Instantiate(_bulletPrefab);
                ob.transform.position = _firePoint.position;
                var args = ob.transform.eulerAngles;
                args.y = transform.eulerAngles.y;
                ob.transform.eulerAngles = args;
                fireCdTimer.ReStart(fireInterval);
            }
        }
    }
}
