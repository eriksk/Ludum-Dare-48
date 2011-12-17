using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gex.Util;

namespace Gex.Cameras
{
    public class Camera2D
    {
        private Vector3 position, origin, destination;
        private Matrix matrix;
        private float speed;

        public Camera2D(Vector2 origin)
        {
            this.origin = new Vector3(origin.X, origin.Y, 0f);
            this.position = this.origin;
            speed = 0.2f;
        }

        public Matrix Matrix
        {
            get
            {
                return matrix;
            }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public void Move(Vector2 destination)
        {
            this.destination.X = destination.X;
            this.destination.Y = destination.Y;
        }

        public Vector2 LocalToWorld(Vector2 local)
        {
            Vector2 pos = new Vector2();
            pos.X = position.X - origin.X + local.X;
            pos.Y = position.Y - origin.Y + local.Y;
            return pos;
        }

        public Vector2 WorldToLocal(Vector2 world)
        {
            Vector2 pos = new Vector2();
            pos.X = (position.X + origin.X) - world.X;
            pos.Y = (position.Y + origin.Y) - world.Y;
            return pos;
        }

        public void Update(float time)
        {
            position.X = Utility.Lerp(position.X, destination.X, speed);
            position.Y = Utility.Lerp(position.Y, destination.Y, speed);

            //Update matrix
            matrix = Matrix.CreateTranslation(-position + origin);
        }

        public void Reset()
        {
            position = origin;
        }
    }
}
