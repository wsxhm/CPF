using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CPF.Drawing.Media3D
{
    class RayHitTestParameters
    {
        /// <summary>
        ///     Creates a RayHitTestParameters where the ray is described
        ///     by an origin and a direction.
        /// </summary>
        public RayHitTestParameters(Point3D origin, Vector3D direction)
        {
            _origin = origin;
            _direction = direction;
            //_isRay = true;
        }
        
        private readonly Point3D _origin;
        private readonly Vector3D _direction;
        // 'true' if this is a ray hit test, 'false' if the ray has become a line
        //private bool _isRay;
        /// <summary>
        ///     The origin of the ray to be used for hit testing.
        /// </summary>
        public Point3D Origin
        {
            get { return _origin; }
        }

        /// <summary>
        ///     The direction of the ray to be used for hit testing.
        /// </summary>
        public Vector3D Direction
        {
            get { return _direction; }
        }
        internal bool HasHitTestProjectionMatrix
        {
            get { return _hitTestProjectionMatrix != null; }
        }
        private Matrix3D? _hitTestProjectionMatrix = null;

        internal Matrix3D HitTestProjectionMatrix
        {
            get
            {
                Debug.Assert(HasHitTestProjectionMatrix,
                    "Check HasHitTestProjectionMatrix before accessing HitTestProjectionMatrix.");

                return _hitTestProjectionMatrix.Value;
            }

            set
            {
                _hitTestProjectionMatrix = new Matrix3D?(value);
            }
        }
    }
}
