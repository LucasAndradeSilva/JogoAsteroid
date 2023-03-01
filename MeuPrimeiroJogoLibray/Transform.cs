using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeuPrimeiroJogoLibrary
{
    public class Transform
    {
        public Vector2 Position{ get; set; } = Vector2.Zero;
        //public float X
        //{
        //    get
        //    {
        //        return Position.X;
        //    }
        //    set
        //    {
        //        Position = new Vector2(value, Y);
        //    }
        //}
        //public float Y
        //{
        //    get
        //    {
        //        return Position.Y;
        //    }
        //    set
        //    {
        //        Position = new Vector2(X, value);
        //    }
        //}
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0;

        public Transform()
        {

        }

        public Transform(Transform source)
        {
            this.Position= source.Position;
            this.Velocity= source.Velocity;
            this.Scale = source.Scale;
            this.Rotation = source.Rotation;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
        }
    }
}
