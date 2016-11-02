using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter_Game
{
    class Enemy
    {
        public Rectangle rect;
        public Texture2D type;
        public int life;

        public Enemy(int rectX, int rectY, int width, int height,Texture2D type,int life)
        {
            this.rect = new Rectangle(rectX, rectY, width, height);
            this.type = type;
            this.life = life;
        }

        public Texture2D getType()
        {
            return type;
        }
        public int getEnemyExp()
        {
            
                if (type.Name == "shooter" )
                {
                    return 2;
                }
                else if (type.Name == "speedy" )
                {
                    return 6;
                }
                else if (type.Name == "userBoss" )
                {
                    return 50;
                }
                else if (type.Name == "chaser")
                {
                    return 10;
                }
            
            throw new ApplicationException("no enemy specified,or something");
            
            
        }
        
        public void deduct(int damage)
        {
            this.life -= damage;
        }

    
    }
}
