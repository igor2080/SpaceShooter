using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter_Game
{
    class Bullet
    {
        public Texture2D type;
        public Rectangle rect;
        public BulletType bulletType;
        public int damage;

        public Bullet(int rectX, int rectY, int width, int height, Texture2D type, int damage,BulletType bulletType)
        {
            this.type = type;
            this.rect=new Rectangle(rectX,rectY,width,height);
            this.damage=damage;
            this.bulletType = bulletType;
        }


        public int Nbullets = 3;//bullet upgrade

        public void SetBulletUpgrade(int upgradeLevel)
        {
            Nbullets = upgradeLevel;

        }
    }
}
