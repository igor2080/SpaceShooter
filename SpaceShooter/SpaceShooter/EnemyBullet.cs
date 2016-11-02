using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shooter_Game
{
    class EnemyBullet
    {
        public Texture2D type;
        public Rectangle rect;
        public Enemy enemy;
        public EnemyBulletType BulletType;

        public EnemyBullet(Enemy enemy,Texture2D type,int x,int y,EnemyBulletType bulletType)
        {
            this.enemy = enemy;
            this.type = type;
            this.rect = new Rectangle(x, y, 15, 15);//enemy.rect.Left + 15,enemy.rect.Bottom
            this.BulletType = bulletType;
        }
    }
}
